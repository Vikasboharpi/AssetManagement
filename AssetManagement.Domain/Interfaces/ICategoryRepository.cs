using AssetManagement.Domain.Entities;

namespace AssetManagement.Domain.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category?> GetByIdAsync(int id);
        Task<Category?> GetByNameAsync(string name);
        Task<IEnumerable<Category>> GetAllAsync();
        Task<IEnumerable<Category>> GetActiveAsync();
        Task<Category> CreateAsync(Category category);
        Task<Category> UpdateAsync(Category category);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(string name, int? excludeId = null);
    }
}