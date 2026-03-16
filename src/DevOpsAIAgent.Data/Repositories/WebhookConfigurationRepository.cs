using DevOpsAIAgent.Core.Interfaces.Repositories;
using DevOpsAIAgent.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DevOpsAIAgent.Data.Repositories;

public class WebhookConfigurationRepository : IWebhookConfigurationRepository
{
    private readonly ApplicationDbContext _context;

    public WebhookConfigurationRepository(ApplicationDbContext context) => _context = context;

    public async Task<WebhookConfiguration?> GetByRepositoryAsync(string repositoryFullName) =>
        await _context.WebhookConfigurations.FirstOrDefaultAsync(w => w.RepositoryFullName == repositoryFullName);

    public async Task<IReadOnlyList<WebhookConfiguration>> GetAllAsync() =>
        await _context.WebhookConfigurations.OrderBy(w => w.RepositoryFullName).ToListAsync();

    public async Task<WebhookConfiguration> AddAsync(WebhookConfiguration entity)
    {
        _context.WebhookConfigurations.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(WebhookConfiguration entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _context.WebhookConfigurations.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await _context.WebhookConfigurations.FindAsync(id);
        if (entity != null)
        {
            _context.WebhookConfigurations.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
