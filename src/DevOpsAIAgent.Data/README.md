# DevOpsAIAgent.Data

Data access layer for the DevOps AI Agent application, providing Entity Framework Core-based repository pattern implementation with dual database support (SQLite for development, PostgreSQL for production).

## Features

- **Dual Database Support**: SQLite for development, PostgreSQL for production
- **Vector Embeddings**: Full pgvector support for AI analysis similarity search
- **High Performance**: Optimized indexes and async patterns throughout
- **Repository Pattern**: Clean abstraction over Entity Framework Core
- **Migration Support**: Automatic database schema management
- **Health Checks**: Built-in database connectivity monitoring

## Database Providers

### SQLite (Development)
- Used for local development and testing
- Stores vector embeddings as JSON text
- Automatically configured for Development environment

### PostgreSQL (Production)
- High-performance production database
- Native vector support via pgvector extension
- Optimized for large-scale operations

## Quick Start

### 1. Add to DI Container

```csharp
// In Program.cs or Startup.cs
services.AddDataServices(configuration, environment);
```

### 2. Initialize Database

```csharp
// After building the service provider
await serviceProvider.InitializeDatabaseAsync();
```

### 3. Use Repositories

```csharp
public class MyController : ControllerBase
{
    private readonly ICiCdEventRepository _ciCdRepository;
    private readonly IAiAnalysisRepository _aiRepository;

    public MyController(
        ICiCdEventRepository ciCdRepository,
        IAiAnalysisRepository aiRepository)
    {
        _ciCdRepository = ciCdRepository;
        _aiRepository = aiRepository;
    }

    [HttpGet("events/recent")]
    public async Task<IActionResult> GetRecentEvents()
    {
        var events = await _ciCdRepository.GetRecentAsync(20);
        return Ok(events);
    }

    [HttpPost("analysis/similar")]
    public async Task<IActionResult> FindSimilar([FromBody] float[] embedding)
    {
        var similar = await _aiRepository.FindSimilarAsync(new Vector(embedding));
        return Ok(similar);
    }
}
```

## Configuration

### Connection Strings

Add to your `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=devops_ai_agent;Username=postgres;Password=postgres"
  }
}
```

### Environment-Based Configuration

The system automatically selects the database provider based on environment:

- **Development**: SQLite (`devops_ai_agent.db`)
- **Production/Staging**: PostgreSQL
- **Testing**: In-Memory database

Override by setting the connection string explicitly.

## Repository Interfaces

### ICiCdEventRepository
```csharp
Task<CiCdEvent?> GetByIdAsync(long id);
Task<IReadOnlyList<CiCdEvent>> GetRecentAsync(int count = 50);
Task<IReadOnlyList<CiCdEvent>> GetByRepositoryAsync(string repositoryName, int count = 50);
Task<CiCdEvent> AddAsync(CiCdEvent entity);
Task<int> GetFailureCountAsync(string repositoryName, DateTime since);
Task<int> GetTotalCountAsync(string repositoryName, DateTime since);
```

### IAiAnalysisRepository
```csharp
Task<AiAnalysis?> GetByIdAsync(long id);
Task<AiAnalysis?> GetByCiCdEventIdAsync(long ciCdEventId);
Task<AiAnalysis> AddAsync(AiAnalysis entity);
Task<IReadOnlyList<AiAnalysis>> FindSimilarAsync(object embedding, int count = 5);
Task<int> GetTotalCountAsync();
```

### IUserSettingsRepository
```csharp
Task<UserSettings?> GetByIdAsync(long id);
Task<UserSettings?> GetByKeyAsync(string key);
Task<IReadOnlyList<UserSettings>> GetAllAsync();
Task<UserSettings> AddAsync(UserSettings entity);
Task<UserSettings> UpdateAsync(UserSettings entity);
Task<bool> DeleteAsync(long id);
Task<bool> ExistsAsync(string key);
```

## Database Schema

### Core Tables

- **CiCdEvents**: CI/CD pipeline events and workflow runs
- **AiAnalyses**: AI-generated analysis with vector embeddings
- **Deployments**: Deployment tracking and status
- **Incidents**: Issue and incident management
- **TrackedRepositories**: Repository configuration
- **WebhookConfigurations**: GitHub webhook settings
- **UserSettings**: Application configuration

### Key Relationships

- AiAnalysis ↔ CiCdEvent (One-to-One)
- Incident → CiCdEvent (Many-to-One, optional)

### Performance Indexes

All tables include optimized indexes for common query patterns:
- Repository name lookups
- Time-based queries
- Status filtering
- Vector similarity (PostgreSQL only)

## Migrations

### Create Migration
```bash
dotnet ef migrations add MigrationName --project DevOpsAIAgent.Data
```

### Update Database
```bash
dotnet ef database update --project DevOpsAIAgent.Data
```

### Production Deployment
```csharp
// Automatic during application startup
await serviceProvider.InitializeDatabaseAsync();
```

## Vector Embeddings

### PostgreSQL (Production)
- Native pgvector support
- Optimized cosine similarity search
- 1536-dimension embeddings (OpenAI compatible)

### SQLite (Development)
- JSON text storage
- Limited similarity search
- Fallback to recency-based ordering

### Usage Example
```csharp
var analysis = new AiAnalysis
{
    CiCdEventId = eventId,
    AnalysisText = "Analysis content",
    ModelUsed = "gpt-4",
    Embedding = new Vector(embeddings), // float[] or Vector
    TokensUsed = 150
};

await _aiRepository.AddAsync(analysis);

// Find similar analyses
var similar = await _aiRepository.FindSimilarAsync(queryEmbedding, count: 5);
```

## Health Monitoring

```csharp
// Check database connectivity
var isHealthy = await serviceProvider.CheckDatabaseHealthAsync();

if (!isHealthy)
{
    // Handle database connectivity issues
}
```

## Best Practices

### Repository Usage
- Always use async methods
- Leverage IReadOnlyList for query results
- Use proper cancellation tokens in production

### Performance Optimization
- Repository methods include Include() statements for related data
- Queries use appropriate indexes
- Batch operations where possible

### Error Handling
- Repositories throw EF Core exceptions
- Use try-catch blocks in service layer
- Log database errors appropriately

## Dependencies

- **Microsoft.EntityFrameworkCore** (8.0.11)
- **Npgsql.EntityFrameworkCore.PostgreSQL** (8.0.10)
- **Microsoft.EntityFrameworkCore.Sqlite** (8.0.11)
- **Pgvector.EntityFrameworkCore** (0.3.0)
- **Microsoft.EntityFrameworkCore.Tools** (8.0.11)

## Database Administration

### PostgreSQL Setup
```sql
-- Create database
CREATE DATABASE devops_ai_agent;

-- Enable vector extension
CREATE EXTENSION IF NOT EXISTS vector;

-- Create user (optional)
CREATE USER devops_ai WITH ENCRYPTED PASSWORD 'your_password';
GRANT ALL PRIVILEGES ON DATABASE devops_ai_agent TO devops_ai;
```

### Backup & Restore
```bash
# PostgreSQL backup
pg_dump devops_ai_agent > backup.sql

# PostgreSQL restore
psql devops_ai_agent < backup.sql

# SQLite backup
cp devops_ai_agent.db backup.db
```

This data layer provides a robust, scalable foundation for the DevOps AI Agent application with optimized performance and dual database support.