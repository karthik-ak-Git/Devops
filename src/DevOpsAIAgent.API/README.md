# DevOps AI Agent API

A comprehensive REST API backend for the DevOps AI Agent system, providing CI/CD monitoring, incident management, and AI-powered analysis capabilities.

## 🚀 Features

### Core Functionality
- **GitHub Webhook Processing**: Secure webhook endpoint for GitHub CI/CD events with HMAC verification
- **Real-time Dashboard**: SignalR-powered real-time updates for dashboard components
- **AI-Powered Analysis**: Intelligent failure analysis using Google Gemini AI
- **Incident Management**: Complete incident lifecycle management
- **Repository Tracking**: Multi-repository monitoring and metrics
- **Vector Embeddings**: Semantic similarity search for related issues

### API Endpoints

#### Webhooks
- `POST /api/webhooks/github` - Process GitHub webhook events
- `GET /api/webhooks/health` - Webhook health check
- `POST /api/webhooks/test/{repo}` - Test webhook configuration

#### CI/CD Events
- `GET /api/cicd-events` - List CI/CD events (paginated)
- `GET /api/cicd-events/{id}` - Get specific event
- `GET /api/cicd-events/statistics` - CI/CD statistics
- `GET /api/cicd-events/failures` - Recent failures

#### Incidents
- `GET /api/incidents` - List incidents (paginated)
- `GET /api/incidents/{id}` - Get specific incident
- `POST /api/incidents` - Create incident
- `PUT /api/incidents/{id}` - Update incident
- `DELETE /api/incidents/{id}` - Delete incident
- `GET /api/incidents/statistics` - Incident statistics

#### Repositories
- `GET /api/repositories` - List tracked repositories
- `GET /api/repositories/{id}` - Get repository details
- `POST /api/repositories` - Add repository for tracking
- `PUT /api/repositories/{id}` - Update repository
- `DELETE /api/repositories/{id}` - Remove repository
- `GET /api/repositories/{id}/metrics` - Repository metrics

#### Dashboard
- `GET /api/dashboard/summary` - Dashboard summary
- `GET /api/dashboard/activity-feed` - Activity feed
- `GET /api/dashboard/failure-trends` - Failure trends
- `GET /api/dashboard/repository/{name}/metrics` - Repository-specific metrics
- `POST /api/dashboard/refresh` - Refresh dashboard data

#### AI Analysis
- `GET /api/ai-analysis` - List AI analyses
- `GET /api/ai-analysis/{id}` - Get specific analysis
- `POST /api/ai-analysis/analyze` - Trigger AI analysis
- `GET /api/ai-analysis/statistics` - Analysis statistics
- `GET /api/ai-analysis/{id}/similar` - Find similar analyses

### Real-time Features (SignalR)
- **Hub Endpoint**: `/hubs/dashboard`
- **Events**:
  - `PipelineFailure` - Pipeline failure notifications
  - `DashboardUpdate` - Dashboard summary updates
  - `ActivityFeedUpdate` - New activity items
  - `RepositoryNotification` - Repository-specific updates

## 🛠️ Technology Stack

- **.NET 8.0** - Modern web API framework
- **ASP.NET Core** - Web API framework
- **Entity Framework Core** - ORM with PostgreSQL support
- **SignalR** - Real-time web functionality
- **Serilog** - Structured logging
- **Swagger/OpenAPI** - API documentation
- **Google Gemini AI** - AI analysis and embeddings
- **Octokit** - GitHub API integration
- **pgvector** - PostgreSQL vector operations

## 📋 Prerequisites

- .NET 8.0 SDK
- PostgreSQL 13+ (with pgvector extension)
- Google Gemini API key
- GitHub token (optional, for enhanced API limits)

## ⚙️ Configuration

### Environment Variables
```bash
# Required
GEMINI_API_KEY=your_gemini_api_key_here
ASPNETCORE_ENVIRONMENT=Development

# Optional
GITHUB_TOKEN=your_github_token_here
GITHUB_WEBHOOK_SECRET=your_webhook_secret
OPENROUTER_API_KEY=your_openrouter_key_here
```

