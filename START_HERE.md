# ✅ APPLICATION IS RUNNING!

## 🌐 Frontend URL:
# **http://localhost:5120**

---

## ⚙️ BEFORE USING - Configure API Keys!

The errors you saw indicate the `.env` file needs configuration.

### Quick Setup (2 minutes):

#### 1. Edit `.env` file:
```powershell
notepad .env
```

#### 2. Add your keys:
```sh
GEMINI_API_KEY=AIzaSyXXXXXXXXXXXXXXXXXXXXX
GITHUB_PAT=ghp_XXXXXXXXXXXXXXXXXX
GEMINI_MODEL=gemini-2.0-flash-exp
```

#### 3. Get Gemini Key (Free):
- Visit: https://aistudio.google.com/apikey
- Sign in with Google
- Click "Create API Key"
- Copy the key

#### 4. Get GitHub Token:
- Visit: https://github.com/settings/tokens/new
- Check: ✅ `repo` and ✅ `admin:repo_hook`
- Generate token
- Copy the token

#### 5. Save & Restart:
```powershell
# Stop the current app (in the other terminal press Ctrl+C)
# Then restart:
dotnet run --project src\DevOpsAIAgent.Web\DevOpsAIAgent.Web.csproj
```

---

## 🎯 What You'll See After Configuration:

### 1. Open: http://localhost:5120

### 2. Initial Dashboard:
```
┌──────────────────────────────────────────────────┐
│  ● DevOps AI Agent - Live CI/CD Monitor ● Connected │
├──────────────┬───────────────────────────────────┤
│ RECENT       │         🔧                        │
│ FAILURES (0) │                                   │
│              │  Waiting for CI/CD Webhooks...    │
│              │                                   │
│              │  ┌─────────────────────────────┐ │
│              │  │ + Configure Webhook         │ │
│              │  │                             │ │
│              │  │ [Load My Repositories]      │ │
│              │  │                             │ │
│              │  │ 📋 Required Permissions:    │ │
│              │  │  • Admin access             │ │
│              │  │  • repo scope               │ │
│              │  │  • admin:repo_hook scope    │ │
│              │  └─────────────────────────────┘ │
└──────────────┴───────────────────────────────────┘
```

### 3. Click "Load My Repositories":
- Your GitHub repos load automatically
- Only shows repos where you have admin access
- Displays webhook configuration status

### 4. Repository List Appears:
```
Search Repositories
[Type to filter..._______________]

Select Repository (5 available)
┌─────────────────────────────────┐
│ username/my-app                 │
│ My awesome application          │
│ [PRIVATE] [✓ WEBHOOK CONFIGURED]│
├─────────────────────────────────┤
│ username/test-project           │
│ Test repository for CI/CD       │
│ [PRIVATE]                       │
├─────────────────────────────────┤
│ orgname/company-api             │
│ Company API service             │
└─────────────────────────────────┘
```

### 5. Select a Repository:
- Click on any repo
- Webhook name auto-generates
- Configure button enables

### 6. Configure Webhook:
- Click "Configure Webhook"
- Success message appears
- Badge updates to "✓ WEBHOOK CONFIGURED"

---

## 🎬 Complete Demo Flow:

```
1. Configure .env file (2 min)
        ↓
2. Start application
        ↓
3. Open http://localhost:5120
        ↓
4. Status shows "Connected" ✓
        ↓
5. Click "Load My Repositories"
        ↓
6. See all your GitHub repos
        ↓
7. Search/filter if needed
        ↓
8. Click to select a repo
        ↓
9. Click "Configure Webhook"
        ↓
10. ✓ Success! Webhook is live
        ↓
11. Push code that breaks CI
        ↓
12. Watch failure + AI fix appear instantly!
```

---

## 🚨 Common Issues & Fixes:

### Issue: "SignalR is not defined"
**Fix:** Clear browser cache (Ctrl+Shift+Delete), hard refresh (Ctrl+F5)

### Issue: "GITHUB_PAT not set"
**Fix:** 
```powershell
# Edit .env
notepad .env

# Add:
GITHUB_PAT=ghp_your_token_here

# Restart app
```

### Issue: "GEMINI_API_KEY not set"
**Fix:**
```powershell
# Edit .env
notepad .env

# Add:
GEMINI_API_KEY=AIzaSy_your_key_here

# Restart app
```

### Issue: "No repositories found"
**Fix:**
- Verify GitHub token has `repo` scope
- Create a test repository on GitHub
- Ensure token hasn't expired

### Issue: "Permission denied"
**Fix:**
- Verify GitHub token has `admin:repo_hook` scope
- Regenerate token with correct scopes
- Update `.env` file

---

## ✨ What Works Without Configuration:

### With NO API keys:
- ✅ Dashboard loads
- ✅ SignalR connects
- ❌ Can't load repositories (need GITHUB_PAT)
- ❌ Can't configure webhooks (need GITHUB_PAT)
- ❌ Can't analyze failures (need GEMINI_API_KEY)

### With GITHUB_PAT only:
- ✅ Dashboard loads
- ✅ SignalR connects
- ✅ Load repositories
- ✅ Configure webhooks
- ✅ Receive failure notifications
- ❌ AI analysis (need GEMINI_API_KEY)

### With BOTH keys:
- ✅ Everything works perfectly! 🎉

---

## 🎯 Quick Start Checklist:

- [ ] `.env` file created
- [ ] `GEMINI_API_KEY` added to `.env`
- [ ] `GITHUB_PAT` added to `.env`
- [ ] GitHub token has `repo` scope
- [ ] GitHub token has `admin:repo_hook` scope
- [ ] Application restarted after .env changes
- [ ] Browser opened to http://localhost:5120
- [ ] Status shows "Connected" (green)
- [ ] "Load My Repositories" button clicked
- [ ] Repositories loaded successfully
- [ ] Repository selected from list
- [ ] Webhook configured successfully
- [ ] Ready to monitor CI/CD! 🚀

---

## 📱 Next Steps:

1. **Configure .env** (see above)
2. **Restart application**
3. **Open http://localhost:5120**
4. **Click "Load My Repositories"**
5. **Select and configure webhook**
6. **Test with a failing CI/CD build**
7. **Watch AI analyze and suggest fixes!**

---

**Need help?** See `CONFIGURATION_SETUP.md` for detailed instructions.

**URL:** http://localhost:5120 🚀
