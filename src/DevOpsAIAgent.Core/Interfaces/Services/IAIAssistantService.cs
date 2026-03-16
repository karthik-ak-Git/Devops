namespace DevOpsAIAgent.Core.Interfaces.Services;

public interface IAIAssistantService
{
    Task<string> AnalyzeFailureAsync(string errorLog, string gitDiff);
    Task<string> SuggestIncidentResolutionAsync(string incidentDescription, IEnumerable<string> similarPastAnalyses);
}
