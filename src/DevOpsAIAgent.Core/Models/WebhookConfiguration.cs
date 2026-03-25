using System.Text.Json.Serialization;

namespace DevOpsAIAgent.Core.Models;

public class WebhookConfiguration
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("repositoryFullName")]
    public string RepositoryFullName { get; set; } = string.Empty;

    [JsonPropertyName("owner")]
    public string Owner { get; set; } = string.Empty;

    [JsonPropertyName("repoName")]
    public string RepoName { get; set; } = string.Empty;

    [JsonPropertyName("gitHubHookId")]
    public long GitHubHookId { get; set; }

    [JsonPropertyName("webhookUrl")]
    public string WebhookUrl { get; set; } = string.Empty;

    [JsonPropertyName("secretHash")]
    public string? SecretHash { get; set; }

    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; } = true;

    [JsonPropertyName("events")]
    public List<string> Events { get; set; } = new();

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
