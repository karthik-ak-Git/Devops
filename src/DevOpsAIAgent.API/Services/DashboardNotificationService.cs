using Microsoft.AspNetCore.SignalR;
using DevOpsAIAgent.Core.DTOs;
using DevOpsAIAgent.Core.Interfaces.Services;
using DevOpsAIAgent.API.Hubs;

namespace DevOpsAIAgent.API.Services;

/// <summary>
/// Service for sending real-time notifications through SignalR
/// </summary>
public class DashboardNotificationService : IDashboardNotificationService
{
    private readonly IHubContext<DashboardHub> _hubContext;
    private readonly ILogger<DashboardNotificationService> _logger;

    public DashboardNotificationService(
        IHubContext<DashboardHub> hubContext,
        ILogger<DashboardNotificationService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    /// <summary>
    /// Send pipeline failure notification to all connected clients
    /// </summary>
    /// <param name="notification">Pipeline failure notification</param>
    public async Task SendPipelineFailureNotificationAsync(PipelineFailureNotification notification)
    {
        try
        {
            _logger.LogInformation("Sending pipeline failure notification for {Repo}", notification.Repo);

            await _hubContext.Clients.Group("Dashboard")
                .SendAsync("PipelineFailure", notification);

            // Also send to repository-specific group
            var repositoryGroup = $"Repository_{notification.Repo}";
            await _hubContext.Clients.Group(repositoryGroup)
                .SendAsync("PipelineFailure", notification);

            _logger.LogDebug("Pipeline failure notification sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending pipeline failure notification for {Repo}", notification.Repo);
        }
    }

    /// <summary>
    /// Send dashboard summary update to all connected clients
    /// </summary>
    /// <param name="summary">Dashboard summary data</param>
    public async Task SendDashboardUpdateAsync(DashboardSummary summary)
    {
        try
        {
            _logger.LogDebug("Sending dashboard update");

            await _hubContext.Clients.Group("Dashboard")
                .SendAsync("DashboardUpdate", summary);

            _logger.LogDebug("Dashboard update sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending dashboard update");
        }
    }

    /// <summary>
    /// Send activity feed update to all connected clients
    /// </summary>
    /// <param name="activity">Activity feed item</param>
    public async Task SendActivityFeedUpdateAsync(ActivityFeedItem activity)
    {
        try
        {
            _logger.LogDebug("Sending activity feed update: {Type} - {Title}", activity.Type, activity.Title);

            await _hubContext.Clients.Group("Dashboard")
                .SendAsync("ActivityFeedUpdate", activity);

            // Also send to repository-specific group if applicable
            if (!string.IsNullOrEmpty(activity.RepositoryName))
            {
                var repositoryGroup = $"Repository_{activity.RepositoryName}";
                await _hubContext.Clients.Group(repositoryGroup)
                    .SendAsync("ActivityFeedUpdate", activity);
            }

            _logger.LogDebug("Activity feed update sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending activity feed update");
        }
    }

    /// <summary>
    /// Send notification to clients subscribed to a specific repository
    /// </summary>
    /// <param name="repositoryName">Repository name</param>
    /// <param name="notification">Notification object</param>
    public async Task SendRepositoryNotificationAsync(string repositoryName, object notification)
    {
        try
        {
            _logger.LogDebug("Sending repository notification for {Repository}", repositoryName);

            var repositoryGroup = $"Repository_{repositoryName}";
            await _hubContext.Clients.Group(repositoryGroup)
                .SendAsync("RepositoryNotification", notification);

            _logger.LogDebug("Repository notification sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending repository notification for {Repository}", repositoryName);
        }
    }
}