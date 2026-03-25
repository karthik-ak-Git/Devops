using DevOpsAIAgent.Core.Models;
using DevOpsAIAgent.Core.Interfaces.Services;
using DevOpsAIAgent.Core.Models.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace DevOpsAIAgent.API.Services;

/// <summary>
/// Service for managing multiple LLM providers
/// </summary>
public class LlmService : ILlmService
{
    private readonly LlmConfiguration _config;
    private readonly ILogger<LlmService> _logger;
    private readonly IEnumerable<ILlmProvider> _providers;

    public LlmService(
        IOptions<LlmConfiguration> options,
        ILogger<LlmService> logger,
        IEnumerable<ILlmProvider> providers)
    {
        _config = options.Value;
        _logger = logger;
        _providers = providers;
    }

    public IEnumerable<ILlmProvider> GetAvailableProviders()
    {
        return _providers;
    }

    public ILlmProvider? GetProvider(string providerName)
    {
        return _providers.FirstOrDefault(p =>
            string.Equals(p.ProviderName, providerName, StringComparison.OrdinalIgnoreCase));
    }

    public ILlmProvider GetDefaultProvider()
    {
        var defaultProvider = GetProvider(_config.DefaultProvider);
        if (defaultProvider != null)
        {
            return defaultProvider;
        }

        // Fallback to first available provider
        var firstProvider = _providers.FirstOrDefault();
        if (firstProvider != null)
        {
            _logger.LogWarning("Default provider '{DefaultProvider}' not found, using '{FirstProvider}' instead",
                _config.DefaultProvider, firstProvider.ProviderName);
            return firstProvider;
        }

        throw new InvalidOperationException("No LLM providers are configured");
    }

    public async Task<AiAnalysis> GenerateAnalysisAsync(
        string prompt,
        string context,
        long ciCdEventId = 0,
        string? providerName = null,
        CancellationToken cancellationToken = default)
    {
        var provider = string.IsNullOrEmpty(providerName)
            ? GetDefaultProvider()
            : GetProvider(providerName);

        if (provider == null)
        {
            throw new ArgumentException($"LLM provider '{providerName}' not found", nameof(providerName));
        }

        _logger.LogInformation("Generating analysis using provider: {Provider}", provider.ProviderName);

        try
        {
            return await provider.GenerateDevOpsAnalysisAsync(prompt, context, ciCdEventId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate analysis with provider: {Provider}", provider.ProviderName);
            throw;
        }
    }

    public async Task<AiAnalysis> GenerateAnalysisWithFallbackAsync(
        string prompt,
        string context,
        long ciCdEventId = 0,
        CancellationToken cancellationToken = default)
    {
        if (!_config.EnableFallback)
        {
            return await GenerateAnalysisAsync(prompt, context, ciCdEventId, null, cancellationToken);
        }

        var providers = GetAvailableProviders().ToList();
        var exceptions = new List<Exception>();

        // Try default provider first
        try
        {
            var defaultProvider = GetDefaultProvider();
            _logger.LogInformation("Attempting analysis with default provider: {Provider}", defaultProvider.ProviderName);
            return await defaultProvider.GenerateDevOpsAnalysisAsync(prompt, context, ciCdEventId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Default provider failed, trying fallback providers");
            exceptions.Add(ex);
        }

        // Try other providers as fallback
        foreach (var provider in providers.Where(p => !string.Equals(p.ProviderName, _config.DefaultProvider, StringComparison.OrdinalIgnoreCase)))
        {
            try
            {
                _logger.LogInformation("Attempting fallback analysis with provider: {Provider}", provider.ProviderName);
                return await provider.GenerateDevOpsAnalysisAsync(prompt, context, ciCdEventId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Fallback provider {Provider} failed", provider.ProviderName);
                exceptions.Add(ex);
            }
        }

        // All providers failed
        _logger.LogError("All LLM providers failed to generate analysis");
        throw new AggregateException("All LLM providers failed", exceptions);
    }

    public async Task<Dictionary<string, bool>> GetProvidersHealthAsync(CancellationToken cancellationToken = default)
    {
        var healthStatus = new Dictionary<string, bool>();

        var healthTasks = _providers.Select(async provider =>
        {
            try
            {
                var isHealthy = await provider.IsHealthyAsync(cancellationToken);
                return new { Provider = provider.ProviderName, IsHealthy = isHealthy };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Health check failed for provider: {Provider}", provider.ProviderName);
                return new { Provider = provider.ProviderName, IsHealthy = false };
            }
        });

        var results = await Task.WhenAll(healthTasks);

        foreach (var result in results)
        {
            healthStatus[result.Provider] = result.IsHealthy;
        }

        return healthStatus;
    }
}