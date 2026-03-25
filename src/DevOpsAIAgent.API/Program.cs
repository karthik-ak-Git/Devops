using Serilog;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using DevOpsAIAgent.Data;
using DevOpsAIAgent.Core.Interfaces.Services;
using DevOpsAIAgent.Core.Interfaces.Repositories;
using DevOpsAIAgent.Data.Repositories;
using DevOpsAIAgent.API.Services;
using DevOpsAIAgent.API.Services.LlmProviders;
using DevOpsAIAgent.Core.Models.Configuration;
using DevOpsAIAgent.API.Hubs;
using Refit;

// Load environment variables
DotNetEnv.Env.Load();

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
        .AddEnvironmentVariables()
        .Build())
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add Serilog
builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();

// Add OpenAPI/Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "DevOps AI Agent API",
        Version = "v1",
        Description = "Comprehensive REST API for DevOps AI Agent - CI/CD monitoring, incident management, and AI-powered analysis",
        Contact = new() { Name = "DevOps AI Agent", Email = "support@devopsaiagent.com" }
    });

    // Include XML comments for better documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Add security definitions if needed
    c.AddSecurityDefinition("Bearer", new()
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (!string.IsNullOrEmpty(connectionString))
    {
        options.UseNpgsql(connectionString);
    }
    else
    {
        // Fallback to in-memory database for development
        options.UseInMemoryDatabase("DevOpsAIAgent");
    }
});

// Add LLM Configuration
builder.Services.Configure<LlmConfiguration>(
    builder.Configuration.GetSection(LlmConfiguration.SectionName));

// Add Refit client for OpenRouter
builder.Services.AddRefitClient<IOpenRouterApi>()
    .ConfigureHttpClient(c =>
    {
        var openRouterConfig = builder.Configuration
            .GetSection($"{LlmConfiguration.SectionName}:OpenRouter")
            .Get<OpenRouterConfiguration>();

        if (openRouterConfig != null && !string.IsNullOrEmpty(openRouterConfig.BaseUrl))
        {
            c.BaseAddress = new Uri(openRouterConfig.BaseUrl);
        }
    });

// Register repositories
builder.Services.AddScoped<IAiAnalysisRepository, AiAnalysisRepository>();
builder.Services.AddScoped<ICiCdEventRepository, CiCdEventRepository>();
builder.Services.AddScoped<IDeploymentRepository, DeploymentRepository>();
builder.Services.AddScoped<IIncidentRepository, IncidentRepository>();
builder.Services.AddScoped<ITrackedRepositoryRepository, TrackedRepositoryRepository>();
builder.Services.AddScoped<IWebhookConfigurationRepository, WebhookConfigurationRepository>();
builder.Services.AddScoped<IUserSettingsRepository, UserSettingsRepository>();

// Register LLM providers
builder.Services.AddScoped<ILlmProvider, GeminiProvider>();
builder.Services.AddScoped<ILlmProvider, OpenRouterProvider>();
// builder.Services.AddScoped<ILlmProvider, OllamaProvider>(); // Temporarily disabled due to API compatibility

// Register LLM service
builder.Services.AddScoped<ILlmService, LlmService>();

// Register core services
builder.Services.AddScoped<IWebhookSecurityService, WebhookSecurityService>();
// builder.Services.AddScoped<IWebhookConfigurationService, WebhookConfigurationService>(); // Temporarily disabled
// builder.Services.AddScoped<IGitHubAnalysisService, GitHubAnalysisService>(); // Temporarily disabled due to API compatibility
// builder.Services.AddScoped<IAIAssistantService, GeminiAssistantService>(); // Temporarily disabled
// builder.Services.AddScoped<IEmbeddingService, EmbeddingService>(); // Temporarily disabled
// builder.Services.AddScoped<IDashboardNotificationService, DashboardNotificationService>(); // Temporarily disabled
// builder.Services.AddScoped<DashboardService>(); // Temporarily disabled

// Add SignalR
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
});

// Add health checks
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("API is running"))
    .AddDbContextCheck<ApplicationDbContext>("database");

// Add memory cache
builder.Services.AddMemoryCache();

// Add HTTP client
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "DevOps AI Agent API V1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at the app's root
        c.DisplayRequestDuration();
        c.EnableTryItOutByDefault();
        c.DocumentTitle = "DevOps AI Agent API Documentation";
    });
}

// Add security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    await next();
});

app.UseSerilogRequestLogging();

app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Map SignalR hubs
app.MapHub<DashboardHub>("/hubs/dashboard");

// Add health check endpoint
app.MapHealthChecks("/health");

// Add a simple ping endpoint
app.MapGet("/ping", () => Results.Ok(new { message = "DevOps AI Agent API is running", timestamp = DateTime.UtcNow }))
   .WithName("Ping");

app.Run();
