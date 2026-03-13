# 🚀 Quick Start Guide - Webhook Configuration Feature

## ✅ Application is Running!

### 🌐 Frontend URL:
**http://localhost:5120**

---

## 🎯 New Feature: Auto-Configure Webhooks

### What You'll See:
1. **Dark-themed dashboard** loads
2. **Empty state** with a new **"Add Repository Webhook"** form
3. Two input fields:
   - **Repository Owner** (e.g., `microsoft`, `facebook`, `your-username`)
   - **Repository Name** (e.g., `vscode`, `react`, `your-repo`)
4. **"Configure Webhook"** button

---

## 📝 How to Use:

### Step 1: Enter Repository Details
In the webhook form on the dashboard:
- **Owner:** `your-github-username` (or organization name)
- **Repo:** `your-repository-name`

### Step 2: Click "Configure Webhook"
- Button shows loading spinner
- Application calls GitHub API
- Webhook is created automatically

### Step 3: See Confirmation
- ✅ **Success:** Green message shows "Webhook created successfully"
- ✗ **Error:** Red message shows the issue (e.g., "Permission denied")

---

## 🔧 Configuration Requirements:

### Set Your GitHub Token in `.env`:
```bash
GEMINI_API_KEY=AIzaSyXXXXXXXXXXXXXXXXXXXXX
GITHUB_PAT=ghp_XXXXXXXXXXXXXXXXXX
GEMINI_MODEL=gemini-2.0-flash-exp
```

### GitHub Token Scopes Required:
- ✅ `repo` - Full repository access
- ✅ `admin:repo_hook` - Manage webhooks

**Get token:** https://github.com/settings/tokens/new
- Select scopes: `repo` and `admin:repo_hook`
- Generate token
- Copy to `.env` file

---

## 🧪 Test the Feature:

### Example 1: Configure Your Own Repo
```
Owner: your-username
Repo: your-test-repo
Click: Configure Webhook
Expected: ✓ Webhook created successfully
```

### Example 2: Verify on GitHub
1. Go to: `https://github.com/{owner}/{repo}/settings/hooks`
2. You should see:
   - **Payload URL:** `http://localhost:5120/api/webhooks/github`
   - **Content type:** application/json
   - **Events:** Workflow runs
   - **Status:** ✓ Active

### Example 3: Test End-to-End
1. Configure webhook via dashboard
2. Make a code change that breaks CI
3. Push to GitHub
4. Watch failure appear in dashboard automatically!

---

## 🎨 UI Features:

### Visual States:
- **Normal:** Blue configure button
- **Loading:** Spinner with "Configuring..." text
- **Success:** Green status message
- **Error:** Red status message with explanation

### Smart Features:
- ✅ **Duplicate Detection** - Won't create multiple webhooks
- ✅ **Input Validation** - Checks for empty fields
- ✅ **Auto-clear** - Clears form after success
- ✅ **Error Messages** - Clear, actionable feedback

---

## 🔐 Security & Permissions:

### What You Need:
1. **GitHub Personal Access Token** with correct scopes
2. **Admin access** to the target repository
3. **GITHUB_PAT** configured in `.env` file

### Common Permission Issues:

| Error | Cause | Solution |
|-------|-------|----------|
| "Permission denied" | Not a repo admin | Request admin access or use your own repo |
| "Repository not found" | Wrong name or private repo | Verify name and ensure token has repo access |
| "GITHUB_PAT not set" | Missing token | Add token to `.env` file |

---

## 📊 What Happens Behind the Scenes:

```
1. User clicks "Configure Webhook"
        ↓
2. JavaScript validates input
        ↓
3. POST request to /api/webhooks/configure
        ↓
4. WebhookConfigurationService initializes GitHub client
        ↓
5. Check if webhook already exists
        ↓
6. Create new webhook with config:
   - URL: http://localhost:5120/api/webhooks/github
   - Content-Type: application/json
   - Events: workflow_run
   - Active: true
        ↓
7. Return result to frontend
        ↓
8. Display success/error message
        ↓
9. Auto-clear form on success
```

---

## 🧪 API Testing (Without UI):

### Using PowerShell:
```powershell
# Configure webhook
Invoke-RestMethod -Uri "http://localhost:5120/api/webhooks/configure" `
    -Method Post `
    -ContentType "application/json" `
    -Body '{"owner":"your-username","repo":"your-repo"}'

# List webhooks
Invoke-RestMethod -Uri "http://localhost:5120/api/webhooks/list/your-username/your-repo"

# Delete webhook
Invoke-RestMethod -Uri "http://localhost:5120/api/webhooks/your-username/your-repo/123456" `
    -Method Delete
```

### Using curl:
```bash
# Configure webhook
curl -X POST http://localhost:5120/api/webhooks/configure \
  -H "Content-Type: application/json" \
  -d '{"owner":"your-username","repo":"your-repo"}'

# List webhooks
curl http://localhost:5120/api/webhooks/list/your-username/your-repo

# Delete webhook
curl -X DELETE http://localhost:5120/api/webhooks/your-username/your-repo/123456
```

---

## 🎬 Demo Script:

### For Stakeholders:
1. **Show the form:** "Here's our webhook configuration feature"
2. **Enter repo:** "I'll enter one of our repositories"
3. **Click configure:** "Watch how it automatically sets up the webhook"
4. **Show success:** "Done! The webhook is now active"
5. **Verify on GitHub:** "Let's verify it was created on GitHub"
6. **Trigger failure:** "Now when CI fails, we'll see it here instantly"

---

## 🚀 You're Ready!

### Quick Checklist:
- ✅ Application running on http://localhost:5120
- ✅ `.env` file configured with `GITHUB_PAT` and `GEMINI_API_KEY`
- ✅ GitHub token has `repo` and `admin:repo_hook` scopes
- ✅ You have admin access to test repository

### Next Steps:
1. **Open browser:** http://localhost:5120
2. **Enter your repository details**
3. **Click "Configure Webhook"**
4. **Watch it work!** 🎉

---

**The webhook auto-configuration feature is live and ready to use!** 🚀🔧
