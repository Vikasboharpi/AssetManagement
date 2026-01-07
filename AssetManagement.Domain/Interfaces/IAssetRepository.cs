using AssetManagement.Domain.Entities;

namespace AssetManagement.Domain.Interfaces
{
    public interface IAssetRepository
    {
        Task<Asset?> GetByIdAsync(int id);
        Task<Asset?> GetBySerialNumberAsync(string serialNumber);
        Task<IEnumerable<Asset>> GetAllAsync();
        Task<IEnumerable<Asset>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Asset>> GetByCategoryIdAsync(int categoryId);
        Task<IEnumerable<Asset>> GetByLocationIdAsync(int locationId);
        Task<IEnumerable<Asset>> GetByStatusAsync(AssetStatus status);
        Task<Asset> CreateAsync(Asset asset);
        Task<Asset> UpdateAsync(Asset asset);
        Task<bool> DeleteAsync(int id);
        Task<bool> SerialNumberExistsAsync(string serialNumber, int? excludeAssetId = null);
    }
}