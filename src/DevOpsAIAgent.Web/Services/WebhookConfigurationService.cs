using DevOpsAIAgent.Core.DTOs;
using DevOpsAIAgent.Core.Interfaces.Repositories;
using DevOpsAIAgent.Core.Interfaces.Services;
using DevOpsAIAgent.Core.Models;
using Octokit;

namespace DevOpsAIAgent.Web.Services;

public class WebhookConfigurationService : IWebhookConfigurationService
{
    private readonly GitHubClient _client;
    private readonly ILogger<WebhookConfigurationService> _logger;
    private readonly IWebhookSecurityService _securityService;
    private readonly IWebhookConfigurationRepository _webhookRepo;
    private readonly bool _isConfigured;

    public WebhookConfigurationService(
        ILogger<WebhookConfigurationService> logger,
        IWebhookSecurityService securityService,
        IWebhookConfigurationRepository webhookRepo)
    {
        _logger = logger;
        _securityService = securityService;
        _webhookRepo = webhookRepo;

        var token = Environment.GetEnvironmentVariable("GITHUB_PAT");
        _isConfigured = !string.IsNullOrWhiteSpace(token);

        _client = new GitHubClient(new ProductHeaderValue("DevOpsAIAgent"));
        if (_isConfigured)
            _client.Credentials = new Credentials(token);
        else
            _logger.LogWarning("GITHUB_PAT not set. Webhook configuration disabled.");
    }

    public async Task<IReadOnlyList<RepositorySummary>> GetUserRepositoriesAsync()
    {
        if (!_isConfigured) return Array.Empty<RepositorySummary>();

        try
        {
            var allRepos = await _client.Repository.GetAllForCurrent();
            var results = new List<RepositorySummary>();

            foreach (var repo in allRepos.Where(r => r.Permissions.Admin))
            {
                bool hasWebhook = false;
                try
                {
                    var hooks = await _client.Repository.Hooks.GetAll(repo.Owner.Login, repo.Name);
                    hasWebhook = hooks.Any(h =>
                        h.Config.ContainsKey("url") &&
                        (h.Config["url"].ToString()?.Contains("/api/webhooks/github") ?? false));
                }
                catch { /* If we can't check, assume no webhook */ }

                results.Add(new RepositorySummary(
                    repo.FullName, repo.Owner.Login, repo.Name, repo.HtmlUrl,
                    repo.Private, hasWebhook, repo.Description ?? ""));
            }

            return results.OrderBy(r => r.FullName).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching repositories");
            return Array.Empty<RepositorySummary>();
        }
    }

    public async Task<WebhookConfigurationResponse> CreateWebhookAsync(
        string owner, string repo, string webhookUrl, IList<string>? events = null)
    {
        if (!_isConfigured)
            return new WebhookConfigurationResponse(false, "GITHUB_PAT not configured.");

        try
        {
            var existingHooks = await _client.Repository.Hooks.GetAll(owner, repo);
            var existing = existingHooks.FirstOrDefault(h =>
                h.Config.ContainsKey("url") && h.Config["url"].ToString() == webhookUrl);

            if (existing != null)
                return new WebhookConfigurationResponse(true,
                    $"Webhook already configured (ID: {existing.Id})", existing.Id, webhookUrl);

            var secret = _securityService.GenerateWebhookSecret();
            var selectedEvents = events is { Count: > 0 } ? events.ToArray() : new[] { "workflow_run" };

            var hook = new NewRepositoryHook("web", new Dictionary<string, string>
            {
                { "url", webhookUrl },
                { "content_type", "json" },
                { "secret", secret },
                { "insecure_ssl", "0" }
            })
            {
                Events = selectedEvents,
                Active = true
            };

            var created = await _client.Repository.Hooks.Create(owner, repo, hook);

            await _webhookRepo.AddAsync(new WebhookConfiguration
            {
                RepositoryFullName = $"{owner}/{repo}",
                Owner = owner,
                RepoName = repo,
                GitHubHookId = created.Id,
                WebhookUrl = webhookUrl,
                SecretHash = Convert.ToBase64String(
                    System.Security.Cryptography.SHA256.HashData(
                        System.Text.Encoding.UTF8.GetBytes(secret))),
                Events = selectedEvents.ToList()
            });

            _logger.LogInformation("Webhook created for {Owner}/{Repo}, ID: {Id}", owner, repo, created.Id);
            return new WebhookConfigurationResponse(true,
                $"Webhook created (ID: {created.Id}). Secret saved securely.",
                created.Id, webhookUrl);
        }
        catch (ForbiddenException)
        {
            return new WebhookConfigurationResponse(false,
                "Permission denied. Ensure your PAT has 'admin:repo_hook' scope.");
        }
        catch (NotFoundException)
        {
            return new WebhookConfigurationResponse(false,
                $"Repository '{owner}/{repo}' not found or no admin access.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating webhook for {Owner}/{Repo}", owner, repo);
            return new WebhookConfigurationResponse(false, $"Error: {ex.Message}");
        }
    }

    public async Task<bool> DeleteWebhookAsync(string owner, string repo, long hookId)
    {
        try
        {
            await _client.Repository.Hooks.Delete(owner, repo, (int)hookId);
            var config = await _webhookRepo.GetByRepositoryAsync($"{owner}/{repo}");
            if (config != null) await _webhookRepo.DeleteAsync(config.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting webhook {HookId}", hookId);
            return false;
        }
    }
}
