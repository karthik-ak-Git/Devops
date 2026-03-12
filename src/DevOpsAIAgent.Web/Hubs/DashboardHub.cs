using Microsoft.AspNetCore.SignalR;

namespace DevOpsAIAgent.Web.Hubs;

/// <summary>
/// SignalR hub for real-time dashboard updates.
/// Handles broadcasting pipeline events, CI/CD status updates, and other real-time notifications to connected clients.
/// </summary>
public class DashboardHub : Hub
{
    public DashboardHub()
    {
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}
