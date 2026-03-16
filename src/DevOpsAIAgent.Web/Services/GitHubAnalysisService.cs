using DevOpsAIAgent.Core.Interfaces.Services;
using Octokit;
using System.Text;

namespace DevOpsAIAgent.Web.Services;

public class GitHubAnalysisService : IGitHubAnalysisService
{
    private readonly GitHubClient _client;
    private readonly ILogger<GitHubAnalysisService> _logger;

    public GitHubAnalysisService(ILogger<GitHubAnalysisService> logger)
    {
        _logger = logger;
        var token = Environment.GetEnvironmentVariable("GITHUB_PAT");

        _client = new GitHubClient(new ProductHeaderValue("DevOpsAIAgent"))
        {
            Credentials = !string.IsNullOrWhiteSpace(token)
                ? new Credentials(token)
                : Credentials.Anonymous
        };

        if (string.IsNullOrWhiteSpace(token))
            _logger.LogWarning("GITHUB_PAT not set. GitHub API will be rate-limited.");
    }

    public async Task<(string GitDiff, string ErrorLog)> GetFailureContextAsync(
        string owner, string repo, string commitHash, long runId)
    {
        _logger.LogInformation("Fetching failure context for {Owner}/{Repo}, commit {Commit}, run {RunId}",
            owner, repo, commitHash, runId);

        var gitDiff = await GetCommitDiffAsync(owner, repo, commitHash);
        var errorLog = await GetWorkflowRunLogsAsync(owner, repo, runId);
        return (gitDiff, errorLog);
    }

    private async Task<string> GetCommitDiffAsync(string owner, string repo, string commitHash)
    {
        try
        {
            var commit = await _client.Repository.Commit.Get(owner, repo, commitHash);
            if (commit.Parents.Count == 0)
                return "No parent commit found for diff comparison.";

            var parentSha = commit.Parents[0].Sha;
            var comparison = await _client.Repository.Commit.Compare(owner, repo, parentSha, commitHash);

            var sb = new StringBuilder();
            sb.AppendLine($"Commit: {commitHash}");
            sb.AppendLine($"Author: {commit.Commit.Author.Name}");
            sb.AppendLine($"Message: {commit.Commit.Message}");
            sb.AppendLine($"Files Changed: {comparison.Files.Count}");
            sb.AppendLine(new string('=', 80));

            foreach (var file in comparison.Files)
            {
                sb.AppendLine();
                sb.AppendLine($"File: {file.Filename}");
                sb.AppendLine($"Status: {file.Status} (+{file.Additions} -{file.Deletions})");
                sb.AppendLine(new string('-', 80));
                sb.AppendLine(!string.IsNullOrEmpty(file.Patch) ? file.Patch : "(Binary file)");
            }

            return sb.ToString();
        }
        catch (NotFoundException)
        {
            return $"Commit {commitHash} not found in {owner}/{repo}.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching commit diff for {Commit}", commitHash);
            return $"Error fetching commit diff: {ex.Message}";
        }
    }

    private async Task<string> GetWorkflowRunLogsAsync(string owner, string repo, long runId)
    {
        try
        {
            var run = await _client.Actions.Workflows.Runs.Get(owner, repo, runId);
            var sb = new StringBuilder();
            sb.AppendLine($"Workflow: {run.Name}");
            sb.AppendLine($"Status: {run.Status} | Conclusion: {run.Conclusion?.StringValue ?? "N/A"}");
            sb.AppendLine($"URL: {run.HtmlUrl}");
            sb.AppendLine(new string('=', 80));

            var jobs = await _client.Actions.Workflows.Jobs.List(owner, repo, runId);
            foreach (var job in jobs.Jobs)
            {
                sb.AppendLine($"\nJob: {job.Name} [{job.Conclusion?.StringValue ?? job.Status.StringValue}]");
                if (job.Conclusion?.StringValue == "failure")
                {
                    foreach (var step in job.Steps)
                    {
                        var icon = step.Conclusion?.StringValue switch
                        {
                            "success" => "[OK]",
                            "failure" => "[FAIL]",
                            "skipped" => "[SKIP]",
                            _ => "[--]"
                        };
                        sb.AppendLine($"  {icon} {step.Name}");
                    }
                }
            }

            return sb.ToString();
        }
        catch (NotFoundException)
        {
            return $"Workflow run {runId} not found in {owner}/{repo}.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching workflow logs for run {RunId}", runId);
            return $"Error fetching workflow logs: {ex.Message}";
        }
    }
}
