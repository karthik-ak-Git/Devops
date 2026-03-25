using DevOpsAIAgent.Core.DTOs;
using DevOpsAIAgent.Core.DTOs.Common;

namespace DevOpsAIAgent.Core.Interfaces.Services;

/// <summary>
/// Service for managing dashboard data and summaries
/// </summary>
public interface IDashboardService
{
    /// <summary>
    /// Gets the dashboard summary with key metrics
    /// </summary>
    Task<DashboardSummary> GetDashboardSummaryAsync();

    /// <summary>
    /// Gets recent activity feed items
    /// </summary>
    Task<PagedResult<ActivityFeedItem>> GetActivityFeedAsync(PageRequest pageRequest);

    /// <summary>
    /// Gets recent pipeline failure notifications
    /// </summary>
    Task<PagedResult<PipelineFailureNotification>> GetRecentFailuresAsync(PageRequest pageRequest);
}

/// <summary>
/// Service for sending notifications
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Sends a pipeline failure notification
    /// </summary>
    Task SendPipelineFailureNotificationAsync(PipelineFailureNotification notification);

    /// <summary>
    /// Sends an incident notification
    /// </summary>
    Task SendIncidentNotificationAsync(string title, string description, string severity);

    /// <summary>
    /// Sends a deployment notification
    /// </summary>
    Task SendDeploymentNotificationAsync(string repositoryName, string environment, string status);
}

/// <summary>
/// Service for managing deployments
/// </summary>
public interface IDeploymentService
{
    /// <summary>
    /// Creates a new deployment
    /// </summary>
    Task<long> CreateDeploymentAsync(CreateDeploymentRequest request);

    /// <summary>
    /// Updates a deployment status
    /// </summary>
    Task UpdateDeploymentStatusAsync(long id, UpdateDeploymentStatusRequest request);

    /// <summary>
    /// Gets deployments for a repository
    /// </summary>
    Task<PagedResult<Models.Deployment>> GetDeploymentsAsync(string repositoryName, PageRequest pageRequest);

    /// <summary>
    /// Gets active deployments
    /// </summary>
    Task<IReadOnlyList<Models.Deployment>> GetActiveDeploymentsAsync();
}

/// <summary>
/// Service for managing incidents
/// </summary>
public interface IIncidentService
{
    /// <summary>
    /// Creates a new incident
    /// </summary>
    Task<long> CreateIncidentAsync(CreateIncidentRequest request);

    /// <summary>
    /// Updates an incident
    /// </summary>
    Task UpdateIncidentAsync(long id, UpdateIncidentRequest request);

    /// <summary>
    /// Gets incidents with pagination
    /// </summary>
    Task<PagedResult<Models.Incident>> GetIncidentsAsync(PageRequest pageRequest);

    /// <summary>
    /// Gets open incidents
    /// </summary>
    Task<IReadOnlyList<Models.Incident>> GetOpenIncidentsAsync();

    /// <summary>
    /// Gets critical incidents
    /// </summary>
    Task<IReadOnlyList<Models.Incident>> GetCriticalIncidentsAsync();

    /// <summary>
    /// Resolves an incident
    /// </summary>
    Task ResolveIncidentAsync(long id, string? resolution = null);
}