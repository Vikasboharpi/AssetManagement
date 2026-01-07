using AssetManagement.Application.DTOs;
using AssetManagement.Domain.Entities;

namespace AssetManagement.Application.Interfaces
{
    public interface IAssetService
    {
        Task<ApiResponseDto<AssetResponseDto>> RegisterAssetAsync(AssetRegistrationDto registrationDto, int createdByUserId);
        Task<ApiResponseDto<AssetResponseDto>> GetAssetByIdAsync(int id);
        Task<ApiResponseDto<AssetResponseDto>> GetAssetBySerialNumberAsync(string serialNumber);
        Task<ApiResponseDto<IEnumerable<AssetResponseDto>>> GetAllAssetsAsync();
        Task<ApiResponseDto<IEnumerable<AssetResponseDto>>> GetAssetsByUserIdAsync(int userId);
        Task<ApiResponseDto<IEnumerable<AssetResponseDto>>> GetAssetsByCategoryIdAsync(int categoryId);
        Task<ApiResponseDto<IEnumerable<AssetResponseDto>>> GetAssetsByLocationIdAsync(int locationId);
        Task<ApiResponseDto<IEnumerable<AssetResponseDto>>> GetAssetsByStatusAsync(AssetStatus status);
        Task<ApiResponseDto<AssetResponseDto>> UpdateAssetAsync(int id, AssetUpdateDto updateDto);
        Task<ApiResponseDto<AssetResponseDto>> AssignAssetAsync(int assetId, AssetAssignmentDto assignmentDto);
        Task<ApiResponseDto<AssetResponseDto>> UnassignAssetAsync(int assetId);
        Task<ApiResponseDto<bool>> DeleteAssetAsync(int id);
        Task<ApiResponseDto<object>> GetAssetRegistrationOptionsAsync();
    }
}