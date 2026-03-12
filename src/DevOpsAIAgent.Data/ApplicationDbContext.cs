using Microsoft.EntityFrameworkCore;
using DevOpsAIAgent.Core.Models;

namespace DevOpsAIAgent.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<CiCdEvent> CiCdEvents { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CiCdEvent>(entity =>
        {
            entity.HasKey(e => new { e.RepositoryName, e.CommitHash, e.ReceivedAt });
            entity.Property(e => e.RepositoryName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.RepositoryUrl).IsRequired().HasMaxLength(500);
            entity.Property(e => e.CommitHash).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Conclusion).HasMaxLength(50);
            entity.Property(e => e.WorkflowName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ReceivedAt).IsRequired();
        });
    }
}
