using AssetManagement.Domain.Entities;

namespace AssetManagement.Domain.Interfaces
{
    public interface IBrandRepository
    {
        Task<Brand?> GetByIdAsync(int id);
        Task<Brand?> GetByNameAsync(string name);
        Task<IEnumerable<Brand>> GetAllAsync();
        Task<IEnumerable<Brand>> GetActiveAsync();
        Task<Brand> CreateAsync(Brand brand);
        Task<Brand> UpdateAsync(Brand brand);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(string name, int? excludeId = null);
    }
}