### Database Configuration
Update `appsettings.json` with your PostgreSQL connection:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=devops_ai_agent;Username=postgres;Password=your_password"
  }
}
```

### AI Provider Configuration
```json
{
  "LlmProviders": {
    "DefaultProvider": "Gemini",
    "Gemini": {
      "ApiKey": "${GEMINI_API_KEY}",
      "Model": "gemini-1.5-pro-latest",
      "Temperature": 0.3
    }
  }
}
```

## 🚀 Getting Started

### 1. Clone and Setup
```bash
cd src/DevOpsAIAgent.API
dotnet restore
```

### 2. Database Setup
```bash
# Install Entity Framework CLI
dotnet tool install --global dotnet-ef

# Create and apply migrations
dotnet ef database update
```

### 3. Configure Environment
```bash
# Copy and edit configuration
cp appsettings.json appsettings.Local.json
# Edit appsettings.Local.json with your settings
```

### 4. Run the API
```bash
dotnet run
```

The API will be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `http://localhost:5000` (in development)

## 📝 API Documentation

### Authentication
Currently, the API uses no authentication for development. JWT Bearer authentication can be enabled via configuration.

### Rate Limiting
- Default: 100 requests per minute
- Burst: 20 requests
- Configurable via `ApiSettings:RateLimiting`

### Error Handling
All endpoints return standardized error responses:
```json
{
  "success": false,
  "message": "Error description",
  "errors": ["Detailed error messages"],
  "timestamp": "2024-03-25T10:30:00Z"
}
```

### Pagination
List endpoints support pagination:
```json
{
  "items": [...],
  "totalCount": 150,
  "page": 1,
  "pageSize": 20,
  "totalPages": 8
}
```

## 🔧 Development

### Project Structure
```
DevOpsAIAgent.API/
├── Controllers/           # API controllers
├── Services/             # Business logic services
├── Hubs/                 # SignalR hubs
├── Properties/           # Launch settings
├── appsettings.json      # Configuration
└── Program.cs            # Application entry point
```

### Service Registration
Services are registered in `Program.cs`:
- **Repositories**: Data access layer
- **Services**: Business logic
- **LLM Providers**: AI service providers
- **SignalR**: Real-time communication

### Logging
Structured logging with Serilog:
- Console output (development)
- File output (`./logs/api-{date}.log`)
- Configurable log levels

### Health Checks
- Database connectivity
- External service status
- Endpoint: `/health`

## 🐳 Docker Support

### Dockerfile
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["DevOpsAIAgent.API/DevOpsAIAgent.API.csproj", "DevOpsAIAgent.API/"]
RUN dotnet restore "DevOpsAIAgent.API/DevOpsAIAgent.API.csproj"
COPY . .
WORKDIR "/src/DevOpsAIAgent.API"
RUN dotnet build "DevOpsAIAgent.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DevOpsAIAgent.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DevOpsAIAgent.API.dll"]
```

### Docker Compose
```yaml
version: '3.8'
services:
  api:
    build: .
    ports:
      - "5000:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=devops_ai_agent;Username=postgres;Password=postgres
    depends_on:
      - postgres

  postgres:
    image: pgvector/pgvector:pg16
    environment:
      POSTGRES_DB: devops_ai_agent
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

volumes:
  postgres_data:
```

## 🔐 Security

### Webhook Security
- HMAC SHA-256 signature verification
- Configurable webhook secrets
- Request size limits

### CORS Configuration
- Configurable allowed origins
- Support for React frontend
- Credentials support for SignalR

### Input Validation
- FluentValidation for request models
- Model binding validation
- SQL injection protection via EF Core

## 📊 Monitoring

### Health Checks
- `/health` - Basic health status
- Database connectivity check
- External service verification

### Logging
- Structured logging with Serilog
- Request/response logging
- Performance metrics
- Error tracking

### Metrics
- Pipeline success/failure rates
- Incident resolution times
- AI analysis performance
- Real-time dashboard updates

## 🚀 Deployment

### Production Considerations
1. **Database**: Use managed PostgreSQL with pgvector
2. **Secrets**: Use Azure Key Vault or AWS Secrets Manager
3. **Logging**: Centralized logging (Application Insights, ELK)
4. **Monitoring**: Health checks and metrics
5. **Scaling**: Load balancer for multiple instances
6. **Security**: Enable HTTPS and authentication

### Environment-Specific Settings
- Development: In-memory fallbacks, detailed errors
- Staging: Full external services, verbose logging
- Production: Optimized performance, error handling

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Implement changes with tests
4. Update documentation
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🆘 Support

For support and questions:
- Create an issue in the repository
- Check the API documentation at `/swagger`
- Review the logs in `./logs/` directory

---

**DevOps AI Agent API** - Intelligent DevOps monitoring and incident management powered by AI.