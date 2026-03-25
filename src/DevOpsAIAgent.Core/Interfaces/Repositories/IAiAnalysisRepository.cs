using DevOpsAIAgent.Core.Models;
// using Pgvector;

namespace DevOpsAIAgent.Core.Interfaces.Repositories;

public interface IAiAnalysisRepository
{
    Task<AiAnalysis?> GetByIdAsync(long id);
    Task<AiAnalysis?> GetByCiCdEventIdAsync(long ciCdEventId);
    Task<AiAnalysis> AddAsync(AiAnalysis entity);
    Task<IReadOnlyList<AiAnalysis>> FindSimilarAsync(object embedding, int count = 5);
    Task<int> GetTotalCountAsync();
}
