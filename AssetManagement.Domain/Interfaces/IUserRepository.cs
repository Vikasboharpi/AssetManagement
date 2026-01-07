using AssetManagement.Domain.Entities;

namespace AssetManagement.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<Users?> GetByIdAsync(int id);
        Task<Users?> GetByEmailAsync(string email);
        Task<Users?> GetByEmploymentIdAsync(string employmentId);
        Task<IEnumerable<Users>> GetAllAsync();
        Task<Users> CreateAsync(Users user);
        Task<Users> UpdateAsync(Users user);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(string email, string employmentId);
    }
}