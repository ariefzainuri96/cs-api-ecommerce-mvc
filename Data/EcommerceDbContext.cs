using Ecommerce.Model;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Data;

public class EcommerceDbContext(DbContextOptions<EcommerceDbContext> options) : DbContext(options)
{
    // below will mimic table in our database
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<User> Users => Set<User>();
    public DbSet<ShoppingCart> ShoppingCarts => Set<ShoppingCart>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // seeding the data to db
        // modelBuilder.Entity<VideoGame>().HasData(
        //     new VideoGame
        //     {
        //         Id = 1,
        //         Name = "Test Game",
        //         ReleaseDate = 212
        //     }
        // );
    }
}

