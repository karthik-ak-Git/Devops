namespace DevOpsAIAgent.Web.Services;

/// <summary>
/// Service for analyzing GitHub repository failures and extracting relevant context.
/// </summary>
public interface IGitHubAnalysisService
{
    /// <summary>
    /// Retrieves the Git diff and error logs for a failed workflow run.
    /// </summary>
    /// <param name="owner">The repository owner (organization or user).</param>
    /// <param name="repo">The repository name.</param>
    /// <param name="commitHash">The commit SHA that triggered the workflow.</param>
    /// <param name="runId">The workflow run ID.</param>
    /// <returns>A tuple containing the Git diff and error log.</returns>
    Task<(string GitDiff, string ErrorLog)> GetFailureContextAsync(
        string owner,
        string repo,
        string commitHash,
        long runId);
}
