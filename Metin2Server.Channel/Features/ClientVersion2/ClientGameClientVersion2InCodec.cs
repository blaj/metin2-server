using Metin2Server.Shared.Common;
using Metin2Server.Shared.Protocol.Codecs;

namespace Metin2Server.Channel.Features.ClientVersion2;

public class ClientGameClientVersion2InCodec : IPacketInCodec<ClientGameClientVersion2Packet>
{
    public ClientGameClientVersion2Packet Read(ReadOnlySpan<byte> payload)
    {
        var length = Constants.ClientVersion2Length + 1;
        var offset = 0;

        var filenameChars = new char[length];
        {
            var slice = payload.Slice(offset, length);

            for (var i = 0; i < length; i++)
            {
                filenameChars[i] = (char)slice[i];
            }

            offset += length;
        }
        
        var timestampChars = new char[length];
        {
            var slice = payload.Slice(offset, length);
            
            for (var i = 0; i < length; i++)
            {
                timestampChars[i] = (char)slice[i];
            }
        }
        
        return new ClientGameClientVersion2Packet(filenameChars, timestampChars);
    }
}