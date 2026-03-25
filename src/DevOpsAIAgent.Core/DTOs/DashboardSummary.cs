using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DevOpsAIAgent.Core.DTOs;

public record DashboardSummary(
    [property: JsonPropertyName("totalPipelines")] int TotalPipelines,
    [property: JsonPropertyName("failedPipelines")] int FailedPipelines,
    [property: JsonPropertyName("activeDeployments")] int ActiveDeployments,
    [property: JsonPropertyName("deploymentSuccessRate")] double DeploymentSuccessRate,
    [property: JsonPropertyName("openIncidents")] int OpenIncidents,
    [property: JsonPropertyName("criticalIncidents")] int CriticalIncidents,
    [property: JsonPropertyName("aiAnalysesPerformed")] int AiAnalysesPerformed,
    [property: JsonPropertyName("lastEventAt")] DateTime? LastEventAt
);

public record PipelineFailureNotification(
    [property: JsonPropertyName("repo")] string Repo,
    [property: JsonPropertyName("repoUrl")] string RepoUrl,
    [property: JsonPropertyName("commitHash")] string CommitHash,
    [property: JsonPropertyName("workflowName")] string WorkflowName,
    [property: JsonPropertyName("runUrl")] string RunUrl,
    [property: JsonPropertyName("runId")] long RunId,
    [property: JsonPropertyName("branchName")] string? BranchName,
    [property: JsonPropertyName("actor")] string? Actor,
    [property: JsonPropertyName("gitDiff")] string GitDiff,
    [property: JsonPropertyName("errorLog")] string ErrorLog,
    [property: JsonPropertyName("aiSuggestion")] string AiSuggestion,
    [property: JsonPropertyName("timestamp")] DateTime Timestamp
);

public record ActivityFeedItem(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("repositoryName")] string? RepositoryName,
    [property: JsonPropertyName("url")] string? Url,
    [property: JsonPropertyName("timestamp")] DateTime Timestamp
);

public record CreateDeploymentRequest
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    [JsonPropertyName("repositoryName")]
    public string RepositoryName { get; init; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 1)]
    [JsonPropertyName("environment")]
    public string Environment { get; init; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 1)]
    [JsonPropertyName("version")]
    public string Version { get; init; } = string.Empty;

    [Required]
    [StringLength(40, MinimumLength = 7)] // Git commit hash length
    [JsonPropertyName("commitHash")]
    public string CommitHash { get; init; } = string.Empty;

    [StringLength(100)]
    [JsonPropertyName("deployedBy")]
    public string? DeployedBy { get; init; }

    [StringLength(1000)]
    [JsonPropertyName("notes")]
    public string? Notes { get; init; }
}

public record UpdateDeploymentStatusRequest
{
    [Required]
    [StringLength(50)]
    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    [StringLength(1000)]
    [JsonPropertyName("notes")]
    public string? Notes { get; init; }
}

public record CreateIncidentRequest
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    [JsonPropertyName("title")]
    public string Title { get; init; } = string.Empty;

    [Required]
    [StringLength(2000, MinimumLength = 1)]
    [JsonPropertyName("description")]
    public string Description { get; init; } = string.Empty;

    [Required]
    [StringLength(20)]
    [JsonPropertyName("severity")]
    public string Severity { get; init; } = string.Empty;

    [StringLength(200)]
    [JsonPropertyName("repositoryName")]
    public string? RepositoryName { get; init; }

    [StringLength(100)]
    [JsonPropertyName("assignedTo")]
    public string? AssignedTo { get; init; }

    [JsonPropertyName("relatedCiCdEventId")]
    public long? RelatedCiCdEventId { get; init; }
}

public record UpdateIncidentRequest
{
    [StringLength(200, MinimumLength = 1)]
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [StringLength(2000, MinimumLength = 1)]
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [StringLength(20)]
    [JsonPropertyName("severity")]
    public string? Severity { get; init; }

    [StringLength(20)]
    [JsonPropertyName("status")]
    public string? Status { get; init; }

    [StringLength(100)]
    [JsonPropertyName("assignedTo")]
    public string? AssignedTo { get; init; }
}
