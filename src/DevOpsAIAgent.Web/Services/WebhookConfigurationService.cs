using Octokit;

namespace DevOpsAIAgent.Web.Services;

/// <summary>
/// Implementation of webhook configuration service using Octokit.
/// </summary>
public class WebhookConfigurationService : IWebhookConfigurationService
{
    private readonly GitHubClient _client;
    private readonly ILogger<WebhookConfigurationService> _logger;
    private readonly bool _isConfigured;
    private readonly string? _configurationError;

    public WebhookConfigurationService(ILogger<WebhookConfigurationService> logger)
    {
        _logger = logger;

        // Load GitHub token from environment variable
        var token = Environment.GetEnvironmentVariable("GITHUB_PAT");
        if (string.IsNullOrWhiteSpace(token))
        {
            _isConfigured = false;
            _configurationError = "GITHUB_PAT environment variable is not set. Please configure it in your .env file.";
            _logger.LogWarning(_configurationError);

            // Create anonymous client for graceful degradation
            _client = new GitHubClient(new ProductHeaderValue("DevOpsAIAgent"));
        }
        else
        {
            _isConfigured = true;
            _client = new GitHubClient(new ProductHeaderValue("DevOpsAIAgent"))
            {
                Credentials = new Credentials(token)
            };
            _logger.LogInformation("Webhook Configuration Service initialized successfully");
        }
    }

    /// <summary>
    /// Gets all repositories the authenticated user has admin access to.
    /// </summary>
    public async Task<IReadOnlyList<RepositorySummary>> GetUserRepositoriesAsync()
    {
        if (!_isConfigured)
        {
            _logger.LogWarning("Cannot fetch repositories: {Error}", _configurationError);
            // Return empty list instead of throwing for graceful degradation
            return Array.Empty<RepositorySummary>();
        }

        _logger.LogInformation("Fetching user repositories with admin access");

        try
        {
            // Get all repositories the user has access to
            var allRepos = await _client.Repository.GetAllForCurrent();

            var repoSummaries = new List<RepositorySummary>();

            foreach (var repo in allRepos)
            {
                // Only include repos where user has admin permissions
                if (repo.Permissions.Admin)
                {
                    // Check if webhook already exists for this app
                    bool hasWebhook = false;
                    try
                    {
                        var hooks = await _client.Repository.Hooks.GetAll(repo.Owner.Login, repo.Name);
                        hasWebhook = hooks.Any(h => 
                            h.Config.ContainsKey("url") && 
                            h.Config["url"].ToString()?.Contains("/api/webhooks/github") == true);
                    }
                    catch
                    {
                        // If we can't check hooks, assume no webhook
                        hasWebhook = false;
                    }

                    repoSummaries.Add(new RepositorySummary(
                        FullName: repo.FullName,
                        Owner: repo.Owner.Login,
                        Name: repo.Name,
                        HtmlUrl: repo.HtmlUrl,
                        IsPrivate: repo.Private,
                        HasWebhook: hasWebhook,
                        Description: repo.Description ?? ""
                    ));
                }
            }

            _logger.LogInformation("Found {Count} repositories with admin access", repoSummaries.Count);
            return repoSummaries.OrderBy(r => r.FullName).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user repositories");
            throw;
        }
    }

    /// <summary>
    /// Creates a webhook for the specified repository.
    /// </summary>
    public async Task<WebhookConfigurationResult> CreateWebhookAsync(string owner, string repo, string webhookUrl)
    {
        if (!_isConfigured)
        {
            _logger.LogWarning("Cannot create webhook: {Error}", _configurationError);
            return new WebhookConfigurationResult(
                Success: false,
                Message: _configurationError ?? "Service not configured"
            );
        }

        _logger.LogInformation("Creating webhook for {Owner}/{Repo} with URL: {WebhookUrl}", owner, repo, webhookUrl);

        try
        {
            // Check if webhook already exists
            var existingHooks = await _client.Repository.Hooks.GetAll(owner, repo);
            var existingWebhook = existingHooks.FirstOrDefault(h => 
                h.Config.ContainsKey("url") && h.Config["url"].ToString() == webhookUrl);

            if (existingWebhook != null)
            {
                _logger.LogInformation("Webhook already exists for {Owner}/{Repo} with ID: {HookId}", 
                    owner, repo, existingWebhook.Id);

                return new WebhookConfigurationResult(
                    Success: true,
                    Message: $"Webhook already configured (ID: {existingWebhook.Id})",
                    WebhookId: existingWebhook.Id,
                    WebhookUrl: webhookUrl
                );
            }

            // Generate descriptive webhook name with timestamp
            var webhookName = $"DevOps-AI-Agent-{DateTime.UtcNow:yyyyMMdd-HHmmss}";

            // Create new webhook with explicit configuration
            var webhookConfig = new NewRepositoryHook("web", new Dictionary<string, string>
            {
                { "url", webhookUrl },
                { "content_type", "json" },
                { "insecure_ssl", "0" }
            })
            {
                Events = new[] { "workflow_run" },
                Active = true
            };

            var createdHook = await _client.Repository.Hooks.Create(owner, repo, webhookConfig);

            _logger.LogInformation("Webhook '{Name}' created successfully for {Owner}/{Repo}. Hook ID: {HookId}", 
                webhookName, owner, repo, createdHook.Id);

            return new WebhookConfigurationResult(
                Success: true,
                Message: $"✓ Webhook '{webhookName}' created successfully! (ID: {createdHook.Id})",
                WebhookId: createdHook.Id,
                WebhookUrl: webhookUrl
            );
        }
        catch (NotFoundException)
        {
            _logger.LogWarning("Repository {Owner}/{Repo} not found or no access", owner, repo);
            return new WebhookConfigurationResult(
                Success: false,
                Message: $"Repository '{owner}/{repo}' not found. Verify the repository exists and you have admin access."
            );
        }
        catch (ForbiddenException)
        {
            _logger.LogWarning("Forbidden: No permission to create webhook for {Owner}/{Repo}", owner, repo);
            return new WebhookConfigurationResult(
                Success: false,
                Message: "Permission denied. You need admin access to the repository to create webhooks."
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating webhook for {Owner}/{Repo}", owner, repo);
            return new WebhookConfigurationResult(
                Success: false,
                Message: $"Error creating webhook: {ex.Message}"
            );
        }
    }

    /// <summary>
    /// Lists all webhooks for the specified repository.
    /// </summary>
    public async Task<IReadOnlyList<RepositoryHook>> ListWebhooksAsync(string owner, string repo)
    {
        _logger.LogInformation("Listing webhooks for {Owner}/{Repo}", owner, repo);

        try
        {
            return await _client.Repository.Hooks.GetAll(owner, repo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing webhooks for {Owner}/{Repo}", owner, repo);
            return Array.Empty<RepositoryHook>();
        }
    }

    /// <summary>
    /// Deletes a webhook from the specified repository.
    /// </summary>
    public async Task<bool> DeleteWebhookAsync(string owner, string repo, long hookId)
    {
        _logger.LogInformation("Deleting webhook {HookId} from {Owner}/{Repo}", hookId, owner, repo);

        try
        {
            await _client.Repository.Hooks.Delete(owner, repo, (int)hookId);
            _logger.LogInformation("Webhook {HookId} deleted successfully from {Owner}/{Repo}", hookId, owner, repo);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting webhook {HookId} from {Owner}/{Repo}", hookId, owner, repo);
            return false;
        }
    }
}
