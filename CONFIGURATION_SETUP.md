# 🔧 Configuration Setup Instructions

## ⚠️ Before Running the Application

The application requires API keys to function. Follow these steps:

---

## Step 1: Configure Your `.env` File

### Location:
`D:\Devops\.env`

### Required Configuration:
```sh
# Google Gemini API Key - Get from: https://makersuite.google.com/app/apikey
GEMINI_API_KEY=AIzaSyXXXXXXXXXXXXXXXXXXXXX

# GitHub Personal Access Token - Get from: https://github.com/settings/tokens/new
GITHUB_PAT=ghp_XXXXXXXXXXXXXXXXXX

# Gemini Model (Optional - defaults to gemini-2.0-flash-exp)
GEMINI_MODEL=gemini-2.0-flash-exp
```

---

## Step 2: Get Your Gemini API Key

1. **Visit:** https://makersuite.google.com/app/apikey (or https://aistudio.google.com/apikey)
2. **Sign in** with your Google account
3. **Click:** "Create API Key"
4. **Copy** the key (starts with `AIzaSy...`)
5. **Paste** into `.env` file as `GEMINI_API_KEY=AIzaSy...`

### Free Tier Limits:
- 15 requests per minute
- 1,500 requests per day
- Perfect for development and small teams!

---

## Step 3: Get Your GitHub Personal Access Token

1. **Visit:** https://github.com/settings/tokens/new
2. **Note:** "DevOps AI Agent - Webhook Management"
3. **Expiration:** Choose your preference (90 days recommended)
4. **Select Scopes:**
   - ✅ `repo` - Full control of private repositories
   - ✅ `admin:repo_hook` - Full control of repository hooks
5. **Click:** "Generate token"
6. **Copy** the token (starts with `ghp_...`)
7. **Paste** into `.env` file as `GITHUB_PAT=ghp_...`

### ⚠️ Important:
- **Save the token immediately** - GitHub shows it only once!
- **Keep it secret** - Never commit to git
- **.env is in .gitignore** - Safe by default

---

## Step 4: Verify Configuration

### Check your `.env` file looks like this:
```sh
GEMINI_API_KEY=AIzaSyAbCdEfGhIjKlMnOpQrStUvWxYz1234567
GITHUB_PAT=ghp_xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
GEMINI_MODEL=gemini-2.0-flash-exp
```

### Test Configuration:
```powershell
# Check if .env file exists
Test-Path ".env"

# View .env file (without showing keys)
Get-Content ".env" | ForEach-Object { $_ -replace '=.*', '=***' }
```

---

## Step 5: Run the Application

```powershell
cd D:\Devops
dotnet run --project src\DevOpsAIAgent.Web\DevOpsAIAgent.Web.csproj
```

### Expected Output:
```
info: DevOpsAIAgent.Web.Services.GeminiAssistantService[0]
      Gemini Assistant Service initialized with model: gemini-2.0-flash-exp
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5120
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

---

## Step 6: Open Dashboard & Test

### Open Browser:
**http://localhost:5120**

### Test Webhook Configuration:
1. **Click:** "Load My Repositories"
2. **Expected:** List of your GitHub repos appears
3. **Select:** Any repository
4. **Click:** "Configure Webhook"
5. **Expected:** ✓ Success message

---

## 🐛 Troubleshooting

### Error: "GEMINI_API_KEY environment variable is not set"
**Solution:**
- Edit `.env` file
- Add: `GEMINI_API_KEY=AIzaSy...`
- Restart application

### Error: "GITHUB_PAT environment variable is not set"
**Solution:**
- Edit `.env` file
- Add: `GITHUB_PAT=ghp_...`
- Restart application

### Error: "SignalR is not defined"
**Solution:**
- Clear browser cache (Ctrl+Shift+Delete)
- Hard refresh (Ctrl+F5)
- Check browser console for errors

### Error: "No repositories found"
**Solution:**
- Verify GitHub token has `repo` scope
- Check you have admin access to at least one repository
- Create a test repository if needed

### Error: "Permission denied"
**Solution:**
- Verify GitHub token has `admin:repo_hook` scope
- Regenerate token with correct scopes
- Update `.env` file

### Error: "Repository not found"
**Solution:**
- Verify repository exists
- Check repository name spelling
- Ensure token has access to the repository

---

## 🔐 Security Best Practices

### ✅ DO:
- Store keys in `.env` file
- Add `.env` to `.gitignore`
- Use environment-specific keys (dev/prod)
- Rotate keys regularly
- Set token expiration

### ❌ DON'T:
- Commit `.env` to git
- Share keys in chat/email
- Use production keys in development
- Give token more permissions than needed
- Use never-expiring tokens

---

## 📊 Verification Checklist

Before running, verify:

- [ ] `.env` file exists in project root
- [ ] `GEMINI_API_KEY` is set and valid
- [ ] `GITHUB_PAT` is set and valid
- [ ] GitHub token has `repo` scope
- [ ] GitHub token has `admin:repo_hook` scope
- [ ] `.env` is in `.gitignore`
- [ ] Application builds successfully
- [ ] Port 5120 is available

---

## 🎯 Quick Test Script

Create `test-config.ps1`:
```powershell
# Test configuration
Write-Host "Testing Configuration..." -ForegroundColor Cyan

# Check .env exists
if (Test-Path ".env") {
    Write-Host "✓ .env file found" -ForegroundColor Green
    
    $envContent = Get-Content ".env"
    
    # Check Gemini key
    if ($envContent | Select-String "GEMINI_API_KEY=AIzaSy") {
        Write-Host "✓ GEMINI_API_KEY configured" -ForegroundColor Green
    } else {
        Write-Host "✗ GEMINI_API_KEY missing or invalid" -ForegroundColor Red
    }
    
    # Check GitHub token
    if ($envContent | Select-String "GITHUB_PAT=ghp_") {
        Write-Host "✓ GITHUB_PAT configured" -ForegroundColor Green
    } else {
        Write-Host "✗ GITHUB_PAT missing or invalid" -ForegroundColor Red
    }
} else {
    Write-Host "✗ .env file not found" -ForegroundColor Red
    Write-Host "  Create .env from .env.example" -ForegroundColor Yellow
}
```

Run: `.\test-config.ps1`

---

## 🚀 You're All Set!

Once configured, you'll have:
- ✅ Real-time pipeline failure monitoring
- ✅ AI-powered fix suggestions
- ✅ Automatic webhook configuration
- ✅ Smart repository selection
- ✅ Professional dark-themed dashboard

**Start the application and visit:** http://localhost:5120

**Happy monitoring!** 🎉🤖🚀
