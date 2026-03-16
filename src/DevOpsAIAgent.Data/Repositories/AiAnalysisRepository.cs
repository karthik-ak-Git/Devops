using DevOpsAIAgent.Core.Interfaces.Repositories;
using DevOpsAIAgent.Core.Models;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace DevOpsAIAgent.Data.Repositories;

public class AiAnalysisRepository : IAiAnalysisRepository
{
    private readonly ApplicationDbContext _context;

    public AiAnalysisRepository(ApplicationDbContext context) => _context = context;

    public async Task<AiAnalysis?> GetByIdAsync(long id) =>
        await _context.AiAnalyses.Include(a => a.CiCdEvent).FirstOrDefaultAsync(a => a.Id == id);

    public async Task<AiAnalysis?> GetByCiCdEventIdAsync(long ciCdEventId) =>
        await _context.AiAnalyses.Include(a => a.CiCdEvent)
            .FirstOrDefaultAsync(a => a.CiCdEventId == ciCdEventId);

    public async Task<AiAnalysis> AddAsync(AiAnalysis entity)
    {
        _context.AiAnalyses.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<IReadOnlyList<AiAnalysis>> FindSimilarAsync(Vector embedding, int count = 5) =>
        await _context.AiAnalyses
            .Where(a => a.Embedding != null)
            .OrderBy(a => a.Embedding!.CosineDistance(embedding))
            .Take(count)
            .Include(a => a.CiCdEvent)
            .ToListAsync();

    public async Task<int> GetTotalCountAsync() =>
        await _context.AiAnalyses.CountAsync();
}
