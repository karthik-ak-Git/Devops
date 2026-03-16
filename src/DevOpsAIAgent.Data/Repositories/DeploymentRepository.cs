using DevOpsAIAgent.Core.Interfaces.Repositories;
using DevOpsAIAgent.Core.Models;
using DevOpsAIAgent.Core.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace DevOpsAIAgent.Data.Repositories;

public class DeploymentRepository : IDeploymentRepository
{
    private readonly ApplicationDbContext _context;

    public DeploymentRepository(ApplicationDbContext context) => _context = context;

    public async Task<Deployment?> GetByIdAsync(long id) =>
        await _context.Deployments.FindAsync(id);

    public async Task<IReadOnlyList<Deployment>> GetRecentAsync(int count = 50) =>
        await _context.Deployments.OrderByDescending(d => d.StartedAt).Take(count).ToListAsync();

    public async Task<IReadOnlyList<Deployment>> GetByRepositoryAsync(string repositoryName, int count = 50) =>
        await _context.Deployments.Where(d => d.RepositoryName == repositoryName)
            .OrderByDescending(d => d.StartedAt).Take(count).ToListAsync();

    public async Task<Deployment> AddAsync(Deployment entity)
    {
        _context.Deployments.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(Deployment entity)
    {
        _context.Deployments.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<int> GetCountByStatusAsync(DeploymentStatus status) =>
        await _context.Deployments.CountAsync(d => d.Status == status);
}
