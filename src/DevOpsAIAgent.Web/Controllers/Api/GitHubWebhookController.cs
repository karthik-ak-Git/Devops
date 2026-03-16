using DevOpsAIAgent.Web.Hubs;
using DevOpsAIAgent.Web.Models.DTOs;
using DevOpsAIAgent.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace DevOpsAIAgent.Web.Controllers.Api;

/// <summary>
/// API controller for receiving GitHub webhook events.
/// Implements security verification using HMAC SHA256 signatures.
/// </summary>
[ApiController]
[Route("api/webhooks/github")]
public class GitHubWebhookController : ControllerBase
{
    private readonly IGitHubAnalysisService _analysisService;
    private readonly IAIAssistantService _aiAssistantService;
    private readonly IWebhookSecurityService _securityService;
    private readonly IHubContext<DashboardHub> _hubContext;
    private readonly ILogger<GitHubWebhookController> _logger;

    // Webhook secret should be configured in environment variables
    private readonly string? _webhookSecret = Environment.GetEnvironmentVariable("GITHUB_WEBHOOK_SECRET");

    public GitHubWebhookController(
        IGitHubAnalysisService analysisService,
        IAIAssistantService aiAssistantService,
        IWebhookSecurityService securityService,
        IHubContext<DashboardHub> hubContext,
        ILogger<GitHubWebhookController> logger)
    {
        _analysisService = analysisService;
        _aiAssistantService = aiAssistantService;
        _securityService = securityService;
        _hubContext = hubContext;
        _logger = logger;
    }

    /// <summary>
    /// Webhook endpoint for GitHub Actions workflow_run events.
    /// Verifies webhook signature for security and processes workflow failures.
    /// </summary>
    /// <param name="payload">The GitHub webhook payload.</param>
    /// <returns>HTTP 200 OK if processed successfully.</returns>
    [HttpPost]
    public async Task<IActionResult> ReceiveWebhook([FromBody] GitHubWebhookPayload payload)
    {
        // Step 1: Verify webhook signature
        var signatureHeader = Request.Headers["X-Hub-Signature-256"].ToString();
        var eventType = Request.Headers["X-GitHub-Event"].ToString();

        _logger.LogInformation("Received webhook: EventType={EventType}, Signature={SignaturePresent}",
            string.IsNullOrEmpty(eventType) ? "unknown" : eventType,
            !string.IsNullOrEmpty(signatureHeader));

        // Verify signature if secret is configured
        if (!string.IsNullOrWhiteSpace(_webhookSecret))
        {
            // Read the raw request body for signature verification
            Request.Body.Position = 0;
            using var reader = new StreamReader(Request.Body);
            var rawPayload = await reader.ReadToEndAsync();

            if (!_securityService.VerifyWebhookSignature(rawPayload, signatureHeader, _webhookSecret))
            {
                _logger.LogWarning("⚠️ Webhook signature verification FAILED. Rejecting request from {RemoteIP}",
                    Request.HttpContext.Connection.RemoteIpAddress);
                return Unauthorized(new { message = "Invalid webhook signature" });
            }

            _logger.LogInformation("✅ Webhook signature verified successfully");
        }
        else
        {
            _logger.LogWarning("⚠️ GITHUB_WEBHOOK_SECRET not configured. Signature verification disabled. This is not recommended for production!");
        }

        // Step 2: Validate payload
        if (payload == null)
        {
            _logger.LogWarning("Received null webhook payload");
            return BadRequest("Invalid payload");
        }

        // Step 3: Check event type
        var detectedEventType = _securityService.GetEventType(eventType);
        if (detectedEventType != "workflow_run")
        {
            _logger.LogDebug("Ignoring webhook event type: {EventType}", detectedEventType);
            return Ok(new { message = $"Webhook received but event type '{detectedEventType}' is not processed" });
        }

        _logger.LogInformation("Received GitHub webhook: Action={Action}, Repo={Repo}, Status={Status}, Conclusion={Conclusion}",
            payload.Action,
            payload.Repository.FullName,
            payload.WorkflowRun.Status,
            payload.WorkflowRun.Conclusion);

        // Only process completed workflows with failure conclusion
        if (payload.WorkflowRun.Status == "completed" &&
            payload.WorkflowRun.Conclusion?.Equals("failure", StringComparison.OrdinalIgnoreCase) == true)
        {
            _logger.LogInformation("Processing pipeline failure for {Repo} at commit {CommitHash}",
                payload.Repository.FullName,
                payload.WorkflowRun.HeadSha);

            try
            {
                // Get the failure context (diff and logs)
                var (gitDiff, errorLog) = await _analysisService.GetFailureContextAsync(
                    payload.Repository.Owner.Login,
                    payload.Repository.Name,
                    payload.WorkflowRun.HeadSha,
                    payload.WorkflowRun.Id);

                _logger.LogInformation("Retrieved failure context for {Repo}. Starting AI analysis...",
                    payload.Repository.FullName);

                // Get AI analysis and fix suggestion
                var aiSuggestion = await _aiAssistantService.AnalyzeFailureAsync(errorLog, gitDiff);

                _logger.LogInformation("AI analysis completed for {Repo}. Broadcasting to dashboard...",
                    payload.Repository.FullName);

                // Broadcast to all connected SignalR clients
                await _hubContext.Clients.All.SendAsync("ReceivePipelineFailure", new
                {
                    Repo = payload.Repository.FullName,
                    RepoUrl = payload.Repository.HtmlUrl,
                    CommitHash = payload.WorkflowRun.HeadSha,
                    WorkflowName = payload.WorkflowRun.Name,
                    RunUrl = payload.WorkflowRun.HtmlUrl,
                    RunId = payload.WorkflowRun.Id,
                    GitDiff = gitDiff,
                    ErrorLog = errorLog,
                    AiSuggestion = aiSuggestion,
                    Timestamp = DateTime.UtcNow
                });

                _logger.LogInformation("Pipeline failure with AI suggestion broadcast to dashboard clients for {Repo}", 
                    payload.Repository.FullName);

                return Ok(new { message = "Webhook processed successfully", aiAnalysisIncluded = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing webhook for {Repo}", payload.Repository.FullName);
                return StatusCode(500, new { message = "Error processing webhook", error = ex.Message });
            }
        }
        else
        {
            _logger.LogDebug("Webhook ignored: Status={Status}, Conclusion={Conclusion}",
                payload.WorkflowRun.Status,
                payload.WorkflowRun.Conclusion);

            return Ok(new { message = "Webhook received but not processed (not a failure)" });
        }
    }

    /// <summary>
    /// Health check endpoint to verify the webhook is accessible.
    /// </summary>
    [HttpGet("health")]
    public IActionResult HealthCheck()
    {
        return Ok(new
        {
            status = "healthy",
            service = "GitHub Webhook Receiver",
            timestamp = DateTime.UtcNow
        });
    }
}
