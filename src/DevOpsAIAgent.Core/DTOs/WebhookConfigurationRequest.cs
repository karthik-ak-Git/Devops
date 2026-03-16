namespace DevOpsAIAgent.Core.DTOs;

public class WebhookConfigurationRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
    public string Repo { get; set; } = string.Empty;
    public string WebhookUrl { get; set; } = string.Empty;
    public List<string>? Events { get; set; }
}

public record WebhookConfigurationResponse(
    bool Success,
    string Message,
    long? WebhookId = null,
    string? WebhookUrl = null
);

public record RepositorySummary(
    string FullName,
    string Owner,
    string Name,
    string HtmlUrl,
    bool IsPrivate,
    bool HasWebhook,
    string Description
);
