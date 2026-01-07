using AssetManagement.Domain.Entities;

namespace AssetManagement.Domain.Interfaces
{
    public interface ILocationRepository
    {
        Task<Location?> GetByIdAsync(int id);
        Task<Location?> GetByNameAsync(string name);
        Task<IEnumerable<Location>> GetAllAsync();
        Task<IEnumerable<Location>> GetActiveAsync();
        Task<Location> CreateAsync(Location location);
        Task<Location> UpdateAsync(Location location);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(string name, int? excludeId = null);
    }
}