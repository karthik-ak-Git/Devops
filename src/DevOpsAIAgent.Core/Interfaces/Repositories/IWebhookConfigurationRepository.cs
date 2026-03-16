using DevOpsAIAgent.Core.Models;

namespace DevOpsAIAgent.Core.Interfaces.Repositories;

public interface IWebhookConfigurationRepository
{
    Task<WebhookConfiguration?> GetByRepositoryAsync(string repositoryFullName);
    Task<IReadOnlyList<WebhookConfiguration>> GetAllAsync();
    Task<WebhookConfiguration> AddAsync(WebhookConfiguration entity);
    Task UpdateAsync(WebhookConfiguration entity);
    Task DeleteAsync(long id);
}
