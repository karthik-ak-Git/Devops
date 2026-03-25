using System.Text.Json.Serialization;

namespace DevOpsAIAgent.Core.DTOs.Responses;

/// <summary>
/// Response DTO for AI Analysis data
/// </summary>
public record AiAnalysisResponse
{
    [JsonPropertyName("id")]
    public long Id { get; init; }

    [JsonPropertyName("ciCdEventId")]
    public long CiCdEventId { get; init; }

    [JsonPropertyName("analysisText")]
    public string AnalysisText { get; init; } = string.Empty;

    [JsonPropertyName("gitDiff")]
    public string? GitDiff { get; init; }

    [JsonPropertyName("errorLog")]
    public string? ErrorLog { get; init; }

    [JsonPropertyName("modelUsed")]
    public string ModelUsed { get; init; } = string.Empty;

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; init; }

    [JsonPropertyName("tokensUsed")]
    public int TokensUsed { get; init; }
}

/// <summary>
/// Response DTO for CI/CD Event data
/// </summary>
public record CiCdEventResponse
{
    [JsonPropertyName("id")]
    public long Id { get; init; }

    [JsonPropertyName("repositoryName")]
    public string RepositoryName { get; init; } = string.Empty;

    [JsonPropertyName("repositoryUrl")]
    public string RepositoryUrl { get; init; } = string.Empty;

    [JsonPropertyName("commitHash")]
    public string CommitHash { get; init; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    [JsonPropertyName("conclusion")]
    public string? Conclusion { get; init; }

    [JsonPropertyName("workflowName")]
    public string WorkflowName { get; init; } = string.Empty;

    [JsonPropertyName("workflowRunId")]
    public long? WorkflowRunId { get; init; }

    [JsonPropertyName("runUrl")]
    public string? RunUrl { get; init; }

    [JsonPropertyName("branchName")]
    public string? BranchName { get; init; }

    [JsonPropertyName("triggerActor")]
    public string? TriggerActor { get; init; }

    [JsonPropertyName("receivedAt")]
    public DateTime ReceivedAt { get; init; }

    [JsonPropertyName("isFailure")]
    public bool IsFailure { get; init; }

    [JsonPropertyName("aiAnalysis")]
    public AiAnalysisResponse? AiAnalysis { get; init; }
}

/// <summary>
/// Response DTO for Deployment data
/// </summary>
public record DeploymentResponse
{
    [JsonPropertyName("id")]
    public long Id { get; init; }

    [JsonPropertyName("repositoryName")]
    public string RepositoryName { get; init; } = string.Empty;

    [JsonPropertyName("environment")]
    public string Environment { get; init; } = string.Empty;

    [JsonPropertyName("version")]
    public string Version { get; init; } = string.Empty;

    [JsonPropertyName("commitHash")]
    public string CommitHash { get; init; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    [JsonPropertyName("deployedBy")]
    public string? DeployedBy { get; init; }

    [JsonPropertyName("notes")]
    public string? Notes { get; init; }

    [JsonPropertyName("startedAt")]
    public DateTime StartedAt { get; init; }

    [JsonPropertyName("completedAt")]
    public DateTime? CompletedAt { get; init; }

    [JsonPropertyName("duration")]
    public TimeSpan? Duration { get; init; }
}

/// <summary>
/// Response DTO for Incident data
/// </summary>
public record IncidentResponse
{
    [JsonPropertyName("id")]
    public long Id { get; init; }

    [JsonPropertyName("title")]
    public string Title { get; init; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; init; } = string.Empty;

    [JsonPropertyName("severity")]
    public string Severity { get; init; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    [JsonPropertyName("repositoryName")]
    public string? RepositoryName { get; init; }

    [JsonPropertyName("assignedTo")]
    public string? AssignedTo { get; init; }

    [JsonPropertyName("aiResolutionSuggestion")]
    public string? AiResolutionSuggestion { get; init; }

    [JsonPropertyName("relatedCiCdEventId")]
    public long? RelatedCiCdEventId { get; init; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; init; }

    [JsonPropertyName("resolvedAt")]
    public DateTime? ResolvedAt { get; init; }

    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; init; }
}

/// <summary>
/// Response DTO for Tracked Repository data
/// </summary>
public record TrackedRepositoryResponse
{
    [JsonPropertyName("id")]
    public long Id { get; init; }

    [JsonPropertyName("repositoryName")]
    public string RepositoryName { get; init; } = string.Empty;

    [JsonPropertyName("repositoryUrl")]
    public string RepositoryUrl { get; init; } = string.Empty;

    [JsonPropertyName("isActive")]
    public bool IsActive { get; init; }

    [JsonPropertyName("lastEventAt")]
    public DateTime? LastEventAt { get; init; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Response DTO for Webhook Configuration data
/// </summary>
public record WebhookConfigurationResponse
{
    [JsonPropertyName("id")]
    public long Id { get; init; }

    [JsonPropertyName("repositoryName")]
    public string RepositoryName { get; init; } = string.Empty;

    [JsonPropertyName("webhookUrl")]
    public string WebhookUrl { get; init; } = string.Empty;

    [JsonPropertyName("secret")]
    public string Secret { get; init; } = string.Empty;

    [JsonPropertyName("isActive")]
    public bool IsActive { get; init; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; init; }

    [JsonPropertyName("lastUsedAt")]
    public DateTime? LastUsedAt { get; init; }
}