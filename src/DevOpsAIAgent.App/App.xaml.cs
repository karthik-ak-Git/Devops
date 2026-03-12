using System.Windows;
using DevOpsAIAgent.App.ViewModels;
using DevOpsAIAgent.App.Views;
using DevOpsAIAgent.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace DevOpsAIAgent.App;

public partial class App : Application
{
    private IHost? _host;
    private IWebhookListenerService? _webhookListener;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Debug()
            .WriteTo.File("logs/devops-ai-agent-.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        _host = Host.CreateDefaultBuilder()
            .UseSerilog()
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<IWebhookListenerService, WebhookListenerService>();
                services.AddSingleton<IGitHubAnalysisService, GitHubAnalysisService>();
                
                services.AddSingleton<DashboardViewModel>();
                services.AddTransient<DashboardWindow>();
            })
            .Build();

        await _host.StartAsync();

        _webhookListener = _host.Services.GetRequiredService<IWebhookListenerService>();
        await _webhookListener.StartAsync();

        var mainWindow = _host.Services.GetRequiredService<DashboardWindow>();
        mainWindow.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_webhookListener != null)
        {
            await _webhookListener.StopAsync();
            _webhookListener.Dispose();
        }

        if (_host != null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }

        Log.CloseAndFlush();
        base.OnExit(e);
    }
}
