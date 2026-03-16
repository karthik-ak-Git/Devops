using DevOpsAIAgent.Core.Interfaces.Repositories;
using DevOpsAIAgent.Core.Interfaces.Services;
using DevOpsAIAgent.Core.Models;
using Pgvector;
using Mscc.GenerativeAI;

namespace DevOpsAIAgent.Web.Services;

public class EmbeddingService : IEmbeddingService
{
    private readonly IAiAnalysisRepository _analysisRepo;
    private readonly ILogger<EmbeddingService> _logger;
    private readonly string _apiKey;
    private readonly bool _isConfigured;

    public EmbeddingService(IAiAnalysisRepository analysisRepo, ILogger<EmbeddingService> logger)
    {
        _analysisRepo = analysisRepo;
        _logger = logger;
        _apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY") ?? string.Empty;
        _isConfigured = !string.IsNullOrWhiteSpace(_apiKey);
    }

    public async Task<Vector?> GenerateEmbeddingAsync(string text)
    {
        if (!_isConfigured) return null;

        try
        {
            var googleAI = new GoogleAI(apiKey: _apiKey);
            var model = googleAI.GenerativeModel(model: "text-embedding-004");
            var response = await model.EmbedContent(text);

            if (response?.Embedding?.Values != null)
            {
                var values = response.Embedding.Values.Select(v => (float)v).ToArray();
                return new Vector(values);
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating embedding");
            return null;
        }
    }

    public async Task<IReadOnlyList<AiAnalysis>> FindSimilarAnalysesAsync(Vector embedding, int count = 5) =>
        await _analysisRepo.FindSimilarAsync(embedding, count);
}
