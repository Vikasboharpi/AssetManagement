using AssetManagement.Application.DTOs;

namespace AssetManagement.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<ApiResponseDto<CategoryDto>> GetCategoryByIdAsync(int id);
        Task<ApiResponseDto<IEnumerable<CategoryDto>>> GetAllCategoriesAsync();
        Task<ApiResponseDto<IEnumerable<CategoryDto>>> GetActiveCategoriesAsync();
        Task<ApiResponseDto<CategoryDto>> CreateCategoryAsync(CreateCategoryDto createDto);
        Task<ApiResponseDto<CategoryDto>> UpdateCategoryAsync(int id, UpdateCategoryDto updateDto);
        Task<ApiResponseDto<bool>> DeleteCategoryAsync(int id);
    }
}