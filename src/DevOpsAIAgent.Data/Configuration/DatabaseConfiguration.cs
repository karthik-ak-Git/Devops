using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace DevOpsAIAgent.Data.Configuration;

/// <summary>
/// Database configuration helper for multi-provider support
/// </summary>
public static class DatabaseConfiguration
{
    /// <summary>
    /// Configure the DbContext for the specified database provider
    /// </summary>
    public static void ConfigureDatabase(DbContextOptionsBuilder options, string connectionString, DatabaseProvider provider)
    {
        switch (provider)
        {
            case DatabaseProvider.PostgreSQL:
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.UseVector();
                    npgsqlOptions.EnableRetryOnFailure(3);
                    npgsqlOptions.CommandTimeout(30);
                });
                break;

            case DatabaseProvider.SQLite:
                options.UseSqlite(connectionString, sqliteOptions =>
                {
                    sqliteOptions.CommandTimeout(30);
                });
                break;

            case DatabaseProvider.InMemory:
                options.UseInMemoryDatabase(connectionString);
                break;

            default:
                throw new ArgumentException($"Unsupported database provider: {provider}");
        }

        // Common configuration
        options.EnableSensitiveDataLogging(false);
        options.EnableServiceProviderCaching();
        options.EnableDetailedErrors();
    }

    /// <summary>
    /// Get the appropriate connection string for the environment
    /// </summary>
    public static string GetConnectionString(string? connectionString, DatabaseProvider provider)
    {
        if (!string.IsNullOrEmpty(connectionString))
            return connectionString;

        return provider switch
        {
            DatabaseProvider.PostgreSQL => "Host=localhost;Database=devops_ai_agent;Username=postgres;Password=postgres",
            DatabaseProvider.SQLite => "Data Source=devops_ai_agent.db",
            DatabaseProvider.InMemory => "DevOpsAIAgent_InMemory",
            _ => throw new ArgumentException($"No default connection string for provider: {provider}")
        };
    }

    /// <summary>
    /// Determine database provider from connection string or environment
    /// </summary>
    public static DatabaseProvider DetermineProvider(string? connectionString, string? environment = null)
    {
        // Check environment variable first
        if (!string.IsNullOrEmpty(environment))
        {
            return environment.ToLowerInvariant() switch
            {
                "production" => DatabaseProvider.PostgreSQL,
                "staging" => DatabaseProvider.PostgreSQL,
                "development" => DatabaseProvider.SQLite,
                "testing" => DatabaseProvider.InMemory,
                _ => DatabaseProvider.SQLite
            };
        }

        // Fallback to connection string analysis
        if (string.IsNullOrEmpty(connectionString))
            return DatabaseProvider.SQLite;

        if (connectionString.Contains("Host=") || connectionString.Contains("Server="))
            return DatabaseProvider.PostgreSQL;

        if (connectionString.Contains("Data Source=") && connectionString.Contains(".db"))
            return DatabaseProvider.SQLite;

        return DatabaseProvider.SQLite;
    }
}

/// <summary>
/// Supported database providers
/// </summary>
public enum DatabaseProvider
{
    PostgreSQL,
    SQLite,
    InMemory
}