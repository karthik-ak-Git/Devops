using DevOpsAIAgent.Core.Models;

namespace DevOpsAIAgent.Core.Interfaces.Services;

/// <summary>
/// Service for managing multiple LLM providers
/// </summary>
public interface ILlmService
{
    /// <summary>
    /// Get all available LLM providers
    /// </summary>
    IEnumerable<ILlmProvider> GetAvailableProviders();

    /// <summary>
    /// Get a specific LLM provider by name
    /// </summary>
    ILlmProvider? GetProvider(string providerName);

    /// <summary>
    /// Get the default/primary LLM provider
    /// </summary>
    ILlmProvider GetDefaultProvider();

    /// <summary>
    /// Generate analysis using the specified provider or default
    /// </summary>
    Task<AiAnalysis> GenerateAnalysisAsync(string prompt, string context, long ciCdEventId = 0, string? providerName = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generate analysis with fallback to other providers if primary fails
    /// </summary>
    Task<AiAnalysis> GenerateAnalysisWithFallbackAsync(string prompt, string context, long ciCdEventId = 0, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get health status of all providers
    /// </summary>
    Task<Dictionary<string, bool>> GetProvidersHealthAsync(CancellationToken cancellationToken = default);
}