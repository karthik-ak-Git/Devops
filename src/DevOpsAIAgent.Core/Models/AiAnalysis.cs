using Pgvector;

namespace DevOpsAIAgent.Core.Models;

public class AiAnalysis
{
    public long Id { get; set; }
    public long CiCdEventId { get; set; }
    public CiCdEvent CiCdEvent { get; set; } = null!;
    public string AnalysisText { get; set; } = string.Empty;
    public string? GitDiff { get; set; }
    public string? ErrorLog { get; set; }
    public string ModelUsed { get; set; } = string.Empty;
    public Vector? Embedding { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int TokensUsed { get; set; }
}
