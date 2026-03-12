namespace DevOpsAIAgent.Core.Services;

public interface IWebhookListenerService : IDisposable
{
    Task StartAsync(CancellationToken cancellationToken = default);
    Task StopAsync(CancellationToken cancellationToken = default);
    bool IsRunning { get; }
}
