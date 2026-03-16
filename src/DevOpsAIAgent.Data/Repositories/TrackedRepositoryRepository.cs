using DevOpsAIAgent.Core.Interfaces.Repositories;
using DevOpsAIAgent.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DevOpsAIAgent.Data.Repositories;

public class TrackedRepositoryRepository : ITrackedRepositoryRepository
{
    private readonly ApplicationDbContext _context;

    public TrackedRepositoryRepository(ApplicationDbContext context) => _context = context;

    public async Task<TrackedRepository?> GetByFullNameAsync(string fullName) =>
        await _context.TrackedRepositories.FirstOrDefaultAsync(r => r.FullName == fullName);

    public async Task<IReadOnlyList<TrackedRepository>> GetAllAsync() =>
        await _context.TrackedRepositories.OrderBy(r => r.FullName).ToListAsync();

    public async Task<TrackedRepository> AddOrUpdateAsync(TrackedRepository entity)
    {
        var existing = await GetByFullNameAsync(entity.FullName);
        if (existing != null)
        {
            existing.Owner = entity.Owner;
            existing.Name = entity.Name;
            existing.HtmlUrl = entity.HtmlUrl;
            existing.IsPrivate = entity.IsPrivate;
            existing.WebhookConfigured = entity.WebhookConfigured;
            await _context.SaveChangesAsync();
            return existing;
        }

        _context.TrackedRepositories.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task IncrementBuildsAsync(string fullName, bool isFailed)
    {
        var repo = await GetByFullNameAsync(fullName);
        if (repo != null)
        {
            repo.TotalBuilds++;
            if (isFailed) repo.FailedBuilds++;
            repo.LastBuildAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
