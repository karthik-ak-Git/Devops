using DevOpsAIAgent.Core.Interfaces.Repositories;
using DevOpsAIAgent.Core.Models;
using DevOpsAIAgent.Core.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace DevOpsAIAgent.Data.Repositories;

public class IncidentRepository : IIncidentRepository
{
    private readonly ApplicationDbContext _context;

    public IncidentRepository(ApplicationDbContext context) => _context = context;

    public async Task<Incident?> GetByIdAsync(long id) =>
        await _context.Incidents.Include(i => i.RelatedCiCdEvent).FirstOrDefaultAsync(i => i.Id == id);

    public async Task<IReadOnlyList<Incident>> GetRecentAsync(int count = 50) =>
        await _context.Incidents.Include(i => i.RelatedCiCdEvent)
            .OrderByDescending(i => i.CreatedAt).Take(count).ToListAsync();

    public async Task<IReadOnlyList<Incident>> GetOpenAsync() =>
        await _context.Incidents.Include(i => i.RelatedCiCdEvent)
            .Where(i => i.Status != IncidentStatus.Resolved && i.Status != IncidentStatus.Closed)
            .OrderByDescending(i => i.Severity).ThenByDescending(i => i.CreatedAt).ToListAsync();

    public async Task<Incident> AddAsync(Incident entity)
    {
        _context.Incidents.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(Incident entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Incidents.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<int> GetCountByStatusAsync(IncidentStatus status) =>
        await _context.Incidents.CountAsync(i => i.Status == status);

    public async Task<int> GetCountBySeverityAsync(IncidentSeverity severity) =>
        await _context.Incidents.CountAsync(i => i.Severity == severity
            && i.Status != IncidentStatus.Resolved && i.Status != IncidentStatus.Closed);
}
