# DevOps AI Agent

[![Build Status](https://img.shields.io/badge/Build-Passing-success.svg)]()
[![.NET 10](https://img.shields.io/badge/.NET-10.0-purple.svg)](https://dotnet.microsoft.com/download/dotnet/10.0)
[![React 19](https://img.shields.io/badge/React-19+-blue.svg)](https://reactjs.org/)
[![TypeScript](https://img.shields.io/badge/TypeScript-5.9-blue.svg)](https://www.typescriptlang.org/)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

A modern .NET API + React TypeScript application for DevOps analysis powered by multiple Large Language Models (LLMs), featuring GitHub webhook integration, CI/CD pipeline monitoring, and AI-powered code review and analysis.

## 🚀 Features

- **Multiple LLM Providers**: Support for Google Gemini, OpenRouter, and Ollama with automatic fallback
- **DevOps Analysis**: AI-powered insights for deployments, CI/CD pipelines, and infrastructure
- **Code Review**: Automated code review with security, performance, and best practices analysis
- **GitHub Integration**: Secure webhook handling with HMAC SHA256 verification
- **Modern Tech Stack**: .NET 10, React 19, TypeScript, Tailwind CSS, Vite
- **Health Monitoring**: Real-time health checks for all LLM providers
- **Responsive UI**: Mobile-friendly interface with modern design

## 🏗️ Architecture

```
src/
├── DevOpsAIAgent.Core/          # Domain entities and interfaces
├── DevOpsAIAgent.Data/          # Entity Framework and repositories
├── DevOpsAIAgent.API/           # Web API backend (.NET 10)
└── DevOpsAIAgent.Client/        # React TypeScript frontend
```

### Backend (.NET 10 API)
- **Clean Architecture**: Separation of concerns with Core, Data, and API layers
- **Multiple LLM Providers**: Gemini, OpenRouter, Ollama integrations with fallback
- **Health Checks**: Built-in health monitoring for all services
- **OpenAPI/Swagger**: Comprehensive API documentation
- **Logging**: Structured logging with Serilog

### Frontend (React 19 TypeScript)
- **React 19**: Latest React with TypeScript
- **Vite**: Fast build tool and dev server
- **Tailwind CSS**: Utility-first CSS framework
- **React Query**: Data fetching and caching
- **React Router**: Client-side routing

## 📊 Project Structure

```
DevOpsAIAgent/
├── src/
│   ├── DevOpsAIAgent.Core/           # Core domain models and interfaces
│   ├── DevOpsAIAgent.Data/           # Data access layer (EF Core)
│   ├── DevOpsAIAgent.API/            # Web API backend (.NET 10)
│   │   ├── Controllers/              # API controllers
│   │   │   ├── BaseApiController.cs
│   │   │   └── GitHubController.cs
│   │   ├── appsettings.json          # Configuration
│   │   └── Program.cs                # API startup
│   │
│   ├── DevOpsAIAgent.Client/         # React TypeScript frontend
│   │   ├── src/
│   │   │   ├── components/           # React components
│   │   │   ├── pages/                # Route pages
│   │   │   ├── services/             # API services
│   │   │   └── App.tsx               # Main app component
│   │   ├── package.json              # Node.js dependencies
│   │   └── vite.config.ts            # Vite configuration
│   │
│   └── DevOpsAIAgent.Web/            # Legacy MVC Web (being migrated)
│
├── docker/
│   ├── Dockerfile.api               # API container configuration
│   ├── Dockerfile.client            # Client container configuration
│   ├── docker-compose.yml           # Multi-container orchestration
│   ├── nginx.conf                   # Nginx configuration
│   └── init.sql                     # Database initialization
│
├── DevOpsAIAgent.sln                # Visual Studio solution file
└── README.md                        # This file
```

## 🚀 Setup Instructions

### Prerequisites
- **.NET 10 SDK** or later
- **Node.js 18+** and npm
- **(Optional)** Ollama for local models
- **Git** for cloning the repository

### 1. Clone and Navigate
```bash
git clone https://github.com/karthik-ak-Git/Devops.git
cd Devops
```

### 2. Backend Setup (.NET API)

#### Install Dependencies
```bash
cd src/DevOpsAIAgent.API
dotnet restore
```

#### Configure LLM Providers
Edit `appsettings.json` or use environment variables:

```json
{
  "LlmProviders": {
    "DefaultProvider": "Gemini",
    "EnableFallback": true,
    "Gemini": {
      "ApiKey": "your-gemini-api-key",
      "Enabled": true,
      "Model": "gemini-1.5-pro"
    },
    "OpenRouter": {
      "ApiKey": "your-openrouter-api-key",
      "Enabled": false,
      "Model": "anthropic/claude-3.5-sonnet"
    },
    "Ollama": {
      "BaseUrl": "http://localhost:11434",
      "Model": "llama3.1",
      "Enabled": false
    }
  }
}
```

#### Environment Variables (Alternative)
Create a `.env` file in the API project root:
```env
# LLM Provider API Keys
GEMINI_API_KEY=your_gemini_api_key_here
OPENROUTER_API_KEY=your_openrouter_api_key_here

# GitHub Configuration (Optional)
GITHUB_PAT=ghp_your_github_personal_access_token_here
GITHUB_WEBHOOK_SECRET=your_webhook_secret_here

# Database Configuration
ConnectionStrings__DefaultConnection=Data Source=devops_ai_agent.db
```

### 3. Frontend Setup (React Client)

```bash
cd src/DevOpsAIAgent.Client
npm install
```

Create `.env` file:
```env
VITE_API_BASE_URL=http://localhost:5000/api
VITE_ENV=development
```

## 🚀 Running the Application

### Start Backend (API)
```bash
cd src/DevOpsAIAgent.API
dotnet run
```
- API: http://localhost:5000
- Swagger UI: http://localhost:5000/swagger

### Start Frontend (React)
```bash
cd src/DevOpsAIAgent.Client
npm run dev
```
- React app: http://localhost:5173

## 🤖 LLM Provider Setup

### Google Gemini
1. Get API key from [Google AI Studio](https://makersuite.google.com/app/apikey)
2. Add to `appsettings.json` or set `GEMINI_API_KEY` environment variable
3. Set `"Enabled": true` in configuration

### OpenRouter
1. Get API key from [OpenRouter](https://openrouter.ai/keys)
2. Add to `appsettings.json` or set `OPENROUTER_API_KEY` environment variable
3. Choose your preferred model (e.g., `anthropic/claude-3.5-sonnet`)
4. Set `"Enabled": true` in configuration

### Ollama (Local Models)
1. Install [Ollama](https://ollama.ai/)
2. Pull a model: `ollama pull llama3.1`
3. Start Ollama service
4. Set `"Enabled": true` in Ollama configuration

## 📖 Usage

### DevOps Analysis
1. Navigate to `/analysis` in the web app
2. Select your preferred LLM provider
3. Enter your analysis prompt
4. Provide context (logs, configurations, etc.)
5. Click "Analyze" or "Analyze with Fallback"

### Code Review
1. Navigate to `/code-review` in the web app
2. Select your LLM provider
3. Paste code changes or new functions
4. Click "Review Code" for AI analysis

## 🔧 API Endpoints

### LLM Analysis
- `POST /api/analysis/devops` - Generate DevOps analysis
- `POST /api/analysis/devops/fallback` - Generate analysis with fallback
- `POST /api/analysis/code-review` - Generate code review
- `GET /api/analysis/providers` - List available providers and status
- `GET /api/analysis/health` - Health check for LLM providers

### System
- `GET /ping` - API health check
- `GET /health` - Detailed health status

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

### Health & Monitoring
- **GET** `/health` - Application health check
- **GET** `/ping` - Simple ping endpoint

### GitHub Integration (v1 API)
- **GET** `/api/v1/github/repository/{owner}/{repo}` - Get repository information
- **POST** `/api/v1/github/webhook` - GitHub webhook handler
- **GET** `/api/v1/github/repository/{owner}/{repo}/pull-requests` - List pull requests

### Legacy Webhooks (v1)
- **POST** `/api/webhooks/github` - Legacy GitHub webhook receiver
- **GET** `/api/webhooks/repositories` - List user repositories
- **POST** `/api/webhooks/configure` - Create webhook configuration
- **GET** `/api/webhooks/github/health` - Legacy health check

### SignalR Hubs
- **Hub:** `/api/hubs/dashboard` - Real-time dashboard updates

### API Documentation
- **Swagger UI:** `http://localhost:5000/swagger` - Interactive API documentation
- **OpenAPI Spec:** `http://localhost:5000/swagger/v1/swagger.json` - OpenAPI specification

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

