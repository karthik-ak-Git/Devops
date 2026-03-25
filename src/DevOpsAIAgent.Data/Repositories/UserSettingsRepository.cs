using DevOpsAIAgent.Core.Interfaces.Repositories;
using DevOpsAIAgent.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DevOpsAIAgent.Data.Repositories;

public class UserSettingsRepository : IUserSettingsRepository
{
    private readonly ApplicationDbContext _context;

    public UserSettingsRepository(ApplicationDbContext context) => _context = context;

    public async Task<UserSettings?> GetByIdAsync(long id) =>
        await _context.UserSettings.FirstOrDefaultAsync(s => s.Id == id);

    public async Task<UserSettings?> GetByKeyAsync(string key) =>
        await _context.UserSettings.FirstOrDefaultAsync(s => s.Key == key);

    public async Task<IReadOnlyList<UserSettings>> GetAllAsync() =>
        await _context.UserSettings.OrderBy(s => s.Key).ToListAsync();

    public async Task<UserSettings> AddAsync(UserSettings entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _context.UserSettings.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<UserSettings> UpdateAsync(UserSettings entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _context.UserSettings.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var entity = await _context.UserSettings.FindAsync(id);
        if (entity == null)
            return false;

        _context.UserSettings.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(string key) =>
        await _context.UserSettings.AnyAsync(s => s.Key == key);
}