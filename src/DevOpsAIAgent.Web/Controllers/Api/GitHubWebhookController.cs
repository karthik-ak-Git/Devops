using DevOpsAIAgent.Web.Hubs;
using DevOpsAIAgent.Web.Models.DTOs;
using DevOpsAIAgent.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace DevOpsAIAgent.Web.Controllers.Api;

/// <summary>
/// API controller for receiving GitHub webhook events.
/// </summary>
[ApiController]
[Route("api/webhooks/github")]
public class GitHubWebhookController : ControllerBase
{
    private readonly IGitHubAnalysisService _analysisService;
    private readonly IHubContext<DashboardHub> _hubContext;
    private readonly ILogger<GitHubWebhookController> _logger;

    public GitHubWebhookController(
        IGitHubAnalysisService analysisService,
        IHubContext<DashboardHub> hubContext,
        ILogger<GitHubWebhookController> logger)
    {
        _analysisService = analysisService;
        _hubContext = hubContext;
        _logger = logger;
    }

    /// <summary>
    /// Webhook endpoint for GitHub Actions workflow_run events.
    /// </summary>
    /// <param name="payload">The GitHub webhook payload.</param>
    /// <returns>HTTP 200 OK if processed successfully.</returns>
    [HttpPost]
    public async Task<IActionResult> ReceiveWebhook([FromBody] GitHubWebhookPayload payload)
    {
        if (payload == null)
        {
            _logger.LogWarning("Received null webhook payload");
            return BadRequest("Invalid payload");
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
                    Timestamp = DateTime.UtcNow
                });

                _logger.LogInformation("Pipeline failure broadcast to dashboard clients for {Repo}", 
                    payload.Repository.FullName);

                return Ok(new { message = "Webhook processed successfully" });
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
