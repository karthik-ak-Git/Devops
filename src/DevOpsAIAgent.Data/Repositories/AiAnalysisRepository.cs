using DevOpsAIAgent.Core.Interfaces.Repositories;
using DevOpsAIAgent.Core.Models;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using System.Text.Json;

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

    public async Task<IReadOnlyList<AiAnalysis>> FindSimilarAsync(object embedding, int count = 5)
    {
        Vector? vector = embedding as Vector;

        if (vector is null)
        {
            // Try to convert from float array or other formats
            vector = embedding switch
            {
                float[] floatArray => new Vector(floatArray),
                string jsonString => TryParseJsonVector(jsonString),
                _ => null
            };
        }

        if (vector is null)
            return Array.Empty<AiAnalysis>();

        // PostgreSQL vector similarity search
        if (_context.Database.IsNpgsql())
        {
            return await _context.AiAnalyses
                .Where(a => a.Embedding != null)
                .OrderBy(a => a.Embedding!.CosineDistance(vector))
                .Take(count)
                .Include(a => a.CiCdEvent)
                .ToListAsync();
        }

        // SQLite fallback - simple text similarity (limited functionality)
        // In production, consider using a separate vector database for SQLite environments
        return await _context.AiAnalyses
            .Where(a => a.Embedding != null)
            .OrderByDescending(a => a.CreatedAt)
            .Take(count)
            .Include(a => a.CiCdEvent)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync() =>
        await _context.AiAnalyses.CountAsync();

    private static Vector? TryParseJsonVector(string jsonString)
    {
        try
        {
            var floats = JsonSerializer.Deserialize<float[]>(jsonString);
            return floats is not null ? new Vector(floats) : null;
        }
        catch
        {
            return null;
        }
    }
}
