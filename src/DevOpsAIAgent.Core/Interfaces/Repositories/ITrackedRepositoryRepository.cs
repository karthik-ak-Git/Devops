using DevOpsAIAgent.Core.Models;

namespace DevOpsAIAgent.Core.Interfaces.Repositories;

public interface ITrackedRepositoryRepository
{
    Task<TrackedRepository?> GetByFullNameAsync(string fullName);
    Task<IReadOnlyList<TrackedRepository>> GetAllAsync();
    Task<TrackedRepository> AddOrUpdateAsync(TrackedRepository entity);
    Task IncrementBuildsAsync(string fullName, bool isFailed);
}
