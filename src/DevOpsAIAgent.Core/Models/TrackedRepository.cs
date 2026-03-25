using System.Text.Json.Serialization;

namespace DevOpsAIAgent.Core.Models;

public class TrackedRepository
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("fullName")]
    public string FullName { get; set; } = string.Empty;

    [JsonPropertyName("owner")]
    public string Owner { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("htmlUrl")]
    public string? HtmlUrl { get; set; }

    [JsonPropertyName("isPrivate")]
    public bool IsPrivate { get; set; }

    [JsonPropertyName("webhookConfigured")]
    public bool WebhookConfigured { get; set; }

    [JsonPropertyName("totalBuilds")]
    public int TotalBuilds { get; set; }

    [JsonPropertyName("failedBuilds")]
    public int FailedBuilds { get; set; }

    [JsonPropertyName("successRate")]
    public double SuccessRate => TotalBuilds > 0 ? (double)(TotalBuilds - FailedBuilds) / TotalBuilds * 100 : 0;

    [JsonPropertyName("lastBuildAt")]
    public DateTime? LastBuildAt { get; set; }

    [JsonPropertyName("addedAt")]
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
