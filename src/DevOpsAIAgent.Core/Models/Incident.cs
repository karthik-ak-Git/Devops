using DevOpsAIAgent.Core.Models.Enums;

namespace DevOpsAIAgent.Core.Models;

public class Incident
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IncidentSeverity Severity { get; set; }
    public IncidentStatus Status { get; set; }
    public string? RepositoryName { get; set; }
    public string? AssignedTo { get; set; }
    public string? AiResolutionSuggestion { get; set; }
    public long? RelatedCiCdEventId { get; set; }
    public CiCdEvent? RelatedCiCdEvent { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
