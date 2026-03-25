using DevOpsAIAgent.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace DevOpsAIAgent.API.Controllers;

/// <summary>
/// Controller for LLM analysis operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AnalysisController : ControllerBase
{
    private readonly ILlmService _llmService;
    private readonly ILogger<AnalysisController> _logger;

    public AnalysisController(ILlmService llmService, ILogger<AnalysisController> logger)
    {
        _llmService = llmService;
        _logger = logger;
    }

    /// <summary>
    /// Generate DevOps analysis using specified LLM provider
    /// </summary>
    /// <param name="request">Analysis request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Analysis result</returns>
    [HttpPost("devops")]
    public async Task<IActionResult> GenerateDevOpsAnalysis(
        [FromBody] AnalysisRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Generating DevOps analysis with provider: {Provider}", request.Provider ?? "default");

            var analysis = await _llmService.GenerateAnalysisAsync(
                request.Prompt,
                request.Context,
                0, // ciCdEventId - default to 0 for now
                request.Provider,
                cancellationToken);

            return Ok(new AnalysisResponse
            {
                Success = true,
                Analysis = analysis,
                Message = "Analysis generated successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating DevOps analysis");
            return StatusCode(500, new AnalysisResponse
            {
                Success = false,
                Message = $"Error generating analysis: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Generate DevOps analysis with automatic fallback to other providers if primary fails
    /// </summary>
    /// <param name="request">Analysis request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Analysis result</returns>
    [HttpPost("devops/fallback")]
    public async Task<IActionResult> GenerateDevOpsAnalysisWithFallback(
        [FromBody] AnalysisRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Generating DevOps analysis with fallback");

            var analysis = await _llmService.GenerateAnalysisWithFallbackAsync(
                request.Prompt,
                request.Context,
                0, // ciCdEventId - default to 0 for now
                cancellationToken);

            return Ok(new AnalysisResponse
            {
                Success = true,
                Analysis = analysis,
                Message = "Analysis generated successfully with fallback support"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating DevOps analysis with fallback");
            return StatusCode(500, new AnalysisResponse
            {
                Success = false,
                Message = $"Error generating analysis: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Generate code review using specified LLM provider
    /// </summary>
    /// <param name="request">Code review request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Code review result</returns>
    [HttpPost("code-review")]
    public async Task<IActionResult> GenerateCodeReview(
        [FromBody] CodeReviewRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var provider = string.IsNullOrEmpty(request.Provider)
                ? _llmService.GetDefaultProvider()
                : _llmService.GetProvider(request.Provider);

            if (provider == null)
            {
                return BadRequest(new { Message = $"Provider '{request.Provider}' not found" });
            }

            var review = await provider.GenerateCodeReviewAsync(request.CodeChanges, cancellationToken);

            return Ok(new
            {
                Success = true,
                Review = review,
                Provider = provider.ProviderName,
                Message = "Code review generated successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating code review");
            return StatusCode(500, new { Success = false, Message = $"Error generating code review: {ex.Message}" });
        }
    }

    /// <summary>
    /// Get available LLM providers and their health status
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Providers status</returns>
    [HttpGet("providers")]
    public async Task<IActionResult> GetProviders(CancellationToken cancellationToken)
    {
        try
        {
            var providers = _llmService.GetAvailableProviders();
            var healthStatus = await _llmService.GetProvidersHealthAsync(cancellationToken);

            var result = providers.Select(p => new
            {
                Name = p.ProviderName,
                IsHealthy = healthStatus.GetValueOrDefault(p.ProviderName, false)
            }).ToList();

            return Ok(new
            {
                Success = true,
                Providers = result,
                DefaultProvider = _llmService.GetDefaultProvider().ProviderName
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting providers status");
            return StatusCode(500, new { Success = false, Message = $"Error getting providers: {ex.Message}" });
        }
    }

    /// <summary>
    /// Health check for LLM providers
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Health status</returns>
    [HttpGet("health")]
    public async Task<IActionResult> GetHealth(CancellationToken cancellationToken)
    {
        try
        {
            var healthStatus = await _llmService.GetProvidersHealthAsync(cancellationToken);
            var overallHealth = healthStatus.Values.Any(h => h);

            return Ok(new
            {
                Success = true,
                OverallHealthy = overallHealth,
                Providers = healthStatus
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking health");
            return StatusCode(500, new { Success = false, Message = $"Health check failed: {ex.Message}" });
        }
    }
}

/// <summary>
/// Analysis request model
/// </summary>
public class AnalysisRequest
{
    /// <summary>
    /// Analysis prompt/question
    /// </summary>
    public string Prompt { get; set; } = string.Empty;

    /// <summary>
    /// Context data for analysis
    /// </summary>
    public string Context { get; set; } = string.Empty;

    /// <summary>
    /// Specific LLM provider to use (optional)
    /// </summary>
    public string? Provider { get; set; }
}

/// <summary>
/// Code review request model
/// </summary>
public class CodeReviewRequest
{
    /// <summary>
    /// Code changes to review
    /// </summary>
    public string CodeChanges { get; set; } = string.Empty;

    /// <summary>
    /// Specific LLM provider to use (optional)
    /// </summary>
    public string? Provider { get; set; }
}

/// <summary>
/// Analysis response model
/// </summary>
public class AnalysisResponse
{
    /// <summary>
    /// Whether the operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Analysis result (if successful)
    /// </summary>
    public DevOpsAIAgent.Core.Models.AiAnalysis? Analysis { get; set; }

    /// <summary>
    /// Response message
    /// </summary>
    public string Message { get; set; } = string.Empty;
}