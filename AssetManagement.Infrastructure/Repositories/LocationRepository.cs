using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Interfaces;
using AssetManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Infrastructure.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly AppDbContext _context;

        public LocationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Location?> GetByIdAsync(int id)
        {
            return await _context.Locations.FindAsync(id);
        }

        public async Task<Location?> GetByNameAsync(string name)
        {
            return await _context.Locations
                .FirstOrDefaultAsync(l => l.Name.ToLower() == name.ToLower());
        }

        public async Task<IEnumerable<Location>> GetAllAsync()
        {
            return await _context.Locations.ToListAsync();
        }

        public async Task<IEnumerable<Location>> GetActiveAsync()
        {
            return await _context.Locations
                .Where(l => l.IsActive)
                .OrderBy(l => l.Name)
                .ToListAsync();
        }

        public async Task<Location> CreateAsync(Location location)
        {
            _context.Locations.Add(location);
            await _context.SaveChangesAsync();
            return location;
        }

        public async Task<Location> UpdateAsync(Location location)
        {
            location.UpdatedAt = DateTime.UtcNow;
            _context.Locations.Update(location);
            await _context.SaveChangesAsync();
            return location;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var location = await GetByIdAsync(id);
            if (location == null) return false;

            // Check if location has assets
            var hasAssets = await _context.Assets.AnyAsync(a => a.LocationId == id && a.IsActive);
            if (hasAssets)
            {
                throw new InvalidOperationException("Cannot delete location that has assets assigned to it");
            }

            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(string name, int? excludeId = null)
        {
            var query = _context.Locations.Where(l => l.Name.ToLower() == name.ToLower());
            
            if (excludeId.HasValue)
            {
                query = query.Where(l => l.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }
}