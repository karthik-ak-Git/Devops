using Microsoft.AspNetCore.SignalR;
using DevOpsAIAgent.Core.DTOs;

namespace DevOpsAIAgent.API.Hubs;

/// <summary>
/// SignalR hub for real-time dashboard updates
/// </summary>
public class DashboardHub : Hub
{
    private readonly ILogger<DashboardHub> _logger;

    public DashboardHub(ILogger<DashboardHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Called when a client connects to the hub
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client {ConnectionId} connected to dashboard hub", Context.ConnectionId);
        await Groups.AddToGroupAsync(Context.ConnectionId, "Dashboard");
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Called when a client disconnects from the hub
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client {ConnectionId} disconnected from dashboard hub", Context.ConnectionId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Dashboard");
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Join a specific repository group for targeted notifications
    /// </summary>
    /// <param name="repositoryName">Repository name to subscribe to</param>
    public async Task JoinRepositoryGroup(string repositoryName)
    {
        var groupName = $"Repository_{repositoryName}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation("Client {ConnectionId} joined repository group {GroupName}",
            Context.ConnectionId, groupName);
    }

    /// <summary>
    /// Leave a specific repository group
    /// </summary>
    /// <param name="repositoryName">Repository name to unsubscribe from</param>
    public async Task LeaveRepositoryGroup(string repositoryName)
    {
        var groupName = $"Repository_{repositoryName}";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation("Client {ConnectionId} left repository group {GroupName}",
            Context.ConnectionId, groupName);
    }
}