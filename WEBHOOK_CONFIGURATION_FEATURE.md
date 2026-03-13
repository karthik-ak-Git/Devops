# Webhook Auto-Configuration Feature

## Overview
Users can now automatically configure GitHub webhooks directly from the dashboard without manually going to GitHub settings.

## How It Works

### User Flow:
1. User opens the dashboard (empty state visible)
2. User enters **Repository Owner** (e.g., `microsoft`)
3. User enters **Repository Name** (e.g., `vscode`)
4. User clicks **"Configure Webhook"** button
5. System automatically creates webhook on GitHub
6. User receives confirmation message

### Backend Flow:
```
Dashboard Form
      ↓
POST /api/webhooks/configure
      ↓
WebhookConfigurationController
      ↓
IWebhookConfigurationService
      ↓
GitHub API (Octokit)
      ↓
Create Repository Hook
      ↓
Return Success/Failure
      ↓
Update Dashboard UI
```

## API Endpoints

### Configure Webhook
```http
POST /api/webhooks/configure
Content-Type: application/json

{
  "owner": "microsoft",
  "repo": "vscode"
}
```

**Response (Success):**
```json
{
  "success": true,
  "message": "Webhook created successfully (ID: 123456)",
  "webhookId": 123456,
  "webhookUrl": "http://localhost:5120/api/webhooks/github"
}
```

**Response (Already Exists):**
```json
{
  "success": true,
  "message": "Webhook already exists (ID: 123456)",
  "webhookId": 123456,
  "webhookUrl": "http://localhost:5120/api/webhooks/github"
}
```

**Response (Error):**
```json
{
  "success": false,
  "message": "Repository 'owner/repo' not found. Verify the repository exists and you have admin access."
}
```

### List Webhooks
```http
GET /api/webhooks/list/{owner}/{repo}
```

**Response:**
```json
{
  "success": true,
  "count": 2,
  "webhooks": [
    {
      "id": 123456,
      "name": "web",
      "active": true,
      "events": ["workflow_run"],
      "url": "http://localhost:5120/api/webhooks/github",
      "createdAt": "2024-03-12T10:00:00Z",
      "updatedAt": "2024-03-12T10:00:00Z"
    }
  ]
}
```

### Delete Webhook
```http
DELETE /api/webhooks/{owner}/{repo}/{hookId}
```

## Components Created

### 1. Service Interface (`IWebhookConfigurationService.cs`)
```csharp
public interface IWebhookConfigurationService
{
    Task<WebhookConfigurationResult> CreateWebhookAsync(string owner, string repo, string webhookUrl);
    Task<IReadOnlyList<RepositoryHook>> ListWebhooksAsync(string owner, string repo);
    Task<bool> DeleteWebhookAsync(string owner, string repo, long hookId);
}
```

### 2. Service Implementation (`WebhookConfigurationService.cs`)
- Uses Octokit GitHub client
- Reads `GITHUB_PAT` from environment variable
- Checks for existing webhooks before creating
- Creates webhook with `workflow_run` event
- Comprehensive error handling

### 3. API Controller (`WebhookConfigurationController.cs`)
- `POST /api/webhooks/configure` - Create webhook
- `GET /api/webhooks/list/{owner}/{repo}` - List webhooks
- `DELETE /api/webhooks/{owner}/{repo}/{hookId}` - Delete webhook

### 4. Dashboard UI Updates (`Index.cshtml`)
- Webhook configuration form in empty state
- Input fields for owner and repo
- Configure button with loading state
- Status message display (success/error)

### 5. JavaScript Functions
- `configureWebhook()` - Handles form submission
- `showWebhookStatus()` - Displays success/error messages
- Input validation
- Loading state management

## UI Features

### Form Design
- **Dark themed** matching the dashboard
- **Two input fields:**
  - Repository Owner/Organization
  - Repository Name
- **Status message area:**
  - Green background for success
  - Red background for errors
- **Configure button:**
  - Shows loading spinner during API call
  - Disabled while processing
  - Returns to normal after completion

### User Experience
1. **Input Validation** - Checks for empty fields
2. **Loading State** - Button shows spinner during API call
3. **Visual Feedback** - Success/error messages with color coding
4. **Auto-clear** - Inputs clear after successful configuration
5. **Error Messages** - Clear, actionable error descriptions

