using DevOpsAIAgent.Data;
using DevOpsAIAgent.Web.Hubs;
using DevOpsAIAgent.Web.Services;
using Microsoft.EntityFrameworkCore;

// Load environment variables from .env file
// Try multiple locations: solution root, project directory, and current directory
var envPaths = new[]
{
    Path.Combine(Directory.GetCurrentDirectory(), ".env"),
    Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".env"),
    Path.Combine(AppContext.BaseDirectory, ".env"),
    Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", ".env")
};

foreach (var envPath in envPaths)
{
    var fullPath = Path.GetFullPath(envPath);
    if (File.Exists(fullPath))
    {
        DotNetEnv.Env.Load(fullPath);
        Console.WriteLine($"✅ Loaded .env from: {fullPath}");
        break;
    }
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Register SignalR
builder.Services.AddSignalR();

// Register Entity Framework SQLite DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=devopsagent.db"));

// Register GitHub Analysis Service
builder.Services.AddScoped<IGitHubAnalysisService, GitHubAnalysisService>();

// Register Webhook Configuration Service
builder.Services.AddScoped<IWebhookConfigurationService, WebhookConfigurationService>();

// Register Gemini AI Assistant Service
builder.Services.AddScoped<IAIAssistantService, GeminiAssistantService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

// Map API Controllers (Web API endpoints)
app.MapControllers();

// Map default MVC controller route (Frontend views)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Map SignalR hub endpoint
app.MapHub<DashboardHub>("/api/hubs/dashboard");

app.Run();
