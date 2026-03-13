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
    public async Task<IActionResult> ConfigureWebhook([FromBody] WebhookConfigurationRequest request)
    {
        string owner, repo;

        // Support both "owner/repo" format and separate fields
        if (!string.IsNullOrWhiteSpace(request.FullName) && request.FullName.Contains('/'))
        {
            var parts = request.FullName.Split('/');
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
            return BadRequest(new { success = false, message = "Either 'FullName' (owner/repo) or both 'Owner' and 'Repo' are required" });
        }

        // Get the base URL from the request
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var webhookUrl = $"{baseUrl}/api/webhooks/github";

        _logger.LogInformation("Configuring webhook for {Owner}/{Repo} with URL: {WebhookUrl}",
            owner, repo, webhookUrl);

        var result = await _webhookService.CreateWebhookAsync(owner, repo, webhookUrl);

        return result.Success
            ? Ok(new
            {
                success = true,
                message = result.Message,
                webhookId = result.WebhookId,
                webhookUrl = result.WebhookUrl
            })
            : BadRequest(new
            {
                success = false,
                message = result.Message
            });
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

/// <summary>
/// Request model for webhook configuration.
/// </summary>
public record WebhookConfigurationRequest(
    string? Owner = null,
    string? Repo = null,
    string? FullName = null
);
