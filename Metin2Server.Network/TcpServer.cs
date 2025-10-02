using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using MediatR;
using Metin2Server.Shared.Application;
using Metin2Server.Shared.Encryption;
using Metin2Server.Shared.Protocol;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Metin2Server.Network;

public class TcpServer
{
    private const int Backlog = 512;
    private const int ReceiveBufferSize = 8192;
    private const int SendBufferSize = 8192;

    private long _nextSessionId = 0;

    private readonly IMediator _mediator;
    private readonly PacketRuleRegistryIn _packetRuleRegistryIn;
    private readonly PacketRuleRegistryOut _packetRuleRegistryOut;
    private readonly PacketInRegistry _packetInRegistry;
    private readonly PacketOutRegistry _packetOutRegistry;
    private readonly IEnumerable<ISessionStartup> _sessionStartups;
    private readonly ISessionAccessor _sessionAccessor;
    private readonly PacketTransport _packetTransport;
    private readonly ILogger<TcpServer> _logger;

    private readonly ConcurrentDictionary<long, Session> _sessions = new();

    public TcpServer(
        IMediator mediator,
        PacketRuleRegistryIn packetRuleRegistryIn,
        PacketRuleRegistryOut packetRuleRegistryOut,
        PacketInRegistry packetInRegistry,
        PacketOutRegistry packetOutRegistry,
        IEnumerable<ISessionStartup> sessionStartups,
        ISessionAccessor sessionAccessor,
        PacketTransport packetTransport,
        ILogger<TcpServer> logger)
    {
        _mediator = mediator;
        _packetRuleRegistryIn = packetRuleRegistryIn;
        _packetRuleRegistryOut = packetRuleRegistryOut;
        _packetInRegistry = packetInRegistry;
        _packetOutRegistry = packetOutRegistry;
        _sessionStartups = sessionStartups;
        _sessionAccessor = sessionAccessor;
        _packetTransport = packetTransport;
        _logger = logger;
    }

    public async Task StartAsync(IPAddress ipAddress, int port, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"TCP server listening on {ipAddress}:{port}");

        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        socket.Bind(new IPEndPoint(ipAddress, port));
        socket.Listen(Backlog);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var client = await socket.AcceptAsync(cancellationToken);
                client.NoDelay = true;
                client.ReceiveBufferSize = ReceiveBufferSize;
                client.SendBufferSize = SendBufferSize;

                var id = Interlocked.Increment(ref _nextSessionId);
                var session = new Session(id, client);

                if (!_sessions.TryAdd(id, session))
                {
                    _logger.LogWarning($"Failed to register session {id}");
                    await session.DisposeAsync();
                    continue;
                }

                _logger.LogInformation($"Connected {id} {session.Endpoint}");
                _ = HandleClientAsync(session, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Listener loop error");
        }
        finally
        {
            try
            {
                socket.Close();
            }
            catch
            {
                // ignored
            }
        }
    }

    private async Task HandleClientAsync(Session session, CancellationToken cancellationToken)
    {
        try
        {
            Func<GameClientHeader, ReadOnlyMemory<byte>, CancellationToken, Task> sendDelegate =
                async (header, delegatePayload, token) =>
                {
                    // var seq = _packetRuleRegistryOut.TryGetRule(header, out var rule)
                    //     ? rule.SequenceBehavior
                    //     : SequenceBehavior.None;
                    // return session.SendAsync(_packetRuleRegistryOut, header, delegatePayload, seq, token);
                    await _packetTransport.SendAsync(session, header, delegatePayload, token);
                };

            foreach (var sessionStartup in _sessionStartups)
            {
                await sessionStartup.RunAsync(session, sendDelegate, cancellationToken);
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                // await foreach (var pkt in PacketFramer.ReadPacketsAsync(
                //                    session.Socket,
                //                    _packetRuleRegistryIn,
                //                    cancellationToken))
                await foreach(var pkt in _packetTransport.ReadAsync(session, cancellationToken))
                {
                    session.Touch();

                    var header = (ClientGameHeader)pkt.Header;
                    var payloadMem = pkt.Payload ?? ReadOnlyMemory<byte>.Empty;

                    if (!_packetRuleRegistryIn.TryGetRule(header, out var packetRule))
                    {
                        _logger.LogWarning($"[{session.Id}] Unknown header={header}");
                        continue;
                    }

                    if ((packetRule.AllowedSessionPhases & session.Phase) == 0)
                    {
                        _logger.LogWarning($"[{session.Id}] Header {header} not allowed in phase {session.Phase}");
                        continue;
                    }

                    var request = _packetInRegistry.ToRequest(header, payloadMem);

                    if (request is null)
                    {
                        _logger.LogWarning($"No IN route for {header}");
                        continue;
                    }

                    using (SessionAccessor.Scope(session))
                    {
                        await _mediator.Send(request, cancellationToken);

                        var collector = _sessionAccessor.CurrentPacketOutCollector;
                        if (collector.Items.Count <= 0)
                        {
                            continue;
                        }

                        foreach (var collectorItem in collector.Items)
                        {
                            if (!_packetOutRegistry.TrySerialize(collectorItem, out var outHeader, out var outPayload))
                            {
                                _logger.LogWarning($"[{session.Id}] Unknown OUT type for collector item");
                                continue;
                            }
                            
                            await _packetTransport.SendAsync(session, (GameClientHeader)outHeader, outPayload, cancellationToken);

                            // var seq = _packetRuleRegistryOut.TryGetRule((GameClientHeader)outHeader, out var rule)
                            //     ? rule.SequenceBehavior
                            //     : SequenceBehavior.None;
                            //
                            // await session.SendAsync(
                            //     _packetRuleRegistryOut,
                            //     (GameClientHeader)outHeader,
                            //     outPayload,
                            //     seq, 
                            //     cancellationToken);
                        }

                        collector.Clear();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"[{session.Id}] Session loop error");
        }
        finally
        {
            _sessions.TryRemove(session.Id, out _);
            await session.DisposeAsync();
            _logger.LogInformation($"Disconnected {session.Id} {session.Endpoint}");
        }
    }
}