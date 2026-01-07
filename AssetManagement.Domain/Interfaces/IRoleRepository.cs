using AssetManagement.Domain.Entities;

namespace AssetManagement.Domain.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(int id);
        Task<Role?> GetByNameAsync(string name);
        Task<IEnumerable<Role>> GetAllAsync();
        Task<IEnumerable<Role>> GetActiveRolesAsync();
        Task<Role> CreateAsync(Role role);
        Task<Role> UpdateAsync(Role role);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(string name);
    }
}