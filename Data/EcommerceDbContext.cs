using Ecommerce.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Data;

public class EcommerceDbContext(DbContextOptions<EcommerceDbContext> options) : DbContext(options)
{
    // below will mimic table in our database
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<User> Users => Set<User>();
    public DbSet<ShoppingCart> ShoppingCarts => Set<ShoppingCart>();

    public override int SaveChanges()
    {
        AuditHandler.UpdateAuditField<Product>(ChangeTracker);
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        AuditHandler.UpdateAuditField<Product>(ChangeTracker);
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>()
        .HasQueryFilter(p => p.DeletedAt == null);

        // seeding the data to db
        // modelBuilder.Entity<VideoGame>().HasData(
        //     new VideoGame
        //     {
        //         Id = 1,
        //         Name = "Test Game",
        //         ReleaseDate = 212
        //     }
        // );

        // modelBuilder.Entity<Product>()
        // .Property(p => p.CreatedAt)
        // // Use a database function for the default value
        // .HasDefaultValueSql("NOW()"); // For PostgreSQL
        // // .HasDefaultValueSql("GETDATE()"); // For SQL Server

        // modelBuilder.Entity<ShoppingCart>()
        // .Property(s => s.CreatedAt)
        // // Use a database function for the default value
        // .HasDefaultValueSql("NOW()"); // For PostgreSQL
        // // .HasDefaultValueSql("GETDATE()"); // For SQL Server
    }
}

