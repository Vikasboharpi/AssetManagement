using AssetManagement.Application.DTOs;
using AssetManagement.Application.Interfaces;
using AssetManagement.API.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [AdminAndIT]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        /// <summary>
        /// Get all categories
        /// </summary>
        /// <returns>List of all categories</returns>
        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<CategoryDto>>>> GetAllCategories()
        {
            var result = await _categoryService.GetAllCategoriesAsync();

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get all active categories
        /// </summary>
        /// <returns>List of active categories</returns>
        [HttpGet("active")]
        [AllowAnonymous] // Allow for asset registration dropdown
        public async Task<ActionResult<ApiResponseDto<IEnumerable<CategoryDto>>>> GetActiveCategories()
        {
            var result = await _categoryService.GetActiveCategoriesAsync();

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get category by ID
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Category information</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<CategoryDto>>> GetCategoryById(int id)
        {
            var result = await _categoryService.GetCategoryByIdAsync(id);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Create a new category (Admin only)
        /// </summary>
        /// <param name="createDto">Category creation details</param>
        /// <returns>Created category information</returns>
        [HttpPost]
        [AdminOnly]
        public async Task<ActionResult<ApiResponseDto<CategoryDto>>> CreateCategory(
            [FromBody] CreateCategoryDto createDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponseDto<CategoryDto>.ErrorResponse(
                    "Validation failed", errors));
            }

            var result = await _categoryService.CreateCategoryAsync(createDto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetCategoryById), 
                new { id = result.Data!.Id }, result);
        }

        /// <summary>
        /// Update an existing category (Admin only)
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <param name="updateDto">Category update details</param>
        /// <returns>Updated category information</returns>
        [HttpPut("{id}")]
        [AdminOnly]
        public async Task<ActionResult<ApiResponseDto<CategoryDto>>> UpdateCategory(
            int id, [FromBody] UpdateCategoryDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponseDto<CategoryDto>.ErrorResponse(
                    "Validation failed", errors));
            }

            var result = await _categoryService.UpdateCategoryAsync(id, updateDto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Delete category by ID (Admin only)
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Deletion result</returns>
        [HttpDelete("{id}")]
        [AdminOnly]
        public async Task<ActionResult<ApiResponseDto<bool>>> DeleteCategory(int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}