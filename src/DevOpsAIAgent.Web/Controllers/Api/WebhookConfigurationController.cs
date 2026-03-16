using DevOpsAIAgent.Web.Models.DTOs;
using DevOpsAIAgent.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace DevOpsAIAgent.Web.Controllers.Api;

/// <summary>
/// API controller for managing GitHub webhook configurations.
/// </summary>
[ApiController]
[Route("api/webhooks")]
public class WebhookConfigurationController : ControllerBase
{
    private readonly IWebhookConfigurationService _webhookService;
    private readonly ILogger<WebhookConfigurationController> _logger;

    public WebhookConfigurationController(
        IWebhookConfigurationService webhookService,
        ILogger<WebhookConfigurationController> logger)
    {
        _webhookService = webhookService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all repositories the authenticated user has admin access to.
    /// </summary>
    /// <returns>List of repositories with webhook status.</returns>
    [HttpGet("repositories")]
    public async Task<IActionResult> GetUserRepositories()
    {
        _logger.LogInformation("Fetching user repositories");

        try
        {
            var repositories = await _webhookService.GetUserRepositoriesAsync();

            if (repositories.Count == 0)
            {
                // Check if it's because of missing configuration
                var githubPat = Environment.GetEnvironmentVariable("GITHUB_PAT");
                if (string.IsNullOrWhiteSpace(githubPat))
                {
                    return Ok(new
                    {
                        success = false,
                        count = 0,
                        repositories = Array.Empty<object>(),
                        message = "GITHUB_PAT not configured. Please add your GitHub Personal Access Token to the .env file.",
                        configurationRequired = true
                    });
                }
            }

            return Ok(new
            {
                success = true,
                count = repositories.Count,
                repositories = repositories.Select(r => new
                {
                    fullName = r.FullName,
                    owner = r.Owner,
                    name = r.Name,
                    htmlUrl = r.HtmlUrl,
                    isPrivate = r.IsPrivate,
                    hasWebhook = r.HasWebhook,
                    description = r.Description
                })
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user repositories");
            return StatusCode(500, new
            {
                success = false,
                message = $"Error fetching repositories: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Creates a webhook for the specified repository.
    /// </summary>
    /// <param name="request">Webhook creation request.</param>
    /// <returns>Result of webhook creation.</returns>
    [HttpPost("configure")]
    public async Task<IActionResult> ConfigureWebhook([FromBody] WebhookConfigurationRequest? request)
    {
        // Validate request
        if (request == null)
        {
            _logger.LogWarning("Webhook configuration request is null");
            return BadRequest(new 
            { 
                success = false, 
                message = "Request body is required" 
            });
        }

        string owner, repo;

        // Support both "owner/repo" format and separate fields
        if (!string.IsNullOrWhiteSpace(request.FullName) && request.FullName.Contains('/'))
        {
            var parts = request.FullName.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
            {
                _logger.LogWarning("Invalid FullName format: {FullName}", request.FullName);
                return BadRequest(new 
                { 
                    success = false, 
                    message = "FullName must be in 'owner/repo' format" 
                });
            }
            owner = parts[0];
            repo = parts[1];
        }
        else if (!string.IsNullOrWhiteSpace(request.Owner) && !string.IsNullOrWhiteSpace(request.Repo))
        {
            owner = request.Owner;
            repo = request.Repo;
        }
        else
        {
            _logger.LogWarning("Missing repository identification: FullName={FullName}, Owner={Owner}, Repo={Repo}", 
                request.FullName, request.Owner, request.Repo);
            return BadRequest(new 
            { 
                success = false, 
                message = "Either 'FullName' (owner/repo) or both 'Owner' and 'Repo' are required" 
            });
        }

        // Get the webhook URL - use custom if provided, otherwise build from request
        string webhookUrl = request.WebhookUrl;

        if (string.IsNullOrWhiteSpace(webhookUrl))
        {
            // Build from request
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            webhookUrl = $"{baseUrl}/api/webhooks/github";
        }

        // Validate webhook URL
        if (!webhookUrl.StartsWith("https://") && !webhookUrl.StartsWith("http://"))
        {
            _logger.LogWarning("Invalid webhook URL format: {WebhookUrl}", webhookUrl);
            return BadRequest(new 
            { 
                success = false, 
                message = "Webhook URL must start with http:// or https://" 
            });
        }

        // Warn if using localhost
        if (webhookUrl.Contains("localhost") || webhookUrl.Contains("127.0.0.1"))
        {
            _logger.LogWarning("Webhook URL is localhost: {WebhookUrl}", webhookUrl);
            return BadRequest(new 
            { 
                success = false, 
                message = "Webhook URL cannot use localhost. Please use ngrok:\n1. Download: https://ngrok.com/download\n2. Run: ngrok http 5120\n3. Use the ngrok URL (e.g., https://abc123.ngrok.io/api/webhooks/github)" 
            });
        }

        _logger.LogInformation("Configuring webhook for {Owner}/{Repo} with URL: {WebhookUrl}",
            owner, repo, webhookUrl);

        // Prepare webhook events (default to workflow_run if none selected)
        var events = request.Events ?? new List<string>();
        if (events.Count == 0)
        {
            events.Add("workflow_run");
        }

        _logger.LogInformation("Webhook events: {Events}", string.Join(", ", events));

        try
        {
            var result = await _webhookService.CreateWebhookAsync(owner, repo, webhookUrl, events);

            if (result.Success)
            {
                _logger.LogInformation("Webhook successfully configured for {Owner}/{Repo}", owner, repo);
                return Ok(new
                {
                    success = true,
                    message = result.Message,
                    webhookId = result.WebhookId,
                    webhookUrl = result.WebhookUrl
                });
            }
            else
            {
                _logger.LogWarning("Webhook configuration failed for {Owner}/{Repo}: {Message}", owner, repo, result.Message);
                return StatusCode(403, new
                {
                    success = false,
                    message = result.Message
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error configuring webhook for {Owner}/{Repo}", owner, repo);
            return StatusCode(500, new
            {
                success = false,
                message = $"Error configuring webhook: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Lists all webhooks for the specified repository.
    /// </summary>
    [HttpGet("list/{owner}/{repo}")]
    public async Task<IActionResult> ListWebhooks(string owner, string repo)
    {
        _logger.LogInformation("Listing webhooks for {Owner}/{Repo}", owner, repo);

        var hooks = await _webhookService.ListWebhooksAsync(owner, repo);

        return Ok(new
        {
            success = true,
            count = hooks.Count,
            webhooks = hooks.Select(h => new
            {
                id = h.Id,
                name = h.Name,
                active = h.Active,
                events = h.Events,
                url = h.Config.ContainsKey("url") ? h.Config["url"] : null,
                createdAt = h.CreatedAt,
                updatedAt = h.UpdatedAt
            })
        });
    }

    /// <summary>
    /// Deletes a webhook from the specified repository.
    /// </summary>
    [HttpDelete("{owner}/{repo}/{hookId}")]
    public async Task<IActionResult> DeleteWebhook(string owner, string repo, long hookId)
    {
        _logger.LogInformation("Deleting webhook {HookId} from {Owner}/{Repo}", hookId, owner, repo);

        var success = await _webhookService.DeleteWebhookAsync(owner, repo, hookId);

        return success
            ? Ok(new { success = true, message = $"Webhook {hookId} deleted successfully" })
            : BadRequest(new { success = false, message = $"Failed to delete webhook {hookId}" });
    }
}
