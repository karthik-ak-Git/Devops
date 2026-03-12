using System.IO.Compression;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Octokit;

namespace DevOpsAIAgent.Core.Services;

public class GitHubAnalysisService : IGitHubAnalysisService
{
    private readonly ILogger<GitHubAnalysisService> _logger;
    private readonly GitHubClient _githubClient;
    private readonly string? _githubToken;

    public GitHubAnalysisService(
        ILogger<GitHubAnalysisService> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _githubToken = configuration["GitHub:PersonalAccessToken"];

        _githubClient = new GitHubClient(new ProductHeaderValue("DevOpsAIAgent"));

        if (!string.IsNullOrEmpty(_githubToken))
        {
            _githubClient.Credentials = new Credentials(_githubToken);
            _logger.LogInformation("GitHub client initialized with authentication");
        }
        else
        {
            _logger.LogWarning("No GitHub token configured. API rate limits will be severely restricted");
        }
    }

    public async Task<(string GitDiff, string ErrorLog)> GetFailureContextAsync(
        string owner,
        string repo,
        string commitHash,
        long runId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "Fetching failure context for {Owner}/{Repo}, Commit={Commit}, RunId={RunId}",
                owner, repo, commitHash, runId);

            var gitDiffTask = GetGitDiffAsync(owner, repo, commitHash, cancellationToken);
            var errorLogTask = GetWorkflowRunLogsAsync(owner, repo, runId, cancellationToken);

            await Task.WhenAll(gitDiffTask, errorLogTask);

            var gitDiff = await gitDiffTask;
            var errorLog = await errorLogTask;

            _logger.LogInformation("Successfully retrieved git diff ({DiffLength} chars) and error log ({LogLength} chars)",
                gitDiff.Length, errorLog.Length);

            return (gitDiff, errorLog);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve failure context for {Owner}/{Repo}", owner, repo);
            return ("Error retrieving git diff: " + ex.Message, "Error retrieving logs: " + ex.Message);
        }
    }

    private async Task<string> GetGitDiffAsync(string owner, string repo, string commitHash, CancellationToken cancellationToken)
    {
        try
        {
            var commit = await _githubClient.Repository.Commit.Get(owner, repo, commitHash);

            if (commit?.Files == null || commit.Files.Count == 0)
            {
                return "No file changes found in this commit.";
            }

            var diffBuilder = new StringBuilder();
            diffBuilder.AppendLine($"Commit: {commitHash}");
            diffBuilder.AppendLine($"Message: {commit.Commit.Message}");
            diffBuilder.AppendLine($"Author: {commit.Commit.Author.Name} <{commit.Commit.Author.Email}>");
            diffBuilder.AppendLine($"Date: {commit.Commit.Author.Date}");
            diffBuilder.AppendLine();
            diffBuilder.AppendLine($"Files Changed: {commit.Files.Count}");
            diffBuilder.AppendLine("=".PadRight(80, '='));
            diffBuilder.AppendLine();

            foreach (var file in commit.Files)
            {
                diffBuilder.AppendLine($"File: {file.Filename}");
                diffBuilder.AppendLine($"Status: {file.Status} (+{file.Additions} -{file.Deletions})");
                diffBuilder.AppendLine();

                if (!string.IsNullOrEmpty(file.Patch))
                {
                    diffBuilder.AppendLine(file.Patch);
                }
                else
                {
                    diffBuilder.AppendLine("(Binary file or no patch available)");
                }

                diffBuilder.AppendLine();
                diffBuilder.AppendLine("-".PadRight(80, '-'));
                diffBuilder.AppendLine();
            }

            return diffBuilder.ToString();
        }
        catch (NotFoundException)
        {
            _logger.LogWarning("Commit {Commit} not found in {Owner}/{Repo}", commitHash, owner, repo);
            return $"Commit {commitHash} not found in repository.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching git diff for {Commit}", commitHash);
            return $"Error fetching git diff: {ex.Message}";
        }
    }

    private async Task<string> GetWorkflowRunLogsAsync(string owner, string repo, long runId, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Fetching workflow run logs for RunId={RunId}", runId);

            var logArchive = await _githubClient.Actions.Workflows.Runs.GetLogs(owner, repo, runId);

            using var memoryStream = new MemoryStream(logArchive);
            using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read);

            var logBuilder = new StringBuilder();
            logBuilder.AppendLine($"Workflow Run ID: {runId}");
            logBuilder.AppendLine("=".PadRight(80, '='));
            logBuilder.AppendLine();

            var processedJobs = 0;
            foreach (var entry in archive.Entries)
            {
                if (entry.FullName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                {
                    using var entryStream = entry.Open();
                    using var reader = new StreamReader(entryStream);
                    var logContent = await reader.ReadToEndAsync(cancellationToken);

                    logBuilder.AppendLine($"Log File: {entry.FullName}");
                    logBuilder.AppendLine("-".PadRight(80, '-'));

                    var lines = logContent.Split('\n');
                    var errorLines = ExtractErrorLines(lines);

                    if (errorLines.Any())
                    {
                        logBuilder.AppendLine("ERRORS DETECTED:");
                        logBuilder.AppendLine();
                        foreach (var line in errorLines)
                        {
                            logBuilder.AppendLine(line);
                        }
                    }
                    else
                    {
                        var lastLines = lines.TakeLast(200).ToArray();
                        foreach (var line in lastLines)
                        {
                            logBuilder.AppendLine(line);
                        }
                    }

                    logBuilder.AppendLine();
                    logBuilder.AppendLine("=".PadRight(80, '='));
                    logBuilder.AppendLine();

                    processedJobs++;
                    if (processedJobs >= 3)
                    {
                        break;
                    }
                }
            }

            if (processedJobs == 0)
            {
                return "No log files found in the workflow run.";
            }

            return logBuilder.ToString();
        }
        catch (NotFoundException)
        {
            _logger.LogWarning("Workflow run {RunId} not found or logs unavailable", runId);
            return $"Workflow run logs not found for RunId {runId}. They may have expired or been deleted.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching workflow run logs for RunId={RunId}", runId);
            return $"Error fetching logs: {ex.Message}";
        }
    }

    private static List<string> ExtractErrorLines(string[] lines)
    {
        var errorLines = new List<string>();
        var keywords = new[] { "error", "failed", "failure", "exception", "fatal", "ERROR", "FAILED", "FAILURE" };

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (keywords.Any(keyword => line.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
            {
                var startIndex = Math.Max(0, i - 2);
                var endIndex = Math.Min(lines.Length - 1, i + 5);

                for (int j = startIndex; j <= endIndex; j++)
                {
                    if (!errorLines.Contains(lines[j]))
                    {
                        errorLines.Add(lines[j]);
                    }
                }

                if (errorLines.Count > 500)
                {
                    break;
                }
            }
        }

        return errorLines;
    }
}
