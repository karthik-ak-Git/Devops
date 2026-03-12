namespace DevOpsAIAgent.Core.Models;

public record CiCdEvent(
    string RepositoryName,
    string RepositoryUrl,
    string CommitHash,
    string Status,
    string? Conclusion,
    string WorkflowName,
    DateTime ReceivedAt,
    string Owner,
    long RunId
)
{
    public static CiCdEvent FromWebhookPayload(WebhookPayload payload)
    {
        return new CiCdEvent(
            RepositoryName: payload.Repository.Name,
            RepositoryUrl: payload.Repository.HtmlUrl,
            CommitHash: payload.WorkflowRun.HeadSha,
            Status: payload.WorkflowRun.Status,
            Conclusion: payload.WorkflowRun.Conclusion,
            WorkflowName: payload.WorkflowRun.Name,
            ReceivedAt: DateTime.UtcNow,
            Owner: payload.Repository.Owner.Login,
            RunId: payload.WorkflowRun.Id
        );
    }

    public bool IsFailure => Conclusion?.Equals("failure", StringComparison.OrdinalIgnoreCase) == true;
}