## Requirements

### GitHub Token Permissions
Your `GITHUB_PAT` must have these scopes:
- ✅ `repo` (Full control of private repositories)
- ✅ `admin:repo_hook` (Full control of repository hooks)

### Repository Access
- User must have **admin access** to the repository
- Repository must exist and be accessible

## Testing

### Test Webhook Configuration
1. Start the application
2. Open dashboard: http://localhost:5120
3. In the webhook form, enter:
   - Owner: `your-username`
   - Repo: `your-test-repo`
4. Click "Configure Webhook"
5. Check for success message

### Verify Webhook on GitHub
1. Go to GitHub: `https://github.com/{owner}/{repo}/settings/hooks`
2. You should see a new webhook:
   - Payload URL: `http://localhost:5120/api/webhooks/github`
   - Content type: `application/json`
   - Events: `Workflow runs`
   - Active: ✓

### Test with Real Repository
```powershell
# Make a change that will fail CI
git add .
git commit -m "Test webhook"
git push

# Watch the dashboard - failure should appear automatically!
```

## Error Handling

### Common Errors & Solutions

| Error | Cause | Solution |
|-------|-------|----------|
| "Repository not found" | Repo doesn't exist or wrong name | Verify repo name and owner |
| "Permission denied" | No admin access | Grant admin access or use personal repo |
| "GITHUB_PAT not set" | Missing environment variable | Configure in .env file |
| "Webhook already exists" | Webhook already configured | This is OK - webhook is ready to use |

## Security Considerations

1. **Admin Access Required** - Only repo admins can create webhooks
2. **Token Validation** - Service checks for GITHUB_PAT before initializing
3. **Input Validation** - Owner and repo fields validated
4. **HTTPS Enforcement** - Production should use HTTPS URLs
5. **Error Messages** - Don't expose sensitive information

## Cost & Rate Limits

- **GitHub API Rate Limit**: 5,000 requests/hour (authenticated)
- **Webhook Creation**: Counts as 1 API request
- **List Webhooks**: Counts as 1 API request
- **No GitHub charges** - Webhooks are free

## Production Considerations

### Public URL Required
For GitHub to send webhooks, you need a publicly accessible URL:

1. **Deploy to Cloud:**
   - Azure App Service
   - AWS Elastic Beanstalk
   - Google Cloud Run
   - Heroku

2. **Use Tunnel Service (Development):**
   ```powershell
   ngrok http 5120
   ```
   Then update webhook URL in the form

### HTTPS Enforcement
- GitHub recommends HTTPS webhooks
- Production deployment should use SSL certificate
- Use `insecure_ssl: "0"` for HTTPS only

### Webhook Secret (Future Enhancement)
Consider adding HMAC validation:
```csharp
// Add secret to webhook config
{ "secret", "your-webhook-secret" }

// Validate incoming webhooks
var signature = Request.Headers["X-Hub-Signature-256"];
// Validate HMAC...
```

## Future Enhancements

- [ ] Add webhook secret support
- [ ] Show list of configured webhooks in sidebar
- [ ] Delete webhook functionality in UI
- [ ] Test webhook button (send test event)
- [ ] Webhook health status monitoring
- [ ] Bulk webhook configuration for multiple repos
- [ ] Export/import webhook configurations
- [ ] Webhook delivery history

## Files Modified/Created

### New Files:
- `src\DevOpsAIAgent.Web\Services\IWebhookConfigurationService.cs`
- `src\DevOpsAIAgent.Web\Services\WebhookConfigurationService.cs`
- `src\DevOpsAIAgent.Web\Controllers\Api\WebhookConfigurationController.cs`

### Modified Files:
- `src\DevOpsAIAgent.Web\Program.cs` - Registered webhook service
- `src\DevOpsAIAgent.Web\Views\Home\Index.cshtml` - Added configuration form and JS

## Code Snippets

### Program.cs Registration
```csharp
// Register Webhook Configuration Service
builder.Services.AddScoped<IWebhookConfigurationService, WebhookConfigurationService>();
```

### JavaScript Usage
```javascript
// Call from button click
await configureWebhook();

// Or programmatically
const response = await fetch("/api/webhooks/configure", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ owner: "microsoft", repo: "vscode" })
});
```

## Success!

Users can now configure webhooks directly from the dashboard without leaving the application! 🎉
