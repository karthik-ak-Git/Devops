using System.Text.Json.Serialization;

namespace DevOpsAIAgent.Core.DTOs;

public record GitHubWebhookPayload(
    [property: JsonPropertyName("action")] string Action,
    [property: JsonPropertyName("workflow_run")] WorkflowRunInfo WorkflowRun,
    [property: JsonPropertyName("repository")] RepositoryInfo Repository
);

public record WorkflowRunInfo(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("head_sha")] string HeadSha,
    [property: JsonPropertyName("head_branch")] string? HeadBranch,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("conclusion")] string? Conclusion,
    [property: JsonPropertyName("html_url")] string HtmlUrl,
    [property: JsonPropertyName("actor")] ActorInfo? Actor
);

public record RepositoryInfo(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("full_name")] string FullName,
    [property: JsonPropertyName("html_url")] string HtmlUrl,
    [property: JsonPropertyName("private")] bool IsPrivate,
    [property: JsonPropertyName("owner")] OwnerInfo Owner
);

public record OwnerInfo(
    [property: JsonPropertyName("login")] string Login,
    [property: JsonPropertyName("avatar_url")] string? AvatarUrl
);

public record ActorInfo(
    [property: JsonPropertyName("login")] string Login,
    [property: JsonPropertyName("avatar_url")] string? AvatarUrl
);
