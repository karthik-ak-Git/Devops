namespace DevOpsAIAgent.Core.Interfaces.Services;

public interface IGitHubAnalysisService
{
    Task<(string GitDiff, string ErrorLog)> GetFailureContextAsync(string owner, string repo, string commitHash, long runId);
}
