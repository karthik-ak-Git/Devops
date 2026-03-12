using System.Text.Json.Serialization;

namespace DevOpsAIAgent.Core.Models;

public record WebhookPayload(
    [property: JsonPropertyName("repository")] RepositoryInfo Repository,
    [property: JsonPropertyName("workflow_run")] WorkflowRunInfo WorkflowRun
);

public record RepositoryInfo(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("html_url")] string HtmlUrl,
    [property: JsonPropertyName("owner")] OwnerInfo Owner
);

public record OwnerInfo(
    [property: JsonPropertyName("login")] string Login
);

public record WorkflowRunInfo(
    [property: JsonPropertyName("head_sha")] string HeadSha,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("conclusion")] string? Conclusion,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("id")] long Id
);
