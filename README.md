# DevOps AI Agent

[![Build Status](https://img.shields.io/badge/Build-Passing-success.svg)]()
[![.NET 10](https://img.shields.io/badge/.NET-10.0-purple.svg)](https://dotnet.microsoft.com/download/dotnet/10.0)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

A comprehensive DevOps solution combining GitHub webhook integration, CI/CD pipeline monitoring, SSH management, and AI-powered failure analysis.

## 🎯 Project Overview

This is a full-stack DevOps AI Agent system consisting of:
- **Web API** - ASP.NET Core REST API with GitHub webhook support
- **Desktop App** - WPF Windows application with real-time dashboard
- **Core Services** - Shared business logic and infrastructure
- **GitHub Integration** - Secure webhook handling with HMAC SHA256 verification
- **AI Analysis** - Gemini AI integration for automated failure analysis

![DevOps AI Agent Architecture](docs/architecture.png)

## 🚀 How to Run

### Prerequisites

- **Windows 10/11** (64-bit) or **Linux/macOS**
- **[.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)** or later
- **Git** for cloning the repository
- **GitHub Personal Access Token** (for webhook functionality)
- **Gemini API Key** (for AI analysis features)
- **WebView2 Runtime** (Windows only, usually pre-installed)

### Step 1: Clone the Repository

```bash
git clone https://github.com/karthik-ak-Git/Devops.git
cd Devops
```

### Step 2: Configure Environment Variables

Create a `.env` file in the root directory:

```env
# GitHub Configuration
GITHUB_PAT=ghp_your_github_personal_access_token_here
GITHUB_WEBHOOK_SECRET=your_webhook_secret_here

# AI Configuration (Gemini)
GEMINI_API_KEY=your_gemini_api_key_here
```

**How to get these tokens:**

#### GitHub Personal Access Token (PAT)
1. Go to GitHub Settings → Developer settings → Personal access tokens → Tokens (classic)
2. Click "Generate new token"
3. Select scopes: `repo`, `admin:repo_hook`
4. Copy the token and paste into `.env`

#### Webhook Secret
Generate a secure secret:
```powershell
# PowerShell
$secret = [Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes((New-Guid).ToString() + (New-Guid).ToString()))
Write-Host $secret
```

Add to `.env`: `GITHUB_WEBHOOK_SECRET=<generated_secret>`

#### Gemini API Key
1. Go to [Google AI Studio](https://aistudio.google.com/app/apikey)
2. Click "Get API Key"
3. Create new API key
4. Copy and paste into `.env`

### Step 3: Run the Web API

```bash
# From the root directory
cd src/DevOpsAIAgent.Web

# Run the web server
dotnet run

# Server will start on: https://localhost:5001 or http://localhost:5000
```

**Expected Output:**
```
✅ Loaded .env from: D:\Devops\.env
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
```

### Step 4: Run the Desktop Application

In a **new terminal**, run:

```bash
cd src/DevOpsAIAgent.App

dotnet run
```

**Dashboard will:**
- Connect to the Web API via SignalR
- Display real-time pipeline failure alerts
- Show AI-generated fix suggestions
- Update in real-time as webhooks arrive

### Step 5: Set Up GitHub Webhooks

#### Option A: Using the API

```bash
# Get list of your repositories
curl http://localhost:5000/api/webhooks/repositories

# Create a webhook for a repository
curl -X POST http://localhost:5000/api/webhooks/configure \
  -H "Content-Type: application/json" \
  -d '{"fullName": "owner/repository"}'
```

#### Option B: Manual Setup (GitHub UI)

1. Go to Repository Settings → Webhooks
2. Click "Add webhook"
3. **Payload URL:** `https://yourdomain.com/api/webhooks/github`
4. **Content Type:** `application/json`
5. **Secret:** Paste your `GITHUB_WEBHOOK_SECRET` value
6. **Events:** Select "Workflow runs"
7. Click "Add webhook"

### Step 6: Test the System

#### Test 1: Health Check
```bash
curl http://localhost:5000/api/webhooks/github/health
```

Expected response:
```json
{
  "status": "healthy",
  "service": "GitHub Webhook Receiver",
  "timestamp": "2025-01-17T10:30:00Z"
}
```

#### Test 2: Create a Webhook
```bash
curl -X POST http://localhost:5000/api/webhooks/configure \
  -H "Content-Type: application/json" \
  -d '{"fullName": "your-username/test-repo"}'
```

#### Test 3: Send Test Webhook

See: `test-webhook.ps1` in the root directory for manual webhook testing.

```bash
# Run the test script
.\test-webhook.ps1
```

### Step 7: Monitor Dashboard

1. Open the Desktop Application
2. Watch for real-time alerts when workflows fail
3. See AI-generated suggestions automatically

---

## 🎯 Development Workflow

### Build All Projects

```bash
# From root directory
dotnet build
```

### Build Specific Project

```bash
# Web API
dotnet build src/DevOpsAIAgent.Web

# Desktop App
dotnet build src/DevOpsAIAgent.App

# Core Services
dotnet build src/DevOpsAIAgent.Core

# Data Layer
dotnet build src/DevOpsAIAgent.Data
```

### Run Tests

```bash
# All tests
dotnet test

# Specific test project
dotnet test src/DevOpsAIAgent.Web.Tests
```

### Debug in Visual Studio

1. Open `DevOpsAIAgent.sln` in Visual Studio
2. Set startup project: `DevOpsAIAgent.Web` or `DevOpsAIAgent.App`
3. Press `F5` to debug

---

## 📊 Project Structure

```
Devops/
├── src/
│   ├── DevOpsAIAgent.Web/           # ASP.NET Core Web API
│   │   ├── Controllers/              # API endpoints
│   │   ├── Services/                 # Business logic
│   │   │   ├── WebhookSecurityService.cs
│   │   │   ├── GitHubAnalysisService.cs
│   │   │   └── AIAssistantService.cs
│   │   └── Program.cs                # Configuration
│   │
│   ├── DevOpsAIAgent.App/            # WPF Desktop Application
│   │   ├── ViewModels/               # MVVM ViewModels
│   │   ├── Views/                    # XAML Views
│   │   └── App.xaml.cs               # Startup
│   │
│   ├── DevOpsAIAgent.Core/           # Shared Services
│   │   ├── Services/                 # Core business logic
│   │   └── Models/                   # Domain models
│   │
│   └── DevOpsAIAgent.Data/           # Data Layer
│       ├── ApplicationDbContext.cs    # EF Core context
│       └── Migrations/                # Database migrations
│
├── docs/                             # Documentation
│   ├── WEBHOOK_IMPLEMENTATION_GUIDE.md
│   ├── WEBHOOK_SECRET_SETUP.md
│   └── README_WEBHOOK_DOCS.md
│
├── .env                              # Environment variables (create this)
├── DevOpsAIAgent.sln                 # Solution file
└── README.md                         # This file
```

---

## 🔗 API Endpoints

### Webhooks
- **POST** `/api/webhooks/github` - Receive GitHub webhooks
- **GET** `/api/webhooks/repositories` - List user repositories
- **POST** `/api/webhooks/configure` - Create webhook
- **GET** `/api/webhooks/github/health` - Health check

### SignalR Hubs
- **Hub:** `/api/hubs/dashboard` - Real-time dashboard updates

---

## Features

### SSH Connections
- **Host Management** - Store and organize SSH connections with groups
- **Embedded Terminal** - Full-featured terminal with xterm.js (vim, tmux, htop all work)
- **Multiple Tabs & Split Panes** - Work with multiple sessions side by side
- **Secure Credential Storage** - Passwords encrypted with Windows DPAPI
- **SSH Key Support** - SSH Agent, private key files, or password authentication
- **SSH Key Management** - Generate, import, and manage SSH keys with passphrase support
- **PPK Import Wizard** - Batch convert PuTTY keys to OpenSSH format
- **SFTP Browser** - Graphical file transfer with drag-and-drop
- **Port Forwarding** - Local and remote port forwarding profiles
- **Jump Hosts** - ProxyJump support for bastion/jump host connections
- **Import/Export** - Import from SSH config or PuTTY, backup to cloud

### Serial Port Connections
- **COM Port Support** - Connect to serial devices (routers, switches, embedded systems)
- **Full Configuration** - Baud rate, data bits, stop bits, parity, flow control
- **DTR/RTS Control** - Toggle hardware signals for device reset/boot modes
- **Local Echo** - Optional local character echo for half-duplex devices
- **Quick Connect** - Enumerate and connect to available COM ports instantly
- **Save & Organize** - Store serial port configurations alongside SSH hosts

### General
- **Modern UI** - Dark theme with Fluent Design (WPF-UI)
- **Session Recording** - Record and playback terminal sessions (ASCIINEMA format)

## Quick Start

### Prerequisites

- Windows 10/11 (64-bit)
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- WebView2 Runtime (pre-installed on Windows 10/11)

### Build & Run

```bash
# Clone the repository
git clone https://github.com/tomertec/sshmanager.git
cd sshmanager

# Build
dotnet build SshManager.sln

# Run
dotnet run --project src/SshManager.App/SshManager.App.csproj
```

### Download Release

Check the [Releases](https://github.com/tomertec/sshmanager/releases) page for pre-built binaries.

## Usage

1. **Add a host**: Click the **+** button and enter connection details
2. **Connect**: Double-click a host or press Enter
3. **Organize**: Create groups to organize your hosts
4. **Split panes**: Right-click a tab to split horizontally/vertically

For detailed usage instructions, see the [Getting Started Guide](docs/GETTING_STARTED.md).

## 📚 Documentation

### Getting Started
- **[00_START_HERE_WEBHOOKS.md](00_START_HERE_WEBHOOKS.md)** - Master webhook implementation summary
- **[WEBHOOK_QUICKSTART.md](WEBHOOK_QUICKSTART.md)** - 5-minute quick start guide

### Webhook Documentation
- **[WEBHOOK_IMPLEMENTATION_GUIDE.md](WEBHOOK_IMPLEMENTATION_GUIDE.md)** - Complete webhook feature documentation
- **[WEBHOOK_SECRET_SETUP.md](WEBHOOK_SECRET_SETUP.md)** - Webhook secret generation and management
- **[WEBHOOK_VISUAL_OVERVIEW.md](WEBHOOK_VISUAL_OVERVIEW.md)** - Architecture diagrams and visual explanations
- **[README_WEBHOOK_DOCS.md](README_WEBHOOK_DOCS.md)** - Webhook documentation index

### Technical Reference
- **[CODE_CHANGES_SUMMARY.md](CODE_CHANGES_SUMMARY.md)** - Technical code changes and implementation details
- **[WEBHOOK_DOCUMENTATION_INDEX.md](WEBHOOK_DOCUMENTATION_INDEX.md)** - Master documentation index with search
- **[IMPLEMENTATION_COMPLETION_REPORT.md](IMPLEMENTATION_COMPLETION_REPORT.md)** - Final implementation report

---

## Technology Stack

| Component | Technology |
|-----------|------------|
| Backend | .NET 10, ASP.NET Core |
| Desktop | .NET 10, WPF |
| Database | SQLite via EF Core |
| API Security | HMAC SHA256, JWT |
| Real-time | SignalR |
| AI | Google Gemini API |
| GitHub | Octokit, GitHub REST API |
| UI | WPF-UI (Fluent Design) |

## Building from Source

```bash
# Debug build
dotnet build DevOpsAIAgent.sln

# Release build
dotnet build DevOpsAIAgent.sln -c Release

# Run tests
dotnet test

# Publish Web API
dotnet publish src/DevOpsAIAgent.Web/DevOpsAIAgent.Web.csproj -c Release -o ./publish/web

# Publish Desktop App
dotnet publish src/DevOpsAIAgent.App/DevOpsAIAgent.App.csproj -c Release -o ./publish/app
```

---

## 🐛 Troubleshooting

### Issue: ".env file not found"
**Solution:**
```bash
# Create .env in the root directory
New-Item -Path ".env" -ItemType File
# Add your configuration
```

### Issue: "GITHUB_WEBHOOK_SECRET not configured"
**Solution:**
- This is a warning during development
- For production, you MUST set `GITHUB_WEBHOOK_SECRET` in `.env`
- See Step 2 above for how to generate and configure

### Issue: "Cannot connect to API"
**Solution:**
1. Verify Web API is running on port 5000/5001
2. Check firewall settings
3. Verify `.env` is loaded (should see ✅ message)
4. Check application logs for errors

### Issue: "Webhook signature verification failed"
**Solution:**
- Verify `GITHUB_WEBHOOK_SECRET` matches the value in GitHub webhook settings
- Check that the secret was properly set in the `.env` file
- See WEBHOOK_SECRET_SETUP.md for detailed troubleshooting

---

## 📞 Support

For detailed documentation on specific features:
- **Webhook Setup:** See [WEBHOOK_QUICKSTART.md](WEBHOOK_QUICKSTART.md)
- **Webhook Security:** See [WEBHOOK_IMPLEMENTATION_GUIDE.md](WEBHOOK_IMPLEMENTATION_GUIDE.md)
- **API Reference:** See [README_WEBHOOK_DOCS.md](README_WEBHOOK_DOCS.md)
- **Troubleshooting:** See [WEBHOOK_IMPLEMENTATION_GUIDE.md](WEBHOOK_IMPLEMENTATION_GUIDE.md) - Troubleshooting section

---

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- [Octokit](https://github.com/octokit/octokit.net) - GitHub API for .NET
- [SSH.NET](https://github.com/sshnet/SSH.NET) - SSH library for .NET
- [WPF-UI](https://github.com/lepoco/wpfui) - Modern WPF controls
- [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet) - MVVM framework
- [Google Gemini API](https://ai.google.dev/) - AI analysis

