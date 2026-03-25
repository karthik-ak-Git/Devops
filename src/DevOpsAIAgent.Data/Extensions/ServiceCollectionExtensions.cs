using DevOpsAIAgent.Core.Interfaces.Repositories;
using DevOpsAIAgent.Data.Configuration;
using DevOpsAIAgent.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DevOpsAIAgent.Data.Extensions;

/// <summary>
/// Service registration extensions for the Data layer
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add data layer services to the DI container
    /// </summary>
    public static IServiceCollection AddDataServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment? environment = null)
    {
        // Get configuration values
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var environmentName = environment?.EnvironmentName ?? "Development";

        // Determine database provider
        var provider = DatabaseConfiguration.DetermineProvider(connectionString, environmentName);
        connectionString = DatabaseConfiguration.GetConnectionString(connectionString, provider);

        // Register DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            DatabaseConfiguration.ConfigureDatabase(options, connectionString, provider);
        });

        // Register repositories
        services.AddScoped<ICiCdEventRepository, CiCdEventRepository>();
        services.AddScoped<IAiAnalysisRepository, AiAnalysisRepository>();
        services.AddScoped<IDeploymentRepository, DeploymentRepository>();
        services.AddScoped<IIncidentRepository, IncidentRepository>();
        services.AddScoped<ITrackedRepositoryRepository, TrackedRepositoryRepository>();
        services.AddScoped<IWebhookConfigurationRepository, WebhookConfigurationRepository>();
        services.AddScoped<IUserSettingsRepository, UserSettingsRepository>();

        return services;
    }

    /// <summary>
    /// Initialize the database (create/migrate)
    /// </summary>
    public static async Task<IServiceProvider> InitializeDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        try
        {
            logger.LogInformation("Initializing database...");

            // Ensure database is created and migrations are applied
            await context.Database.MigrateAsync();

            logger.LogInformation("Database initialized successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database");
            throw;
        }

        return serviceProvider;
    }

    /// <summary>
    /// Check database health
    /// </summary>
    public static async Task<bool> CheckDatabaseHealthAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            return await context.Database.CanConnectAsync();
        }
        catch
        {
            return false;
        }
    }
}