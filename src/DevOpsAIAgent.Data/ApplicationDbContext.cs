using DevOpsAIAgent.Core.Models;
using Microsoft.EntityFrameworkCore;
using Pgvector;

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

        // Enable pgvector extension for PostgreSQL
        if (Database.IsNpgsql())
        {
            modelBuilder.HasPostgresExtension("vector");
        }

        // Configure CiCdEvent entity
        modelBuilder.Entity<CiCdEvent>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Primary key configuration for dual database support
            if (Database.IsNpgsql())
            {
                entity.Property(e => e.Id)
                    .UseIdentityByDefaultColumn()
                    .HasColumnType("bigint");
            }
            else
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();
            }

            entity.Property(e => e.RepositoryName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.RepositoryUrl).HasMaxLength(500);
            entity.Property(e => e.CommitHash).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Conclusion).HasMaxLength(50);
            entity.Property(e => e.WorkflowName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.RunUrl).HasMaxLength(500);
            entity.Property(e => e.BranchName).HasMaxLength(200);
            entity.Property(e => e.TriggerActor).HasMaxLength(100);

            // Performance indexes
            entity.HasIndex(e => e.RepositoryName);
            entity.HasIndex(e => e.ReceivedAt);
            entity.HasIndex(e => new { e.RepositoryName, e.ReceivedAt });
            entity.HasIndex(e => e.Conclusion);
        });

        // Configure AiAnalysis entity
        modelBuilder.Entity<AiAnalysis>(entity =>
        {
            entity.HasKey(e => e.Id);

            if (Database.IsNpgsql())
            {
                entity.Property(e => e.Id)
                    .UseIdentityByDefaultColumn()
                    .HasColumnType("bigint");

                // PostgreSQL vector configuration
                entity.Property(e => e.Embedding)
                    .HasColumnType("vector(1536)");  // Standard OpenAI embedding dimension
            }
            else
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                // SQLite - store as JSON text
                entity.Property(e => e.Embedding)
                    .HasColumnType("TEXT");
            }

            entity.Property(e => e.ModelUsed).HasMaxLength(100);
            entity.Property(e => e.AnalysisText).IsRequired();

            // Foreign key relationship
            entity.HasOne(e => e.CiCdEvent)
                  .WithOne(e => e.AiAnalysis)
                  .HasForeignKey<AiAnalysis>(e => e.CiCdEventId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Performance indexes
            entity.HasIndex(e => e.CiCdEventId).IsUnique();
            entity.HasIndex(e => e.CreatedAt);
        });

        // Configure Deployment entity
        modelBuilder.Entity<Deployment>(entity =>
        {
            entity.HasKey(e => e.Id);

            if (Database.IsNpgsql())
            {
                entity.Property(e => e.Id)
                    .UseIdentityByDefaultColumn()
                    .HasColumnType("bigint");
            }
            else
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();
            }

            entity.Property(e => e.RepositoryName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Environment).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Version).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CommitHash).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(20);
            entity.Property(e => e.DeployedBy).HasMaxLength(100);
            entity.Property(e => e.Notes).HasMaxLength(2000);

            // Performance indexes
            entity.HasIndex(e => e.RepositoryName);
            entity.HasIndex(e => e.StartedAt);
            entity.HasIndex(e => new { e.Environment, e.Status });
        });

        // Configure Incident entity
        modelBuilder.Entity<Incident>(entity =>
        {
            entity.HasKey(e => e.Id);

            if (Database.IsNpgsql())
            {
                entity.Property(e => e.Id)
                    .UseIdentityByDefaultColumn()
                    .HasColumnType("bigint");
            }
            else
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();
            }

            entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Description).HasMaxLength(5000);
            entity.Property(e => e.Severity)
                .HasConversion<string>()
                .HasMaxLength(20);
            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(20);
            entity.Property(e => e.RepositoryName).HasMaxLength(200);
            entity.Property(e => e.AssignedTo).HasMaxLength(100);

            // Foreign key relationship
            entity.HasOne(e => e.RelatedCiCdEvent)
                  .WithMany()
                  .HasForeignKey(e => e.RelatedCiCdEventId)
                  .OnDelete(DeleteBehavior.SetNull);

            // Performance indexes
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.Severity);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => new { e.Status, e.Severity });
        });

        // Configure TrackedRepository entity
        modelBuilder.Entity<TrackedRepository>(entity =>
        {
            entity.HasKey(e => e.Id);

            if (Database.IsNpgsql())
            {
                entity.Property(e => e.Id)
                    .UseIdentityByDefaultColumn()
                    .HasColumnType("bigint");
            }
            else
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();
            }

            entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Owner).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.HtmlUrl).HasMaxLength(500);

            // Unique constraint
            entity.HasIndex(e => e.FullName).IsUnique();

            // Performance indexes
            entity.HasIndex(e => e.Owner);
            entity.HasIndex(e => e.WebhookConfigured);
            entity.HasIndex(e => e.LastBuildAt);
        });

        // Configure WebhookConfiguration entity
        modelBuilder.Entity<WebhookConfiguration>(entity =>
        {
            entity.HasKey(e => e.Id);

            if (Database.IsNpgsql())
            {
                entity.Property(e => e.Id)
                    .UseIdentityByDefaultColumn()
                    .HasColumnType("bigint");

                // PostgreSQL - use jsonb for events
                entity.Property(e => e.Events)
                    .HasColumnType("jsonb");
            }
            else
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                // SQLite - store as TEXT
                entity.Property(e => e.Events)
                    .HasColumnType("TEXT");
            }

            entity.Property(e => e.RepositoryFullName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Owner).IsRequired().HasMaxLength(100);
            entity.Property(e => e.RepoName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.WebhookUrl).IsRequired().HasMaxLength(500);
            entity.Property(e => e.SecretHash).HasMaxLength(500);

            // Unique constraint
            entity.HasIndex(e => e.RepositoryFullName).IsUnique();

            // Performance indexes
            entity.HasIndex(e => e.Owner);
            entity.HasIndex(e => e.IsActive);
        });

        // Configure UserSettings entity
        modelBuilder.Entity<UserSettings>(entity =>
        {
            entity.HasKey(e => e.Id);

            if (Database.IsNpgsql())
            {
                entity.Property(e => e.Id)
                    .UseIdentityByDefaultColumn()
                    .HasColumnType("bigint");
            }
            else
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();
            }

            entity.Property(e => e.Key).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Value).IsRequired().HasMaxLength(5000);

            // Unique constraint on key
            entity.HasIndex(e => e.Key).IsUnique();

            // Performance index
            entity.HasIndex(e => e.UpdatedAt);
        });
    }
}
