namespace Metin2Server.Database.Domain.Repositories;

public interface IUnitOfWork
{
    int SaveChanges();
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}