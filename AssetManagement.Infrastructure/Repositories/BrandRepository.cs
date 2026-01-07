using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Interfaces;
using AssetManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Infrastructure.Repositories
{
    public class BrandRepository : IBrandRepository
    {
        private readonly AppDbContext _context;

        public BrandRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Brand?> GetByIdAsync(int id)
        {
            return await _context.Brands.FindAsync(id);
        }

        public async Task<Brand?> GetByNameAsync(string name)
        {
            return await _context.Brands
                .FirstOrDefaultAsync(b => b.Name.ToLower() == name.ToLower());
        }

        public async Task<IEnumerable<Brand>> GetAllAsync()
        {
            return await _context.Brands.ToListAsync();
        }

        public async Task<IEnumerable<Brand>> GetActiveAsync()
        {
            return await _context.Brands
                .Where(b => b.IsActive)
                .OrderBy(b => b.Name)
                .ToListAsync();
        }

        public async Task<Brand> CreateAsync(Brand brand)
        {
            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();
            return brand;
        }

        public async Task<Brand> UpdateAsync(Brand brand)
        {
            brand.UpdatedAt = DateTime.UtcNow;
            _context.Brands.Update(brand);
            await _context.SaveChangesAsync();
            return brand;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var brand = await GetByIdAsync(id);
            if (brand == null) return false;

            // Check if brand has assets
            var hasAssets = await _context.Assets.AnyAsync(a => a.BrandId == id && a.IsActive);
            if (hasAssets)
            {
                throw new InvalidOperationException("Cannot delete brand that has assets assigned to it");
            }

            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(string name, int? excludeId = null)
        {
            var query = _context.Brands.Where(b => b.Name.ToLower() == name.ToLower());
            
            if (excludeId.HasValue)
            {
                query = query.Where(b => b.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }
}