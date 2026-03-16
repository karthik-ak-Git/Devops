using DevOpsAIAgent.Core.DTOs;

namespace DevOpsAIAgent.Core.Interfaces.Services;

public interface IWebhookConfigurationService
{
    Task<IReadOnlyList<RepositorySummary>> GetUserRepositoriesAsync();
    Task<WebhookConfigurationResponse> CreateWebhookAsync(string owner, string repo, string webhookUrl, IList<string>? events = null);
    Task<bool> DeleteWebhookAsync(string owner, string repo, long hookId);
}
