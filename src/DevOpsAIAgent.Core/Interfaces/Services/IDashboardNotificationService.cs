using DevOpsAIAgent.Core.DTOs;

namespace DevOpsAIAgent.Core.Interfaces.Services;

/// <summary>
/// Interface for sending real-time dashboard notifications
/// </summary>
public interface IDashboardNotificationService
{
    Task SendPipelineFailureNotificationAsync(PipelineFailureNotification notification);
    Task SendDashboardUpdateAsync(DashboardSummary summary);
    Task SendActivityFeedUpdateAsync(ActivityFeedItem activity);
    Task SendRepositoryNotificationAsync(string repositoryName, object notification);
}