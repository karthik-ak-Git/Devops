namespace DevOpsAIAgent.Core.Models;

public class WebhookConfiguration
{
    public long Id { get; set; }
    public string RepositoryFullName { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
    public string RepoName { get; set; } = string.Empty;
    public long GitHubHookId { get; set; }
    public string WebhookUrl { get; set; } = string.Empty;
    public string? SecretHash { get; set; }
    public bool IsActive { get; set; } = true;
    public List<string> Events { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
