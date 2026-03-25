using System.Text.Json.Serialization;

namespace DevOpsAIAgent.Core.Models;

public class CiCdEvent
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("repositoryName")]
    public string RepositoryName { get; set; } = string.Empty;

    [JsonPropertyName("repositoryUrl")]
    public string RepositoryUrl { get; set; } = string.Empty;

    [JsonPropertyName("commitHash")]
    public string CommitHash { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("conclusion")]
    public string? Conclusion { get; set; }

    [JsonPropertyName("workflowName")]
    public string WorkflowName { get; set; } = string.Empty;

    [JsonPropertyName("workflowRunId")]
    public long? WorkflowRunId { get; set; }

    [JsonPropertyName("runUrl")]
    public string? RunUrl { get; set; }

    [JsonPropertyName("branchName")]
    public string? BranchName { get; set; }

    [JsonPropertyName("triggerActor")]
    public string? TriggerActor { get; set; }

    [JsonPropertyName("receivedAt")]
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;

    [JsonIgnore] // Navigation property - exclude from JSON serialization to avoid circular references
    public AiAnalysis? AiAnalysis { get; set; }

    [JsonPropertyName("isFailure")]
    public bool IsFailure => Conclusion?.Equals("failure", StringComparison.OrdinalIgnoreCase) == true;
}
