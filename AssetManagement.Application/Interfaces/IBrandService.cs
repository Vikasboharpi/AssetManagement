using AssetManagement.Application.DTOs;

namespace AssetManagement.Application.Interfaces
{
    public interface IBrandService
    {
        Task<ApiResponseDto<BrandDto>> GetBrandByIdAsync(int id);
        Task<ApiResponseDto<IEnumerable<BrandDto>>> GetAllBrandsAsync();
        Task<ApiResponseDto<IEnumerable<BrandDto>>> GetActiveBrandsAsync();
        Task<ApiResponseDto<BrandDto>> CreateBrandAsync(CreateBrandDto createDto);
        Task<ApiResponseDto<BrandDto>> UpdateBrandAsync(int id, UpdateBrandDto updateDto);
        Task<ApiResponseDto<bool>> DeleteBrandAsync(int id);
    }
}