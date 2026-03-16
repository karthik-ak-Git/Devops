using DevOpsAIAgent.Core.Models.Enums;

namespace DevOpsAIAgent.Core.Models;

public class Deployment
{
    public long Id { get; set; }
    public string RepositoryName { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string CommitHash { get; set; } = string.Empty;
    public DeploymentStatus Status { get; set; }
    public string? DeployedBy { get; set; }
    public string? Notes { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public TimeSpan? Duration => CompletedAt.HasValue ? CompletedAt - StartedAt : null;
}
