using Metin2Server.Database.Domain.Entities;

namespace Metin2Server.Database.Domain.Repositories;

public interface IChannelInformationRepository
{
    Task UpsertAsync(ChannelInformation channelInformation, CancellationToken cancellationToken);
    
    Task<IEnumerable<ChannelInformation>> GetAllAsync(CancellationToken cancellationToken);
    
    Task RemoveAsync(ushort port, CancellationToken cancellationToken);
}