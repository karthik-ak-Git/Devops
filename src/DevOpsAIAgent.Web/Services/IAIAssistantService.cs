namespace DevOpsAIAgent.Web.Services;

/// <summary>
/// Service for AI-powered analysis of CI/CD failures and code fixes.
/// </summary>
public interface IAIAssistantService
{
    /// <summary>
    /// Analyzes a CI/CD failure using AI to determine root cause and suggest fixes.
    /// </summary>
    /// <param name="errorLog">The error log from the failed workflow run.</param>
    /// <param name="gitDiff">The Git diff showing changes that triggered the failure.</param>
    /// <returns>A markdown-formatted analysis with root cause and suggested code fix.</returns>
    Task<string> AnalyzeFailureAsync(string errorLog, string gitDiff);
}
