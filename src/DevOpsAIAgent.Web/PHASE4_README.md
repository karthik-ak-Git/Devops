# Phase 4 - AI Analysis Engine

## Overview
This phase integrates Azure OpenAI / OpenAI to provide intelligent analysis of CI/CD failures and automated fix suggestions.

## Components Created

### 1. AI Assistant Service Interface (`Services/IAIAssistantService.cs`)
Defines the contract for AI-powered failure analysis.

### 2. AI Assistant Service Implementation (`Services/AIAssistantService.cs`)
- Supports both **Standard OpenAI** and **Azure OpenAI**
- Configurable model selection (defaults to `gpt-4o`)
- Structured prompts for DevOps/C# expertise
- Returns markdown-formatted analysis with:
  - Root cause analysis
  - Recommended code fix
  - Explanation of the fix

### 3. Updated Webhook Controller
Enhanced to include AI analysis in the pipeline:
1. Receives GitHub webhook
2. Fetches Git diff and error logs
3. **Sends to AI for analysis** ← NEW
4. Broadcasts complete context to dashboard via SignalR

### 4. Updated SignalR Broadcast
The `ReceivePipelineFailure` event now includes:
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
  "AiSuggestion": "...AI-generated fix...",  ← NEW
  "Timestamp": "2024-03-12T10:30:00Z"
}
```

## Configuration

### appsettings.Development.json
```json
{
  "GitHub": {
    "PersonalAccessToken": "ghp_your_github_token"
  },
  "OpenAI": {
    "ApiKey": "sk-proj-your_openai_key",
    "Model": "gpt-4o",
    "AzureEndpoint": ""
  }
}
```

### For Azure OpenAI:
```json
{
  "OpenAI": {
    "ApiKey": "your-azure-key",
    "Model": "gpt-4o",
    "AzureEndpoint": "https://your-resource.openai.azure.com/"
  }
}
```

## AI Prompt Strategy

### System Message (Persona):
- Expert DevOps engineer
- Senior C#/.NET developer
- Specializes in CI/CD debugging
- Provides structured Markdown responses

### User Message (Context):
- Git diff showing the changes
- Error log from the failed workflow
- Request for root cause and fix

### Response Format:
```markdown
## Root Cause Analysis
[Clear explanation of the failure]

## Recommended Fix
[Exact code snippet to resolve the issue]

## Explanation
[Why this fix works]
```

## Service Registration

Added to `Program.cs` (Line 22):
```csharp
builder.Services.AddScoped<IAIAssistantService, AIAssistantService>();
```

## Testing

### 1. Configure API Keys
```powershell
cd src\DevOpsAIAgent.Web
dotnet user-secrets set "OpenAI:ApiKey" "sk-proj-xxxxx"
dotnet user-secrets set "GitHub:PersonalAccessToken" "ghp_xxxxx"
```

### 2. Run the Application
```powershell
dotnet run --project src\DevOpsAIAgent.Web\DevOpsAIAgent.Web.csproj
```

### 3. Send Test Webhook
The webhook will now:
1. Fetch GitHub context ✓
2. Analyze with AI ✓
3. Broadcast to dashboard ✓

## Error Handling

The AI service gracefully handles errors:
- Returns error message if API call fails
- Logs all errors for diagnostics
- Doesn't break the webhook pipeline

## Performance Considerations

- AI analysis runs asynchronously
- Webhook returns 200 OK before AI completes (non-blocking)
- SignalR broadcast happens after AI analysis
- Typical AI response time: 2-10 seconds

## Cost Optimization

### Model Selection Impact:
- `gpt-4o`: ~$5/1M input tokens, ~$15/1M output tokens
- `gpt-4-turbo`: Similar pricing, faster
- `gpt-3.5-turbo`: ~$0.50/1M input tokens (90% cheaper)

### Token Usage per Analysis:
- System prompt: ~200 tokens
- User context (diff + logs): ~2,000-5,000 tokens
- Response: ~500-1,000 tokens
- **Total per failure: ~3,000-6,000 tokens**
- **Cost per analysis: ~$0.02-$0.05 (gpt-4o)**

## Next Steps (Phase 5)
- Create the Dashboard UI to display failures
- Add markdown rendering for AI suggestions
- Implement "Apply Fix" button to create PR
- Add historical view of past failures
