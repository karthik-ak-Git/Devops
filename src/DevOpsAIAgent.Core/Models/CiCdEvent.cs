namespace DevOpsAIAgent.Core.Models;

public class CiCdEvent
{
    public long Id { get; set; }
    public string RepositoryName { get; set; } = string.Empty;
    public string RepositoryUrl { get; set; } = string.Empty;
    public string CommitHash { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Conclusion { get; set; }
    public string WorkflowName { get; set; } = string.Empty;
    public long? WorkflowRunId { get; set; }
    public string? RunUrl { get; set; }
    public string? BranchName { get; set; }
    public string? TriggerActor { get; set; }
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;

    public AiAnalysis? AiAnalysis { get; set; }

    public bool IsFailure => Conclusion?.Equals("failure", StringComparison.OrdinalIgnoreCase) == true;
}
