using Microsoft.EntityFrameworkCore;
using Tareas.Api.Domain.Entities;

namespace Tareas.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Sector> Sectors => Set<Sector>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Client
        modelBuilder.Entity<Client>(e =>
        {
            e.HasIndex(x => x.Code).IsUnique(false);
            e.HasIndex(x => x.Name);
            e.Property(x => x.Code).HasMaxLength(50);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
        });

        // Sector
        modelBuilder.Entity<Sector>(e =>
        {
            e.HasIndex(x => x.Name);
            e.Property(x => x.Name).HasMaxLength(150).IsRequired();
        });

        // User
        modelBuilder.Entity<User>(e =>
        {
            e.HasIndex(x => x.Email).IsUnique(false);
            e.Property(x => x.DisplayName).HasMaxLength(200).IsRequired();
            e.Property(x => x.Email).HasMaxLength(200).IsRequired();

            e.HasOne(x => x.PrimarySector)
             .WithMany()
             .HasForeignKey(x => x.PrimarySectorId)
             .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
