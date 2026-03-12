# Phase 3 - Webhook Receiver & Git Analysis Service

## Overview
This phase implements the webhook receiver that listens for GitHub Actions failures and analyzes the context using the GitHub API.

## Components

### 1. Webhook Data Models (`Models/DTOs/GitHubWebhookPayload.cs`)
- `GitHubWebhookPayload` - Main webhook payload
- `WorkflowRunInfo` - Workflow run details
- `RepositoryInfo` - Repository metadata
- `OwnerInfo` - Repository owner details

### 2. GitHub Analysis Service (`Services/`)
- `IGitHubAnalysisService` - Service interface
- `GitHubAnalysisService` - Implementation using Octokit
  - Fetches commit diffs between failed commit and parent
  - Retrieves workflow job details and step failures
  - Extracts error context for AI analysis

### 3. Webhook Controller (`Controllers/Api/GitHubWebhookController.cs`)
- Endpoint: `POST /api/webhooks/github`
- Processes GitHub webhook payloads
- Filters for completed workflows with "failure" conclusion
- Broadcasts failure data to SignalR clients via `DashboardHub`
- Health check endpoint: `GET /api/webhooks/github/health`

## Configuration

Add your GitHub Personal Access Token to `appsettings.Development.json`:

```json
{
  "GitHub": {
    "PersonalAccessToken": "ghp_your_token_here"
  }
}
```

## Testing the Webhook

### Using curl:
```bash
curl -X POST https://localhost:5001/api/webhooks/github \
  -H "Content-Type: application/json" \
  -d @test-payload.json
```

### Health Check:
```bash
curl https://localhost:5001/api/webhooks/github/health
```

## SignalR Event

The webhook broadcasts the following event to all connected clients:

**Event Name:** `ReceivePipelineFailure`

**Payload:**
```json
{
  "Repo": "owner/repository",
  "RepoUrl": "https://github.com/owner/repository",
  "CommitHash": "abc123...",
  "WorkflowName": "CI/CD Pipeline",
  "RunUrl": "https://github.com/owner/repository/actions/runs/123456",
  "RunId": 123456,
  "GitDiff": "...diff content...",
  "ErrorLog": "...error log content...",
  "Timestamp": "2024-03-12T10:30:00Z"
}
```

## Next Steps (Phase 4)
- Implement the AI Code Fixing Agent
- Create the Dashboard UI to display failures
- Add database persistence for events
