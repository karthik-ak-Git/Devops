using DevOpsAIAgent.Core.Models;
using DevOpsAIAgent.Core.Models.Enums;

namespace DevOpsAIAgent.Core.Interfaces.Repositories;

public interface IIncidentRepository
{
    Task<Incident?> GetByIdAsync(long id);
    Task<IReadOnlyList<Incident>> GetRecentAsync(int count = 50);
    Task<IReadOnlyList<Incident>> GetOpenAsync();
    Task<Incident> AddAsync(Incident entity);
    Task UpdateAsync(Incident entity);
    Task<int> GetCountByStatusAsync(IncidentStatus status);
    Task<int> GetCountBySeverityAsync(IncidentSeverity severity);
}
