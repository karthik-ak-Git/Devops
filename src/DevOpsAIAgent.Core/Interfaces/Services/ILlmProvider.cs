using DevOpsAIAgent.Core.Models;

namespace DevOpsAIAgent.Core.Interfaces.Services;

/// <summary>
/// Interface for LLM (Large Language Model) providers
/// </summary>
public interface ILlmProvider
{
    /// <summary>
    /// Name of the LLM provider (e.g., "Gemini", "OpenRouter", "Ollama")
    /// </summary>
    string ProviderName { get; }

    /// <summary>
    /// Generate analysis for DevOps data
    /// </summary>
    Task<AiAnalysis> GenerateDevOpsAnalysisAsync(string prompt, string context, long ciCdEventId = 0, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generate code review analysis
    /// </summary>
    Task<string> GenerateCodeReviewAsync(string codeChanges, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generate deployment recommendation
    /// </summary>
    Task<string> GenerateDeploymentRecommendationAsync(string deploymentContext, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generate CI/CD pipeline analysis
    /// </summary>
    Task<string> GenerateCiCdAnalysisAsync(string pipelineData, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if the provider is available/healthy
    /// </summary>
    Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default);
}