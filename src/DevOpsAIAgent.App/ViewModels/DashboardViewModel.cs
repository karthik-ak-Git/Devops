using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using DevOpsAIAgent.Core.Messages;
using DevOpsAIAgent.Core.Models;
using DevOpsAIAgent.Core.Services;
using Microsoft.Extensions.Logging;

namespace DevOpsAIAgent.App.ViewModels;

public partial class DashboardViewModel : ObservableObject, IRecipient<PipelineFailedMessage>
{
    private readonly ILogger<DashboardViewModel> _logger;
    private readonly IGitHubAnalysisService _githubAnalysisService;

    [ObservableProperty]
    private string _repositoryName = "No failures yet";

    [ObservableProperty]
    private string _lastFailedCommit = "Waiting for webhook...";

    [ObservableProperty]
    private string _workflowName = string.Empty;

    [ObservableProperty]
    private string _repositoryUrl = string.Empty;

    [ObservableProperty]
    private DateTime? _lastFailureTime;

    [ObservableProperty]
    private string _currentGitDiff = string.Empty;

    [ObservableProperty]
    private string _currentErrorLog = string.Empty;

    [ObservableProperty]
    private bool _isAnalyzing = false;

    [ObservableProperty]
    private string _analysisStatus = string.Empty;

    public DashboardViewModel(
        ILogger<DashboardViewModel> logger,
        IGitHubAnalysisService githubAnalysisService)
    {
        _logger = logger;
        _githubAnalysisService = githubAnalysisService;
        WeakReferenceMessenger.Default.Register(this);
    }

    public async void Receive(PipelineFailedMessage message)
    {
        _logger.LogInformation("Received pipeline failure notification for {Repository}", message.Event.RepositoryName);

        RepositoryName = message.Event.RepositoryName;
        LastFailedCommit = message.Event.CommitHash;
        WorkflowName = message.Event.WorkflowName;
        RepositoryUrl = message.Event.RepositoryUrl;
        LastFailureTime = message.Event.ReceivedAt;

        await AnalyzeFailureAsync(message.Event);
    }

    private async Task AnalyzeFailureAsync(CiCdEvent cicdEvent)
    {
        IsAnalyzing = true;
        AnalysisStatus = "Analyzing failure...";
        CurrentGitDiff = string.Empty;
        CurrentErrorLog = string.Empty;

        try
        {
            _logger.LogInformation(
                "Starting GitHub analysis for {Owner}/{Repo}, Commit={Commit}, RunId={RunId}",
                cicdEvent.Owner, cicdEvent.RepositoryName, cicdEvent.CommitHash, cicdEvent.RunId);

            AnalysisStatus = "Fetching git diff and error logs from GitHub...";

            var (gitDiff, errorLog) = await _githubAnalysisService.GetFailureContextAsync(
                cicdEvent.Owner,
                cicdEvent.RepositoryName,
                cicdEvent.CommitHash,
                cicdEvent.RunId);

            CurrentGitDiff = gitDiff;
            CurrentErrorLog = errorLog;

            AnalysisStatus = "Analysis complete";
            _logger.LogInformation("GitHub analysis completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to analyze GitHub failure");
            AnalysisStatus = $"Analysis failed: {ex.Message}";
            CurrentGitDiff = "Failed to retrieve git diff";
            CurrentErrorLog = "Failed to retrieve error logs";
        }
        finally
        {
            IsAnalyzing = false;
        }
    }
}
