using System.Text.Json.Serialization;
using DevOpsAIAgent.Core.Converters;
using Pgvector;

namespace DevOpsAIAgent.Core.Models;

public class AiAnalysis
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("ciCdEventId")]
    public long CiCdEventId { get; set; }

    [JsonIgnore] // Navigation property - exclude from JSON serialization to avoid circular references
    public CiCdEvent CiCdEvent { get; set; } = null!;

    [JsonPropertyName("analysisText")]
    public string AnalysisText { get; set; } = string.Empty;

    [JsonPropertyName("gitDiff")]
    public string? GitDiff { get; set; }

    [JsonPropertyName("errorLog")]
    public string? ErrorLog { get; set; }

    [JsonPropertyName("modelUsed")]
    public string ModelUsed { get; set; } = string.Empty;

    [JsonPropertyName("embedding")]
    [JsonConverter(typeof(VectorJsonConverter))]
    public Vector? Embedding { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("tokensUsed")]
    public int TokensUsed { get; set; }
}
