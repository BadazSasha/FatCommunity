using Microsoft.EntityFrameworkCore;
using FatCommunity.Models;

namespace FatCommunity.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Restaurant> Restaurants => Set<Restaurant>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<RestaurantReview> RestaurantReviews => Set<RestaurantReview>();
    public DbSet<MenuItemReview> MenuItemReviews => Set<MenuItemReview>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Restaurant -> MenuItems (1-to-many)
        modelBuilder.Entity<MenuItem>()
            .HasOne(m => m.Restaurant)
            .WithMany(r => r.MenuItems)
            .HasForeignKey(m => m.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        // Restaurant -> RestaurantReviews (1-to-many)
        modelBuilder.Entity<RestaurantReview>()
            .HasOne(r => r.Restaurant)
            .WithMany(r => r.Reviews)
            .HasForeignKey(r => r.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        // MenuItem -> MenuItemReviews (1-to-many)
        modelBuilder.Entity<MenuItemReview>()
            .HasOne(r => r.MenuItem)
            .WithMany(m => m.Reviews)
            .HasForeignKey(r => r.MenuItemId)
            .OnDelete(DeleteBehavior.Cascade);

        // Helpful indexes (faster lookup)
        modelBuilder.Entity<Restaurant>()
            .HasIndex(r => new { r.City, r.Name });

        modelBuilder.Entity<MenuItem>()
            .HasIndex(m => new { m.RestaurantId, m.Name });
    }

    public override int SaveChanges()
    {
        TouchUpdatedTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        TouchUpdatedTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void TouchUpdatedTimestamps()
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is Restaurant r)
            {
                if (entry.State == EntityState.Modified) r.UpdatedAtUtc = utcNow;
                if (entry.State == EntityState.Added) { r.CreatedAtUtc = utcNow; r.UpdatedAtUtc = utcNow; }
            }

            if (entry.Entity is MenuItem m)
            {
                if (entry.State == EntityState.Modified) m.UpdatedAtUtc = utcNow;
                if (entry.State == EntityState.Added) { m.CreatedAtUtc = utcNow; m.UpdatedAtUtc = utcNow; }
            }
        }
    }
}
