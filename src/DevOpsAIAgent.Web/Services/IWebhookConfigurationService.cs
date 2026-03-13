using Octokit;

namespace DevOpsAIAgent.Web.Services;

/// <summary>
/// Service for managing GitHub webhooks.
/// </summary>
public interface IWebhookConfigurationService
{
    /// <summary>
    /// Gets all repositories the authenticated user has admin access to.
    /// </summary>
    Task<IReadOnlyList<RepositorySummary>> GetUserRepositoriesAsync();

    /// <summary>
    /// Creates a webhook for the specified repository.
    /// </summary>
    /// <param name="owner">Repository owner.</param>
    /// <param name="repo">Repository name.</param>
    /// <param name="webhookUrl">The webhook callback URL.</param>
    /// <returns>The created webhook details.</returns>
    Task<WebhookConfigurationResult> CreateWebhookAsync(string owner, string repo, string webhookUrl);

    /// <summary>
    /// Lists all webhooks for the specified repository.
    /// </summary>
    Task<IReadOnlyList<RepositoryHook>> ListWebhooksAsync(string owner, string repo);

    /// <summary>
    /// Deletes a webhook from the specified repository.
    /// </summary>
    Task<bool> DeleteWebhookAsync(string owner, string repo, long hookId);
}

/// <summary>
/// Summary of a repository with webhook configuration status.
/// </summary>
public record RepositorySummary(
    string FullName,
    string Owner,
    string Name,
    string HtmlUrl,
    bool IsPrivate,
    bool HasWebhook,
    string Description
);

/// <summary>
/// Result of webhook configuration operation.
/// </summary>
public record WebhookConfigurationResult(
    bool Success,
    string Message,
    long? WebhookId = null,
    string? WebhookUrl = null
);
