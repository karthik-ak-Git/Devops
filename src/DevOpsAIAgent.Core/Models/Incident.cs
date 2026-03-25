using System.Text.Json.Serialization;
using DevOpsAIAgent.Core.Models.Enums;

namespace DevOpsAIAgent.Core.Models;

public class Incident
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("severity")]
    public IncidentSeverity Severity { get; set; }

    [JsonPropertyName("status")]
    public IncidentStatus Status { get; set; }

    [JsonPropertyName("repositoryName")]
    public string? RepositoryName { get; set; }

    [JsonPropertyName("assignedTo")]
    public string? AssignedTo { get; set; }

    [JsonPropertyName("aiResolutionSuggestion")]
    public string? AiResolutionSuggestion { get; set; }

    [JsonPropertyName("relatedCiCdEventId")]
    public long? RelatedCiCdEventId { get; set; }

    [JsonIgnore] // Navigation property - exclude from JSON serialization to avoid circular references
    public CiCdEvent? RelatedCiCdEvent { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("resolvedAt")]
    public DateTime? ResolvedAt { get; set; }

    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
