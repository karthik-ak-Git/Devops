using System.Text.Json.Serialization;

namespace DevOpsAIAgent.Core.Models;

public class UserSettings
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;

    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
