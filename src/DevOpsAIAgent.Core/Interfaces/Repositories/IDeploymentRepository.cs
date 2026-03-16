using DevOpsAIAgent.Core.Models;
using DevOpsAIAgent.Core.Models.Enums;

namespace DevOpsAIAgent.Core.Interfaces.Repositories;

public interface IDeploymentRepository
{
    Task<Deployment?> GetByIdAsync(long id);
    Task<IReadOnlyList<Deployment>> GetRecentAsync(int count = 50);
    Task<IReadOnlyList<Deployment>> GetByRepositoryAsync(string repositoryName, int count = 50);
    Task<Deployment> AddAsync(Deployment entity);
    Task UpdateAsync(Deployment entity);
    Task<int> GetCountByStatusAsync(DeploymentStatus status);
}
