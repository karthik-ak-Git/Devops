using DevOpsAIAgent.Core.DTOs.Responses;
using DevOpsAIAgent.Core.Models;

namespace DevOpsAIAgent.Core.Extensions;

/// <summary>
/// Extension methods for mapping domain models to response DTOs
/// </summary>
public static class ModelMappingExtensions
{
    /// <summary>
    /// Maps AiAnalysis domain model to response DTO
    /// </summary>
    public static AiAnalysisResponse ToResponse(this AiAnalysis analysis) => new()
    {
        Id = analysis.Id,
        CiCdEventId = analysis.CiCdEventId,
        AnalysisText = analysis.AnalysisText,
        GitDiff = analysis.GitDiff,
        ErrorLog = analysis.ErrorLog,
        ModelUsed = analysis.ModelUsed,
        CreatedAt = analysis.CreatedAt,
        TokensUsed = analysis.TokensUsed
    };

    /// <summary>
    /// Maps CiCdEvent domain model to response DTO
    /// </summary>
    public static CiCdEventResponse ToResponse(this CiCdEvent ciCdEvent) => new()
    {
        Id = ciCdEvent.Id,
        RepositoryName = ciCdEvent.RepositoryName,
        RepositoryUrl = ciCdEvent.RepositoryUrl,
        CommitHash = ciCdEvent.CommitHash,
        Status = ciCdEvent.Status,
        Conclusion = ciCdEvent.Conclusion,
        WorkflowName = ciCdEvent.WorkflowName,
        WorkflowRunId = ciCdEvent.WorkflowRunId,
        RunUrl = ciCdEvent.RunUrl,
        BranchName = ciCdEvent.BranchName,
        TriggerActor = ciCdEvent.TriggerActor,
        ReceivedAt = ciCdEvent.ReceivedAt,
        IsFailure = ciCdEvent.IsFailure,
        AiAnalysis = ciCdEvent.AiAnalysis?.ToResponse()
    };

    /// <summary>
    /// Maps Deployment domain model to response DTO
    /// </summary>
    public static DeploymentResponse ToResponse(this Deployment deployment) => new()
    {
        Id = deployment.Id,
        RepositoryName = deployment.RepositoryName,
        Environment = deployment.Environment,
        Version = deployment.Version,
        CommitHash = deployment.CommitHash,
        Status = deployment.Status.ToString(),
        DeployedBy = deployment.DeployedBy,
        Notes = deployment.Notes,
        StartedAt = deployment.StartedAt,
        CompletedAt = deployment.CompletedAt,
        Duration = deployment.Duration
    };

    /// <summary>
    /// Maps Incident domain model to response DTO
    /// </summary>
    public static IncidentResponse ToResponse(this Incident incident) => new()
    {
        Id = incident.Id,
        Title = incident.Title,
        Description = incident.Description,
        Severity = incident.Severity.ToString(),
        Status = incident.Status.ToString(),
        RepositoryName = incident.RepositoryName,
        AssignedTo = incident.AssignedTo,
        AiResolutionSuggestion = incident.AiResolutionSuggestion,
        RelatedCiCdEventId = incident.RelatedCiCdEventId,
        CreatedAt = incident.CreatedAt,
        ResolvedAt = incident.ResolvedAt,
        UpdatedAt = incident.UpdatedAt
    };

    /// <summary>
    /// Maps TrackedRepository domain model to response DTO
    /// </summary>
    public static TrackedRepositoryResponse ToResponse(this TrackedRepository repository) => new()
    {
        Id = repository.Id,
        RepositoryName = repository.FullName,
        RepositoryUrl = repository.HtmlUrl ?? string.Empty,
        IsActive = repository.WebhookConfigured,
        LastEventAt = repository.LastBuildAt,
        CreatedAt = repository.AddedAt
    };

    /// <summary>
    /// Maps WebhookConfiguration domain model to response DTO
    /// </summary>
    public static WebhookConfigurationResponse ToResponse(this WebhookConfiguration webhook) => new()
    {
        Id = webhook.Id,
        RepositoryName = webhook.RepositoryFullName,
        WebhookUrl = webhook.WebhookUrl,
        Secret = webhook.SecretHash ?? string.Empty,
        IsActive = webhook.IsActive,
        CreatedAt = webhook.CreatedAt,
        LastUsedAt = webhook.UpdatedAt
    };

    /// <summary>
    /// Maps a collection of domain models to response DTOs
    /// </summary>
    public static IEnumerable<TResponse> ToResponse<TModel, TResponse>(
        this IEnumerable<TModel> models,
        Func<TModel, TResponse> mapper) =>
        models.Select(mapper);

    /// <summary>
    /// Maps a collection of AiAnalysis models to response DTOs
    /// </summary>
    public static IEnumerable<AiAnalysisResponse> ToResponse(this IEnumerable<AiAnalysis> analyses) =>
        analyses.Select(a => a.ToResponse());

    /// <summary>
    /// Maps a collection of CiCdEvent models to response DTOs
    /// </summary>
    public static IEnumerable<CiCdEventResponse> ToResponse(this IEnumerable<CiCdEvent> events) =>
        events.Select(e => e.ToResponse());

    /// <summary>
    /// Maps a collection of Deployment models to response DTOs
    /// </summary>
    public static IEnumerable<DeploymentResponse> ToResponse(this IEnumerable<Deployment> deployments) =>
        deployments.Select(d => d.ToResponse());

    /// <summary>
    /// Maps a collection of Incident models to response DTOs
    /// </summary>
    public static IEnumerable<IncidentResponse> ToResponse(this IEnumerable<Incident> incidents) =>
        incidents.Select(i => i.ToResponse());

    /// <summary>
    /// Maps a collection of TrackedRepository models to response DTOs
    /// </summary>
    public static IEnumerable<TrackedRepositoryResponse> ToResponse(this IEnumerable<TrackedRepository> repositories) =>
        repositories.Select(r => r.ToResponse());

    /// <summary>
    /// Maps a collection of WebhookConfiguration models to response DTOs
    /// </summary>
    public static IEnumerable<WebhookConfigurationResponse> ToResponse(this IEnumerable<WebhookConfiguration> webhooks) =>
        webhooks.Select(w => w.ToResponse());
}