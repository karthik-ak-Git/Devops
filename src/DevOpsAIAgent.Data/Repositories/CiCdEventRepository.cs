using DevOpsAIAgent.Core.Interfaces.Repositories;
using DevOpsAIAgent.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DevOpsAIAgent.Data.Repositories;

public class CiCdEventRepository : ICiCdEventRepository
{
    private readonly ApplicationDbContext _context;

    public CiCdEventRepository(ApplicationDbContext context) => _context = context;

    public async Task<CiCdEvent?> GetByIdAsync(long id) =>
        await _context.CiCdEvents.Include(e => e.AiAnalysis).FirstOrDefaultAsync(e => e.Id == id);

    public async Task<IReadOnlyList<CiCdEvent>> GetRecentAsync(int count = 50) =>
        await _context.CiCdEvents.Include(e => e.AiAnalysis)
            .OrderByDescending(e => e.ReceivedAt).Take(count).ToListAsync();

    public async Task<IReadOnlyList<CiCdEvent>> GetByRepositoryAsync(string repositoryName, int count = 50) =>
        await _context.CiCdEvents.Include(e => e.AiAnalysis)
            .Where(e => e.RepositoryName == repositoryName)
            .OrderByDescending(e => e.ReceivedAt).Take(count).ToListAsync();

    public async Task<CiCdEvent> AddAsync(CiCdEvent entity)
    {
        _context.CiCdEvents.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<int> GetFailureCountAsync(string repositoryName, DateTime since) =>
        await _context.CiCdEvents.CountAsync(e =>
            e.RepositoryName == repositoryName && e.ReceivedAt >= since && e.Conclusion == "failure");

    public async Task<int> GetTotalCountAsync(string repositoryName, DateTime since) =>
        await _context.CiCdEvents.CountAsync(e =>
            e.RepositoryName == repositoryName && e.ReceivedAt >= since);
}
