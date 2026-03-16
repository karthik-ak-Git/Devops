namespace DevOpsAIAgent.Core.DTOs;

public record DashboardSummary(
    int TotalPipelines,
    int FailedPipelines,
    int ActiveDeployments,
    double DeploymentSuccessRate,
    int OpenIncidents,
    int CriticalIncidents,
    int AiAnalysesPerformed,
    DateTime? LastEventAt
);

public record PipelineFailureNotification(
    string Repo,
    string RepoUrl,
    string CommitHash,
    string WorkflowName,
    string RunUrl,
    long RunId,
    string? BranchName,
    string? Actor,
    string GitDiff,
    string ErrorLog,
    string AiSuggestion,
    DateTime Timestamp
);

public record ActivityFeedItem(
    string Type,
    string Title,
    string Description,
    string? RepositoryName,
    string? Url,
    DateTime Timestamp
);

public record CreateDeploymentRequest(
    string RepositoryName,
    string Environment,
    string Version,
    string CommitHash,
    string? DeployedBy,
    string? Notes
);

public record UpdateDeploymentStatusRequest(
    string Status,
    string? Notes
);

public record CreateIncidentRequest(
    string Title,
    string Description,
    string Severity,
    string? RepositoryName,
    string? AssignedTo,
    long? RelatedCiCdEventId
);

public record UpdateIncidentRequest(
    string? Title,
    string? Description,
    string? Severity,
    string? Status,
    string? AssignedTo
);
