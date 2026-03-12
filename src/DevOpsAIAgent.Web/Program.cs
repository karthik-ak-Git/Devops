using DevOpsAIAgent.Data;
using DevOpsAIAgent.Web.Hubs;
using DevOpsAIAgent.Web.Services;
using Microsoft.EntityFrameworkCore;

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

// Map default controller route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Map SignalR hub endpoint
app.MapHub<DashboardHub>("/dashboardHub");

app.Run();
