using Microsoft.EntityFrameworkCore;
using Tareas.Api.Domain.Entities;

namespace Tareas.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Sector> Sectors => Set<Sector>();
    public DbSet<User> Users => Set<User>();

    public DbSet<TaskTemplate> TaskTemplates => Set<TaskTemplate>();
    public DbSet<TaskInstance> TaskInstances => Set<TaskInstance>();
    public DbSet<TaskDependency> TaskDependencies => Set<TaskDependency>();

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

        // TaskTemplate
        modelBuilder.Entity<TaskTemplate>(e =>
        {
            e.HasIndex(x => x.Name);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.DefaultEstimatedHours).HasPrecision(9, 2);

            e.HasOne(x => x.Sector)
             .WithMany()
             .HasForeignKey(x => x.SectorId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // TaskInstance
        modelBuilder.Entity<TaskInstance>(e =>
        {
            e.HasIndex(x => x.DueDateUtc);
            e.Property(x => x.Title).HasMaxLength(250).IsRequired();
            e.Property(x => x.EstimatedHours).HasPrecision(9, 2);
            e.Property(x => x.Link).HasMaxLength(1000);
            e.Property(x => x.Reason).HasMaxLength(2000);
            e.Property(x => x.Comments).HasMaxLength(4000);

            e.HasOne(x => x.TaskTemplate)
             .WithMany()
             .HasForeignKey(x => x.TaskTemplateId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Client)
             .WithMany()
             .HasForeignKey(x => x.ClientId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Sector)
             .WithMany()
             .HasForeignKey(x => x.SectorId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // TaskDependency (self-referencing)
        modelBuilder.Entity<TaskDependency>(e =>
        {
            e.HasIndex(x => new { x.TaskId, x.DependsOnTaskId }).IsUnique();

            e.HasOne(x => x.Task)
             .WithMany()
             .HasForeignKey(x => x.TaskId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.DependsOnTask)
             .WithMany()
             .HasForeignKey(x => x.DependsOnTaskId)
             .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
