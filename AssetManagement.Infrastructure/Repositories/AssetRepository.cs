using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Interfaces;
using AssetManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Infrastructure.Repositories
{
    public class AssetRepository : IAssetRepository
    {
        private readonly AppDbContext _context;

        public AssetRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Asset?> GetByIdAsync(int id)
        {
            return await _context.Assets
                .Include(a => a.Category)
                .Include(a => a.Brand)
                .Include(a => a.Location)
                .Include(a => a.CreatedByUser)
                .Include(a => a.AssignedToUser)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Asset?> GetBySerialNumberAsync(string serialNumber)
        {
            return await _context.Assets
                .Include(a => a.Category)
                .Include(a => a.Brand)
                .Include(a => a.Location)
                .Include(a => a.CreatedByUser)
                .Include(a => a.AssignedToUser)
                .FirstOrDefaultAsync(a => a.SerialNumber == serialNumber);
        }

        public async Task<IEnumerable<Asset>> GetAllAsync()
        {
            return await _context.Assets
                .Include(a => a.Category)
                .Include(a => a.Brand)
                .Include(a => a.Location)
                .Include(a => a.CreatedByUser)
                .Include(a => a.AssignedToUser)
                .Where(a => a.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Asset>> GetByUserIdAsync(int userId)
        {
            return await _context.Assets
                .Include(a => a.Category)
                .Include(a => a.Brand)
                .Include(a => a.Location)
                .Include(a => a.CreatedByUser)
                .Include(a => a.AssignedToUser)
                .Where(a => a.AssignedToUserId == userId && a.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Asset>> GetByCategoryIdAsync(int categoryId)
        {
            return await _context.Assets
                .Include(a => a.Category)
                .Include(a => a.Brand)
                .Include(a => a.Location)
                .Include(a => a.CreatedByUser)
                .Include(a => a.AssignedToUser)
                .Where(a => a.CategoryId == categoryId && a.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Asset>> GetByLocationIdAsync(int locationId)
        {
            return await _context.Assets
                .Include(a => a.Category)
                .Include(a => a.Brand)
                .Include(a => a.Location)
                .Include(a => a.CreatedByUser)
                .Include(a => a.AssignedToUser)
                .Where(a => a.LocationId == locationId && a.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Asset>> GetByStatusAsync(AssetStatus status)
        {
            return await _context.Assets
                .Include(a => a.Category)
                .Include(a => a.Brand)
                .Include(a => a.Location)
                .Include(a => a.CreatedByUser)
                .Include(a => a.AssignedToUser)
                .Where(a => a.Status == status && a.IsActive)
                .ToListAsync();
        }

        public async Task<Asset> CreateAsync(Asset asset)
        {
            _context.Assets.Add(asset);
            await _context.SaveChangesAsync();

            // Load related entities
            await _context.Entry(asset)
                .Reference(a => a.Category)
                .LoadAsync();
            await _context.Entry(asset)
                .Reference(a => a.Brand)
                .LoadAsync();
            await _context.Entry(asset)
                .Reference(a => a.Location)
                .LoadAsync();
            await _context.Entry(asset)
                .Reference(a => a.CreatedByUser)
                .LoadAsync();

            return asset;
        }

        public async Task<Asset> UpdateAsync(Asset asset)
        {
            asset.UpdatedAt = DateTime.UtcNow;
            _context.Assets.Update(asset);
            await _context.SaveChangesAsync();

            // Load related entities
            await _context.Entry(asset)
                .Reference(a => a.Category)
                .LoadAsync();
            await _context.Entry(asset)
                .Reference(a => a.Brand)
                .LoadAsync();
            await _context.Entry(asset)
                .Reference(a => a.Location)
                .LoadAsync();
            await _context.Entry(asset)
                .Reference(a => a.CreatedByUser)
                .LoadAsync();
            if (asset.AssignedToUserId.HasValue)
            {
                await _context.Entry(asset)
                    .Reference(a => a.AssignedToUser)
                    .LoadAsync();
            }

            return asset;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var asset = await GetByIdAsync(id);
            if (asset == null) return false;

            asset.IsActive = false;
            asset.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SerialNumberExistsAsync(string serialNumber, int? excludeAssetId = null)
        {
            var query = _context.Assets.Where(a => a.SerialNumber == serialNumber && a.IsActive);
            
            if (excludeAssetId.HasValue)
            {
                query = query.Where(a => a.Id != excludeAssetId.Value);
            }

            return await query.AnyAsync();
        }
    }
}