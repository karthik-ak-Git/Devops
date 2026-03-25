using System.Text.Json.Serialization;
using DevOpsAIAgent.Core.Models.Enums;

namespace DevOpsAIAgent.Core.Models;

public class Deployment
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("repositoryName")]
    public string RepositoryName { get; set; } = string.Empty;

    [JsonPropertyName("environment")]
    public string Environment { get; set; } = string.Empty;

    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    [JsonPropertyName("commitHash")]
    public string CommitHash { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public DeploymentStatus Status { get; set; }

    [JsonPropertyName("deployedBy")]
    public string? DeployedBy { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    [JsonPropertyName("startedAt")]
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("completedAt")]
    public DateTime? CompletedAt { get; set; }

    [JsonPropertyName("duration")]
    public TimeSpan? Duration => CompletedAt.HasValue ? CompletedAt - StartedAt : null;
}
