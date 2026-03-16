using DevOpsAIAgent.Core.Models;

namespace DevOpsAIAgent.Core.Interfaces.Repositories;

public interface ICiCdEventRepository
{
    Task<CiCdEvent?> GetByIdAsync(long id);
    Task<IReadOnlyList<CiCdEvent>> GetRecentAsync(int count = 50);
    Task<IReadOnlyList<CiCdEvent>> GetByRepositoryAsync(string repositoryName, int count = 50);
    Task<CiCdEvent> AddAsync(CiCdEvent entity);
    Task<int> GetFailureCountAsync(string repositoryName, DateTime since);
    Task<int> GetTotalCountAsync(string repositoryName, DateTime since);
}
