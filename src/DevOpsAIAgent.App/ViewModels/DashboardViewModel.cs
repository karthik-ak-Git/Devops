using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using DevOpsAIAgent.Core.Messages;
using DevOpsAIAgent.Core.Models;
using Microsoft.Extensions.Logging;

namespace DevOpsAIAgent.App.ViewModels;

public partial class DashboardViewModel : ObservableObject, IRecipient<PipelineFailedMessage>
{
    private readonly ILogger<DashboardViewModel> _logger;

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

    public DashboardViewModel(ILogger<DashboardViewModel> logger)
    {
        _logger = logger;
        WeakReferenceMessenger.Default.Register(this);
    }

    public void Receive(PipelineFailedMessage message)
    {
        _logger.LogInformation("Received pipeline failure notification for {Repository}", message.Event.RepositoryName);

        RepositoryName = message.Event.RepositoryName;
        LastFailedCommit = message.Event.CommitHash;
        WorkflowName = message.Event.WorkflowName;
        RepositoryUrl = message.Event.RepositoryUrl;
        LastFailureTime = message.Event.ReceivedAt;
    }
}
