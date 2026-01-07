using AssetManagement.Application.DTOs;
using AssetManagement.Application.Interfaces;
using AssetManagement.Domain.Entities;
using AssetManagement.API.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AssetManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AssetsController : ControllerBase
    {
        private readonly IAssetService _assetService;
        private readonly ILogger<AssetsController> _logger;

        public AssetsController(IAssetService assetService, ILogger<AssetsController> logger)
        {
            _assetService = assetService;
            _logger = logger;
        }

        /// <summary>
        /// Register a new asset (Admin, Manager, and IT Support only)
        /// </summary>
        /// <param name="registrationDto">Asset registration details</param>
        /// <returns>Registered asset information</returns>
        [HttpPost("register")]
        [AdminManagerIT]
        public async Task<ActionResult<ApiResponseDto<AssetResponseDto>>> RegisterAsset(
            [FromBody] AssetRegistrationDto registrationDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponseDto<AssetResponseDto>.ErrorResponse(
                    "Validation failed", errors));
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized(ApiResponseDto<AssetResponseDto>.ErrorResponse("Invalid user token"));
            }

            var result = await _assetService.RegisterAssetAsync(registrationDto, userId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetAssetById), 
                new { id = result.Data!.Id }, result);
        }

        /// <summary>
        /// Get asset by ID
        /// </summary>
        /// <param name="id">Asset ID</param>
        /// <returns>Asset information</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<AssetResponseDto>>> GetAssetById(int id)
        {
            var result = await _assetService.GetAssetByIdAsync(id);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get asset by serial number
        /// </summary>
        /// <param name="serialNumber">Asset serial number</param>
        /// <returns>Asset information</returns>
        [HttpGet("by-serial/{serialNumber}")]
        public async Task<ActionResult<ApiResponseDto<AssetResponseDto>>> GetAssetBySerialNumber(string serialNumber)
        {
            var result = await _assetService.GetAssetBySerialNumberAsync(serialNumber);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get all assets (Admin and IT Support only)
        /// </summary>
        /// <returns>List of all assets</returns>
        [HttpGet]
        [AdminAndIT]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<AssetResponseDto>>>> GetAllAssets()
        {
            var result = await _assetService.GetAllAssetsAsync();

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get assets assigned to a specific user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of assets assigned to the user</returns>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<AssetResponseDto>>>> GetAssetsByUserId(int userId)
        {
            var result = await _assetService.GetAssetsByUserIdAsync(userId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get assets by category
        /// </summary>
        /// <param name="categoryId">Category ID</param>
        /// <returns>List of assets in the category</returns>
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<AssetResponseDto>>>> GetAssetsByCategory(int categoryId)
        {
            var result = await _assetService.GetAssetsByCategoryIdAsync(categoryId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get assets by location
        /// </summary>
        /// <param name="locationId">Location ID</param>
        /// <returns>List of assets at the location</returns>
        [HttpGet("location/{locationId}")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<AssetResponseDto>>>> GetAssetsByLocation(int locationId)
        {
            var result = await _assetService.GetAssetsByLocationIdAsync(locationId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get assets by status
        /// </summary>
        /// <param name="status">Asset status</param>
        /// <returns>List of assets with the specified status</returns>
        [HttpGet("status/{status}")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<AssetResponseDto>>>> GetAssetsByStatus(AssetStatus status)
        {
            var result = await _assetService.GetAssetsByStatusAsync(status);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Update an existing asset (Admin, Manager, and IT Support only)
        /// </summary>
        /// <param name="id">Asset ID</param>
        /// <param name="updateDto">Asset update details</param>
        /// <returns>Updated asset information</returns>
        [HttpPut("{id}")]
        [AdminManagerIT]
        public async Task<ActionResult<ApiResponseDto<AssetResponseDto>>> UpdateAsset(
            int id, [FromBody] AssetUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponseDto<AssetResponseDto>.ErrorResponse(
                    "Validation failed", errors));
            }

            var result = await _assetService.UpdateAssetAsync(id, updateDto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Assign asset to a user (Admin, Manager, and IT Support only)
        /// </summary>
        /// <param name="id">Asset ID</param>
        /// <param name="assignmentDto">Assignment details</param>
        /// <returns>Updated asset information</returns>
        [HttpPost("{id}/assign")]
        [AdminManagerIT]
        public async Task<ActionResult<ApiResponseDto<AssetResponseDto>>> AssignAsset(
            int id, [FromBody] AssetAssignmentDto assignmentDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponseDto<AssetResponseDto>.ErrorResponse(
                    "Validation failed", errors));
            }

            var result = await _assetService.AssignAssetAsync(id, assignmentDto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Unassign asset from current user (Admin, Manager, and IT Support only)
        /// </summary>
        /// <param name="id">Asset ID</param>
        /// <returns>Updated asset information</returns>
        [HttpPost("{id}/unassign")]
        [AdminManagerIT]
        public async Task<ActionResult<ApiResponseDto<AssetResponseDto>>> UnassignAsset(int id)
        {
            var result = await _assetService.UnassignAssetAsync(id);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Delete asset by ID (Admin only)
        /// </summary>
        /// <param name="id">Asset ID</param>
        /// <returns>Deletion result</returns>
        [HttpDelete("{id}")]
        [AdminOnly]
        public async Task<ActionResult<ApiResponseDto<bool>>> DeleteAsset(int id)
        {
            var result = await _assetService.DeleteAssetAsync(id);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get asset registration options (categories, brands, locations) - Available to all authenticated users
        /// </summary>
        /// <returns>Registration options</returns>
        [HttpGet("registration-options")]
        public async Task<ActionResult<ApiResponseDto<object>>> GetRegistrationOptions()
        {
            var result = await _assetService.GetAssetRegistrationOptionsAsync();

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get current user's assigned assets
        /// </summary>
        /// <returns>List of assets assigned to the current user</returns>
        [HttpGet("my-assets")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<AssetResponseDto>>>> GetMyAssets()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized(ApiResponseDto<IEnumerable<AssetResponseDto>>.ErrorResponse("Invalid user token"));
            }

            var result = await _assetService.GetAssetsByUserIdAsync(userId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}