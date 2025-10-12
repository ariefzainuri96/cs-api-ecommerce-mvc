using Ecommerce.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Ecommerce.Data;

public static class AuditHandler
{
    public static void UpdateAuditField<T>(ChangeTracker changeTracker) where T : class, IAuditable
    {
        var entries = changeTracker.Entries<T>().Where(e => e.Entity is not null &&
                        (e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted));

        foreach (var entry in entries)
        {
            var entity = entry.Entity;

            if (entry.State == EntityState.Added)
                entity.CreatedAt = DateTimeOffset.UtcNow;

            if (entry.State == EntityState.Deleted)
            {
                entity.DeletedAt = DateTimeOffset.UtcNow;
                entry.State = EntityState.Modified;
            }

            if (entry.State == EntityState.Modified)
                entity.UpdatedAt = DateTimeOffset.UtcNow;
        }
    }
}
