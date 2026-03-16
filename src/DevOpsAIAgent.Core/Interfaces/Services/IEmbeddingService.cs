using DevOpsAIAgent.Core.Models;
using Pgvector;

namespace DevOpsAIAgent.Core.Interfaces.Services;

public interface IEmbeddingService
{
    Task<Vector?> GenerateEmbeddingAsync(string text);
    Task<IReadOnlyList<AiAnalysis>> FindSimilarAnalysesAsync(Vector embedding, int count = 5);
}
