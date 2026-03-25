using DevOpsAIAgent.Core.Models;

namespace DevOpsAIAgent.Core.Interfaces.Repositories;

public interface IUserSettingsRepository
{
    Task<UserSettings?> GetByIdAsync(long id);
    Task<UserSettings?> GetByKeyAsync(string key);
    Task<IReadOnlyList<UserSettings>> GetAllAsync();
    Task<UserSettings> AddAsync(UserSettings entity);
    Task<UserSettings> UpdateAsync(UserSettings entity);
    Task<bool> DeleteAsync(long id);
    Task<bool> ExistsAsync(string key);
}