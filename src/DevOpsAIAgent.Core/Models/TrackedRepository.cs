namespace DevOpsAIAgent.Core.Models;

public class TrackedRepository
{
    public long Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? HtmlUrl { get; set; }
    public bool IsPrivate { get; set; }
    public bool WebhookConfigured { get; set; }
    public int TotalBuilds { get; set; }
    public int FailedBuilds { get; set; }
    public double SuccessRate => TotalBuilds > 0 ? (double)(TotalBuilds - FailedBuilds) / TotalBuilds * 100 : 0;
    public DateTime? LastBuildAt { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
