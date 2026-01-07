using AssetManagement.Application.DTOs;
using AssetManagement.Application.Interfaces;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace AssetManagement.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper, ILogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponseDto<CategoryDto>> GetCategoryByIdAsync(int id)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(id);
                if (category == null)
                {
                    return ApiResponseDto<CategoryDto>.ErrorResponse("Category not found");
                }

                var categoryDto = _mapper.Map<CategoryDto>(category);
                return ApiResponseDto<CategoryDto>.SuccessResponse(categoryDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting category by ID: {CategoryId}", id);
                return ApiResponseDto<CategoryDto>.ErrorResponse("An error occurred while retrieving the category");
            }
        }

        public async Task<ApiResponseDto<IEnumerable<CategoryDto>>> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _categoryRepository.GetAllAsync();
                var categoryDtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);
                return ApiResponseDto<IEnumerable<CategoryDto>>.SuccessResponse(categoryDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all categories");
                return ApiResponseDto<IEnumerable<CategoryDto>>.ErrorResponse("An error occurred while retrieving categories");
            }
        }

        public async Task<ApiResponseDto<IEnumerable<CategoryDto>>> GetActiveCategoriesAsync()
        {
            try
            {
                var categories = await _categoryRepository.GetActiveAsync();
                var categoryDtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);
                return ApiResponseDto<IEnumerable<CategoryDto>>.SuccessResponse(categoryDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting active categories");
                return ApiResponseDto<IEnumerable<CategoryDto>>.ErrorResponse("An error occurred while retrieving active categories");
            }
        }

        public async Task<ApiResponseDto<CategoryDto>> CreateCategoryAsync(CreateCategoryDto createDto)
        {
            try
            {
                // Check if category already exists
                var existingCategory = await _categoryRepository.ExistsAsync(createDto.Name);
                if (existingCategory)
                {
                    return ApiResponseDto<CategoryDto>.ErrorResponse("Category with this name already exists");
                }

                var category = new Category
                {
                    Name = createDto.Name,
                    Description = createDto.Description,
                    IsActive = createDto.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var createdCategory = await _categoryRepository.CreateAsync(category);
                var categoryDto = _mapper.Map<CategoryDto>(createdCategory);

                _logger.LogInformation("Category created successfully with ID: {CategoryId}", createdCategory.Id);
                return ApiResponseDto<CategoryDto>.SuccessResponse(categoryDto, "Category created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating category");
                return ApiResponseDto<CategoryDto>.ErrorResponse("An error occurred while creating the category");
            }
        }

        public async Task<ApiResponseDto<CategoryDto>> UpdateCategoryAsync(int id, UpdateCategoryDto updateDto)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(id);
                if (category == null)
                {
                    return ApiResponseDto<CategoryDto>.ErrorResponse("Category not found");
                }

                // Check if another category with the same name exists
                var existingCategory = await _categoryRepository.ExistsAsync(updateDto.Name, id);
                if (existingCategory)
                {
                    return ApiResponseDto<CategoryDto>.ErrorResponse("Another category with this name already exists");
                }

                category.Name = updateDto.Name;
                category.Description = updateDto.Description;
                category.IsActive = updateDto.IsActive;

                var updatedCategory = await _categoryRepository.UpdateAsync(category);
                var categoryDto = _mapper.Map<CategoryDto>(updatedCategory);

                _logger.LogInformation("Category updated successfully with ID: {CategoryId}", updatedCategory.Id);
                return ApiResponseDto<CategoryDto>.SuccessResponse(categoryDto, "Category updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating category with ID: {CategoryId}", id);
                return ApiResponseDto<CategoryDto>.ErrorResponse("An error occurred while updating the category");
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteCategoryAsync(int id)
        {
            try
            {
                var result = await _categoryRepository.DeleteAsync(id);
                if (!result)
                {
                    return ApiResponseDto<bool>.ErrorResponse("Category not found");
                }

                _logger.LogInformation("Category deleted successfully with ID: {CategoryId}", id);
                return ApiResponseDto<bool>.SuccessResponse(true, "Category deleted successfully");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Cannot delete category with ID: {CategoryId} - has assets assigned", id);
                return ApiResponseDto<bool>.ErrorResponse("Cannot delete category that has assets assigned to it");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting category with ID: {CategoryId}", id);
                return ApiResponseDto<bool>.ErrorResponse("An error occurred while deleting the category");
            }
        }
    }
}