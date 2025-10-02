namespace Metin2Server.Domain.Repositories;

public interface IUnitOfWork
{
    int SaveChanges();
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}