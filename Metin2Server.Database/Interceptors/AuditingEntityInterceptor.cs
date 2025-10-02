using Metin2Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Metin2Server.Database.Interceptors;

public class AuditingEntityInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context is null)
        {
            return result;
        }

        var entries = eventData.Context.ChangeTracker.Entries<AuditingEntity>().ToList();

        if (entries.Count > 0)
        {
            SetEntityCreated(entries);
            SetEntityUpdated(entries);
            SetEntityDeleted(entries);
        }

        return result;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        if (eventData.Context is null)
        {
            return await ValueTask.FromResult(result);
        }

        var entries = eventData.Context.ChangeTracker.Entries<AuditingEntity>().ToList();

        if (entries.Count > 0)
        {
            SetEntityCreated(entries);
            SetEntityUpdated(entries);
            SetEntityDeleted(entries);
        }

        return await ValueTask.FromResult(result);
    }

    private void SetEntityCreated(IList<EntityEntry<AuditingEntity>> entries)
    {
        var createdEntries = entries.Where(e => e.State == EntityState.Added);

        foreach (var entry in createdEntries)
        {
            entry.Entity.CreatedAt = DateTime.Now;
        }
    }

    private void SetEntityUpdated(IList<EntityEntry<AuditingEntity>> entries)
    {
        var updatedEntries = entries.Where(e => e.State == EntityState.Modified);

        foreach (var entry in updatedEntries)
        {
            entry.Entity.UpdatedAt = DateTime.Now;
        }
    }

    private void SetEntityDeleted(IList<EntityEntry<AuditingEntity>> entries)
    {
        var deletedEntries = entries.Where(e => e.State == EntityState.Deleted);

        foreach (var entry in deletedEntries)
        {
            entry.State = EntityState.Modified;

            entry.Entity.Deleted = true;
            entry.Entity.DeletedAt = DateTime.Now;
        }
    }
}