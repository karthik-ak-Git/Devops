using System.Text.Json.Serialization;

namespace DevOpsAIAgent.Web.Models.DTOs;

/// <summary>
/// Represents a GitHub Actions webhook payload for workflow_run events.
/// </summary>
public record GitHubWebhookPayload(
    [property: JsonPropertyName("action")] string Action,
    [property: JsonPropertyName("workflow_run")] WorkflowRunInfo WorkflowRun,
    [property: JsonPropertyName("repository")] RepositoryInfo Repository
);

/// <summary>
/// Workflow run information from the webhook payload.
/// </summary>
public record WorkflowRunInfo(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("head_sha")] string HeadSha,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("conclusion")] string? Conclusion,
    [property: JsonPropertyName("html_url")] string HtmlUrl,
    [property: JsonPropertyName("run_number")] int RunNumber,
    [property: JsonPropertyName("run_attempt")] int RunAttempt
);

/// <summary>
/// Repository information from the webhook payload.
/// </summary>
public record RepositoryInfo(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("full_name")] string FullName,
    [property: JsonPropertyName("html_url")] string HtmlUrl,
    [property: JsonPropertyName("owner")] OwnerInfo Owner
);

/// <summary>
/// Repository owner information from the webhook payload.
/// </summary>
public record OwnerInfo(
    [property: JsonPropertyName("login")] string Login,
    [property: JsonPropertyName("type")] string Type
);
