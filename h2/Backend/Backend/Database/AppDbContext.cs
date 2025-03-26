using Backend.Entities;
using Backend.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.ChangeTracking;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Favorite> Favorites => Set<Favorite>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var platformsConverter = new ValueConverter<List<Platforms>, string>(
            v => string.Join(",", v.Select(p => p.ToString())),
            v => v.Split(",", StringSplitOptions.RemoveEmptyEntries)
                  .Select(p => Enum.Parse<Platforms>(p)).ToList()
        );

        var platformsComparer = new ValueComparer<List<Platforms>>(
            (a, b) => a.SequenceEqual(b),
            v => v.Aggregate(0, (acc, x) => HashCode.Combine(acc, x.GetHashCode())),
            v => v.ToList()
        );

        modelBuilder.Entity<User>()
            .Property(u => u.Platforms)
            .HasConversion(platformsConverter)
            .Metadata.SetValueComparer(platformsComparer);

    }
}
