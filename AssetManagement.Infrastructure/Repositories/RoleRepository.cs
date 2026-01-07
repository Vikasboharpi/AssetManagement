using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Interfaces;
using AssetManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _context;

        public RoleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Role?> GetByIdAsync(int id)
        {
            return await _context.Roles
                .Include(r => r.Users)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Role?> GetByNameAsync(string name)
        {
            return await _context.Roles
                .Include(r => r.Users)
                .FirstOrDefaultAsync(r => r.Name.ToLower() == name.ToLower());
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _context.Roles
                .Include(r => r.Users)
                .ToListAsync();
        }

        public async Task<IEnumerable<Role>> GetActiveRolesAsync()
        {
            return await _context.Roles
                .Where(r => r.IsActive)
                .ToListAsync();
        }

        public async Task<Role> CreateAsync(Role role)
        {
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
            return role;
        }

        public async Task<Role> UpdateAsync(Role role)
        {
            role.UpdatedAt = DateTime.UtcNow;
            _context.Roles.Update(role);
            await _context.SaveChangesAsync();
            return role;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var role = await GetByIdAsync(id);
            if (role == null) return false;

            // Check if role has users assigned
            if (role.Users.Any())
            {
                throw new InvalidOperationException("Cannot delete role that has users assigned to it");
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(string name)
        {
            return await _context.Roles
                .AnyAsync(r => r.Name.ToLower() == name.ToLower());
        }
    }
}