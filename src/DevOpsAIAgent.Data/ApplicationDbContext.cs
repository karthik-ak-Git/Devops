using DevOpsAIAgent.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DevOpsAIAgent.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<CiCdEvent> CiCdEvents => Set<CiCdEvent>();
    public DbSet<Deployment> Deployments => Set<Deployment>();
    public DbSet<Incident> Incidents => Set<Incident>();
    public DbSet<TrackedRepository> TrackedRepositories => Set<TrackedRepository>();
    public DbSet<AiAnalysis> AiAnalyses => Set<AiAnalysis>();
    public DbSet<WebhookConfiguration> WebhookConfigurations => Set<WebhookConfiguration>();
    public DbSet<UserSettings> UserSettings => Set<UserSettings>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresExtension("vector");

        modelBuilder.Entity<CiCdEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.RepositoryName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.RepositoryUrl).HasMaxLength(500);
            entity.Property(e => e.CommitHash).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Conclusion).HasMaxLength(50);
            entity.Property(e => e.WorkflowName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.RunUrl).HasMaxLength(500);
            entity.Property(e => e.BranchName).HasMaxLength(200);
            entity.Property(e => e.TriggerActor).HasMaxLength(100);
            entity.HasIndex(e => e.RepositoryName);
            entity.HasIndex(e => e.ReceivedAt);
        });

        modelBuilder.Entity<AiAnalysis>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.ModelUsed).HasMaxLength(100);
            entity.Property(e => e.Embedding).HasColumnType("vector(768)");
            entity.HasOne(e => e.CiCdEvent)
                  .WithOne(e => e.AiAnalysis)
                  .HasForeignKey<AiAnalysis>(e => e.CiCdEventId);
        });

        modelBuilder.Entity<Deployment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.RepositoryName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Environment).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Version).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CommitHash).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.DeployedBy).HasMaxLength(100);
            entity.HasIndex(e => e.RepositoryName);
            entity.HasIndex(e => e.StartedAt);
        });

        modelBuilder.Entity<Incident>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Severity).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.RepositoryName).HasMaxLength(200);
            entity.Property(e => e.AssignedTo).HasMaxLength(100);
            entity.HasOne(e => e.RelatedCiCdEvent)
                  .WithMany()
                  .HasForeignKey(e => e.RelatedCiCdEventId)
                  .OnDelete(DeleteBehavior.SetNull);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.Severity);
        });

        modelBuilder.Entity<TrackedRepository>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Owner).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.HtmlUrl).HasMaxLength(500);
            entity.HasIndex(e => e.FullName).IsUnique();
        });

        modelBuilder.Entity<WebhookConfiguration>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.RepositoryFullName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Owner).IsRequired().HasMaxLength(100);
            entity.Property(e => e.RepoName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.WebhookUrl).IsRequired().HasMaxLength(500);
            entity.Property(e => e.SecretHash).HasMaxLength(500);
            entity.Property(e => e.Events).HasColumnType("text[]");
            entity.HasIndex(e => e.RepositoryFullName).IsUnique();
        });

        modelBuilder.Entity<UserSettings>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.Key).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Value).IsRequired();
            entity.HasIndex(e => e.Key).IsUnique();
        });
    }
}
