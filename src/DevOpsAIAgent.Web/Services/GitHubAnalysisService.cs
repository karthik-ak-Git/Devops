using Octokit;
using System.Text;

namespace DevOpsAIAgent.Web.Services;

/// <summary>
/// Implementation of GitHub analysis service using Octokit.
/// </summary>
public class GitHubAnalysisService : IGitHubAnalysisService
{
    private readonly GitHubClient _client;
    private readonly ILogger<GitHubAnalysisService> _logger;

    public GitHubAnalysisService(IConfiguration configuration, ILogger<GitHubAnalysisService> logger)
    {
        _logger = logger;

        var token = configuration["GitHub:PersonalAccessToken"];
        if (string.IsNullOrWhiteSpace(token))
        {
            _logger.LogWarning("GitHub Personal Access Token not configured. GitHub API calls may be rate-limited.");
        }

        _client = new GitHubClient(new ProductHeaderValue("DevOpsAIAgent"))
        {
            Credentials = !string.IsNullOrWhiteSpace(token)
                ? new Credentials(token)
                : Credentials.Anonymous
        };
    }

    /// <summary>
    /// Retrieves the Git diff and error logs for a failed workflow run.
    /// </summary>
    public async Task<(string GitDiff, string ErrorLog)> GetFailureContextAsync(
        string owner,
        string repo,
        string commitHash,
        long runId)
    {
        _logger.LogInformation("Fetching failure context for {Owner}/{Repo}, commit {CommitHash}, run {RunId}",
            owner, repo, commitHash, runId);

        try
        {
            // Fetch the commit diff
            var gitDiff = await GetCommitDiffAsync(owner, repo, commitHash);

            // Fetch the workflow run logs
            var errorLog = await GetWorkflowRunLogsAsync(owner, repo, runId);

            return (gitDiff, errorLog);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve failure context for {Owner}/{Repo}, commit {CommitHash}, run {RunId}",
                owner, repo, commitHash, runId);
            throw;
        }
    }

    /// <summary>
    /// Gets the diff for a specific commit compared to its parent.
    /// </summary>
    private async Task<string> GetCommitDiffAsync(string owner, string repo, string commitHash)
    {
        try
        {
            _logger.LogDebug("Fetching commit {CommitHash} from {Owner}/{Repo}", commitHash, owner, repo);

            // Get the commit
            var commit = await _client.Repository.Commit.Get(owner, repo, commitHash);

            // Get the parent commit if it exists
            if (commit.Parents.Count == 0)
            {
                _logger.LogWarning("Commit {CommitHash} has no parent - returning empty diff", commitHash);
                return "No parent commit found for diff comparison.";
            }

            var parentSha = commit.Parents[0].Sha;

            // Use GitHub's compare endpoint to get the diff
            var comparison = await _client.Repository.Commit.Compare(owner, repo, parentSha, commitHash);

            var diffBuilder = new StringBuilder();
            diffBuilder.AppendLine($"Commit: {commitHash}");
            diffBuilder.AppendLine($"Parent: {parentSha}");
            diffBuilder.AppendLine($"Author: {commit.Commit.Author.Name} <{commit.Commit.Author.Email}>");
            diffBuilder.AppendLine($"Date: {commit.Commit.Author.Date}");
            diffBuilder.AppendLine($"Message: {commit.Commit.Message}");
            diffBuilder.AppendLine();
            diffBuilder.AppendLine($"Files Changed: {comparison.Files.Count}");
            diffBuilder.AppendLine(new string('=', 80));

            foreach (var file in comparison.Files)
            {
                diffBuilder.AppendLine();
                diffBuilder.AppendLine($"File: {file.Filename}");
                diffBuilder.AppendLine($"Status: {file.Status} (+{file.Additions} -{file.Deletions})");
                diffBuilder.AppendLine(new string('-', 80));

                if (!string.IsNullOrEmpty(file.Patch))
                {
                    diffBuilder.AppendLine(file.Patch);
                }
                else
                {
                    diffBuilder.AppendLine("(Binary file or no patch available)");
                }

                diffBuilder.AppendLine();
            }

            return diffBuilder.ToString();
        }
        catch (NotFoundException)
        {
            _logger.LogWarning("Commit {CommitHash} not found in {Owner}/{Repo}", commitHash, owner, repo);
            return $"Commit {commitHash} not found in repository {owner}/{repo}.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching commit diff for {CommitHash}", commitHash);
            return $"Error fetching commit diff: {ex.Message}";
        }
    }

    /// <summary>
    /// Gets the workflow run logs for a specific run ID.
    /// </summary>
    private async Task<string> GetWorkflowRunLogsAsync(string owner, string repo, long runId)
    {
        try
        {
            _logger.LogDebug("Fetching workflow run logs for run {RunId} from {Owner}/{Repo}", runId, owner, repo);

            // Get the workflow run
            var workflowRun = await _client.Actions.Workflows.Runs.Get(owner, repo, runId);

            var logBuilder = new StringBuilder();
            logBuilder.AppendLine($"Workflow: {workflowRun.Name}");
            logBuilder.AppendLine($"Run Number: {workflowRun.RunNumber}");
            logBuilder.AppendLine($"Status: {workflowRun.Status}");
            logBuilder.AppendLine($"Conclusion: {workflowRun.Conclusion?.StringValue ?? "N/A"}");
            logBuilder.AppendLine($"Run URL: {workflowRun.HtmlUrl}");
            logBuilder.AppendLine(new string('=', 80));
            logBuilder.AppendLine();

            // Get the jobs for this workflow run
            var jobs = await _client.Actions.Workflows.Jobs.List(owner, repo, runId);

            foreach (var job in jobs.Jobs)
            {
                logBuilder.AppendLine($"Job: {job.Name}");
                logBuilder.AppendLine($"Status: {job.Status} | Conclusion: {job.Conclusion?.StringValue ?? "N/A"}");

                if (job.Conclusion?.StringValue == "failure")
                {
                    logBuilder.AppendLine("Steps:");
                    foreach (var step in job.Steps)
                    {
                        var statusIcon = step.Conclusion?.StringValue switch
                        {
                            "success" => "✓",
                            "failure" => "✗",
                            "skipped" => "⊘",
                            _ => "◦"
                        };

                        logBuilder.AppendLine($"  {statusIcon} {step.Name} - {step.Conclusion?.StringValue ?? "N/A"}");

                        if (step.Conclusion?.StringValue == "failure")
                        {
                            logBuilder.AppendLine($"    Started: {step.StartedAt}");
                            logBuilder.AppendLine($"    Completed: {step.CompletedAt}");
                        }
                    }
                }

                logBuilder.AppendLine();
            }

            // Note: GitHub API doesn't provide direct access to raw log content via Octokit in a simple way.
            // The logs need to be downloaded as a zip archive. For now, we provide job-level information.
            // You can enhance this by downloading and parsing the log archive if needed.

            logBuilder.AppendLine("Note: Detailed step logs require downloading the log archive from GitHub.");
            logBuilder.AppendLine($"Log Archive URL: {workflowRun.LogsUrl}");

            return logBuilder.ToString();
        }
        catch (NotFoundException)
        {
            _logger.LogWarning("Workflow run {RunId} not found in {Owner}/{Repo}", runId, owner, repo);
            return $"Workflow run {runId} not found in repository {owner}/{repo}.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching workflow run logs for run {RunId}", runId);
            return $"Error fetching workflow run logs: {ex.Message}";
        }
    }
}
