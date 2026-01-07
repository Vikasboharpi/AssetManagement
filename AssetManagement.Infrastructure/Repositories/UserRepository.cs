using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Interfaces;
using AssetManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Users?> GetByIdAsync(int id)
        {
            return await _context.Employees
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Users?> GetByEmailAsync(string email)
        {
            return await _context.Employees
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<Users?> GetByEmploymentIdAsync(string employmentId)
        {
            return await _context.Employees
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.EmploymentId == employmentId);
        }

        public async Task<IEnumerable<Users>> GetAllAsync()
        {
            return await _context.Employees
                .Include(u => u.Role)
                .ToListAsync();
        }

        public async Task<Users> CreateAsync(Users user)
        {
            _context.Employees.Add(user);
            await _context.SaveChangesAsync();
            
            // Load the role information
            await _context.Entry(user)
                .Reference(u => u.Role)
                .LoadAsync();
                
            return user;
        }

        public async Task<Users> UpdateAsync(Users user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            _context.Employees.Update(user);
            await _context.SaveChangesAsync();
            
            // Load the role information
            await _context.Entry(user)
                .Reference(u => u.Role)
                .LoadAsync();
                
            return user;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await GetByIdAsync(id);
            if (user == null) return false;

            _context.Employees.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(string email, string employmentId)
        {
            return await _context.Employees
                .AnyAsync(u => u.Email.ToLower() == email.ToLower() || u.EmploymentId == employmentId);
        }
    }
}