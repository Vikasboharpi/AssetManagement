using AssetManagement.Application.DTOs;
using AssetManagement.Application.Interfaces;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace AssetManagement.Application.Services
{
    public class AssetService : IAssetService
    {
        private readonly IAssetRepository _assetRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AssetService> _logger;

        public AssetService(
            IAssetRepository assetRepository,
            ICategoryRepository categoryRepository,
            IBrandRepository brandRepository,
            ILocationRepository locationRepository,
            IUserRepository userRepository,
            IMapper mapper,
            ILogger<AssetService> logger)
        {
            _assetRepository = assetRepository;
            _categoryRepository = categoryRepository;
            _brandRepository = brandRepository;
            _locationRepository = locationRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponseDto<AssetResponseDto>> RegisterAssetAsync(AssetRegistrationDto registrationDto, int createdByUserId)
        {
            try
            {
                // Check if serial number already exists
                var serialExists = await _assetRepository.SerialNumberExistsAsync(registrationDto.SerialNumber);
                if (serialExists)
                {
                    return ApiResponseDto<AssetResponseDto>.ErrorResponse("Asset with this serial number already exists");
                }

                // Validate category exists
                var category = await _categoryRepository.GetByIdAsync(registrationDto.CategoryId);
                if (category == null || !category.IsActive)
                {
                    return ApiResponseDto<AssetResponseDto>.ErrorResponse("Invalid category selected");
                }

                // Validate brand exists
                var brand = await _brandRepository.GetByIdAsync(registrationDto.BrandId);
                if (brand == null || !brand.IsActive)
                {
                    return ApiResponseDto<AssetResponseDto>.ErrorResponse("Invalid brand selected");
                }

                // Validate location exists
                var location = await _locationRepository.GetByIdAsync(registrationDto.LocationId);
                if (location == null || !location.IsActive)
                {
                    return ApiResponseDto<AssetResponseDto>.ErrorResponse("Invalid location selected");
                }

                // Create asset
                var asset = new Asset
                {
                    Name = registrationDto.Name,
                    SerialNumber = registrationDto.SerialNumber,
                    CategoryId = registrationDto.CategoryId,
                    BrandId = registrationDto.BrandId,
                    LocationId = registrationDto.LocationId,
                    Remarks = registrationDto.Remarks,
                    CreatedByUserId = createdByUserId,
                    Status = AssetStatus.Available,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var createdAsset = await _assetRepository.CreateAsync(asset);
                var assetResponse = _mapper.Map<AssetResponseDto>(createdAsset);

                _logger.LogInformation("Asset registered successfully with ID: {AssetId}", createdAsset.Id);
                return ApiResponseDto<AssetResponseDto>.SuccessResponse(assetResponse, "Asset registered successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering asset");
                return ApiResponseDto<AssetResponseDto>.ErrorResponse("An error occurred while registering the asset");
            }
        }

        public async Task<ApiResponseDto<AssetResponseDto>> GetAssetByIdAsync(int id)
        {
            try
            {
                var asset = await _assetRepository.GetByIdAsync(id);
                if (asset == null)
                {
                    return ApiResponseDto<AssetResponseDto>.ErrorResponse("Asset not found");
                }

                var assetResponse = _mapper.Map<AssetResponseDto>(asset);
                return ApiResponseDto<AssetResponseDto>.SuccessResponse(assetResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting asset by ID: {AssetId}", id);
                return ApiResponseDto<AssetResponseDto>.ErrorResponse("An error occurred while retrieving the asset");
            }
        }

        public async Task<ApiResponseDto<AssetResponseDto>> GetAssetBySerialNumberAsync(string serialNumber)
        {
            try
            {
                var asset = await _assetRepository.GetBySerialNumberAsync(serialNumber);
                if (asset == null)
                {
                    return ApiResponseDto<AssetResponseDto>.ErrorResponse("Asset not found");
                }

                var assetResponse = _mapper.Map<AssetResponseDto>(asset);
                return ApiResponseDto<AssetResponseDto>.SuccessResponse(assetResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting asset by serial number: {SerialNumber}", serialNumber);
                return ApiResponseDto<AssetResponseDto>.ErrorResponse("An error occurred while retrieving the asset");
            }
        }

        public async Task<ApiResponseDto<IEnumerable<AssetResponseDto>>> GetAllAssetsAsync()
        {
            try
            {
                var assets = await _assetRepository.GetAllAsync();
                var assetResponses = _mapper.Map<IEnumerable<AssetResponseDto>>(assets);
                return ApiResponseDto<IEnumerable<AssetResponseDto>>.SuccessResponse(assetResponses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all assets");
                return ApiResponseDto<IEnumerable<AssetResponseDto>>.ErrorResponse("An error occurred while retrieving assets");
            }
        }

        public async Task<ApiResponseDto<IEnumerable<AssetResponseDto>>> GetAssetsByUserIdAsync(int userId)
        {
            try
            {
                var assets = await _assetRepository.GetByUserIdAsync(userId);
                var assetResponses = _mapper.Map<IEnumerable<AssetResponseDto>>(assets);
                return ApiResponseDto<IEnumerable<AssetResponseDto>>.SuccessResponse(assetResponses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting assets for user: {UserId}", userId);
                return ApiResponseDto<IEnumerable<AssetResponseDto>>.ErrorResponse("An error occurred while retrieving user assets");
            }
        }

        public async Task<ApiResponseDto<IEnumerable<AssetResponseDto>>> GetAssetsByCategoryIdAsync(int categoryId)
        {
            try
            {
                var assets = await _assetRepository.GetByCategoryIdAsync(categoryId);
                var assetResponses = _mapper.Map<IEnumerable<AssetResponseDto>>(assets);
                return ApiResponseDto<IEnumerable<AssetResponseDto>>.SuccessResponse(assetResponses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting assets for category: {CategoryId}", categoryId);
                return ApiResponseDto<IEnumerable<AssetResponseDto>>.ErrorResponse("An error occurred while retrieving category assets");
            }
        }

        public async Task<ApiResponseDto<IEnumerable<AssetResponseDto>>> GetAssetsByLocationIdAsync(int locationId)
        {
            try
            {
                var assets = await _assetRepository.GetByLocationIdAsync(locationId);
                var assetResponses = _mapper.Map<IEnumerable<AssetResponseDto>>(assets);
                return ApiResponseDto<IEnumerable<AssetResponseDto>>.SuccessResponse(assetResponses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting assets for location: {LocationId}", locationId);
                return ApiResponseDto<IEnumerable<AssetResponseDto>>.ErrorResponse("An error occurred while retrieving location assets");
            }
        }

        public async Task<ApiResponseDto<IEnumerable<AssetResponseDto>>> GetAssetsByStatusAsync(AssetStatus status)
        {
            try
            {
                var assets = await _assetRepository.GetByStatusAsync(status);
                var assetResponses = _mapper.Map<IEnumerable<AssetResponseDto>>(assets);
                return ApiResponseDto<IEnumerable<AssetResponseDto>>.SuccessResponse(assetResponses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting assets by status: {Status}", status);
                return ApiResponseDto<IEnumerable<AssetResponseDto>>.ErrorResponse("An error occurred while retrieving assets by status");
            }
        }

        public async Task<ApiResponseDto<AssetResponseDto>> UpdateAssetAsync(int id, AssetUpdateDto updateDto)
        {
            try
            {
                var asset = await _assetRepository.GetByIdAsync(id);
                if (asset == null)
                {
                    return ApiResponseDto<AssetResponseDto>.ErrorResponse("Asset not found");
                }

                // Validate category, brand, and location
                var category = await _categoryRepository.GetByIdAsync(updateDto.CategoryId);
                if (category == null || !category.IsActive)
                {
                    return ApiResponseDto<AssetResponseDto>.ErrorResponse("Invalid category selected");
                }

                var brand = await _brandRepository.GetByIdAsync(updateDto.BrandId);
                if (brand == null || !brand.IsActive)
                {
                    return ApiResponseDto<AssetResponseDto>.ErrorResponse("Invalid brand selected");
                }

                var location = await _locationRepository.GetByIdAsync(updateDto.LocationId);
                if (location == null || !location.IsActive)
                {
                    return ApiResponseDto<AssetResponseDto>.ErrorResponse("Invalid location selected");
                }

                // Update asset
                asset.Name = updateDto.Name;
                asset.CategoryId = updateDto.CategoryId;
                asset.BrandId = updateDto.BrandId;
                asset.LocationId = updateDto.LocationId;
                asset.Remarks = updateDto.Remarks;
                asset.Status = updateDto.Status;

                var updatedAsset = await _assetRepository.UpdateAsync(asset);
                var assetResponse = _mapper.Map<AssetResponseDto>(updatedAsset);

                _logger.LogInformation("Asset updated successfully with ID: {AssetId}", updatedAsset.Id);
                return ApiResponseDto<AssetResponseDto>.SuccessResponse(assetResponse, "Asset updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating asset with ID: {AssetId}", id);
                return ApiResponseDto<AssetResponseDto>.ErrorResponse("An error occurred while updating the asset");
            }
        }

        public async Task<ApiResponseDto<AssetResponseDto>> AssignAssetAsync(int assetId, AssetAssignmentDto assignmentDto)
        {
            try
            {
                var asset = await _assetRepository.GetByIdAsync(assetId);
                if (asset == null)
                {
                    return ApiResponseDto<AssetResponseDto>.ErrorResponse("Asset not found");
                }

                if (asset.Status != AssetStatus.Available)
                {
                    return ApiResponseDto<AssetResponseDto>.ErrorResponse("Asset is not available for assignment");
                }

                var user = await _userRepository.GetByIdAsync(assignmentDto.UserId);
                if (user == null || !user.IsActive)
                {
                    return ApiResponseDto<AssetResponseDto>.ErrorResponse("Invalid user selected");
                }

                asset.AssignedToUserId = assignmentDto.UserId;
                asset.Status = AssetStatus.Assigned;
                if (!string.IsNullOrEmpty(assignmentDto.AssignmentRemarks))
                {
                    asset.Remarks = $"{asset.Remarks}\n[Assignment] {assignmentDto.AssignmentRemarks}";
                }

                var updatedAsset = await _assetRepository.UpdateAsync(asset);
                var assetResponse = _mapper.Map<AssetResponseDto>(updatedAsset);

                _logger.LogInformation("Asset assigned successfully. Asset ID: {AssetId}, User ID: {UserId}", assetId, assignmentDto.UserId);
                return ApiResponseDto<AssetResponseDto>.SuccessResponse(assetResponse, "Asset assigned successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while assigning asset with ID: {AssetId}", assetId);
                return ApiResponseDto<AssetResponseDto>.ErrorResponse("An error occurred while assigning the asset");
            }
        }

        public async Task<ApiResponseDto<AssetResponseDto>> UnassignAssetAsync(int assetId)
        {
            try
            {
                var asset = await _assetRepository.GetByIdAsync(assetId);
                if (asset == null)
                {
                    return ApiResponseDto<AssetResponseDto>.ErrorResponse("Asset not found");
                }

                if (asset.Status != AssetStatus.Assigned)
                {
                    return ApiResponseDto<AssetResponseDto>.ErrorResponse("Asset is not currently assigned");
                }

                asset.AssignedToUserId = null;
                asset.Status = AssetStatus.Available;

                var updatedAsset = await _assetRepository.UpdateAsync(asset);
                var assetResponse = _mapper.Map<AssetResponseDto>(updatedAsset);

                _logger.LogInformation("Asset unassigned successfully. Asset ID: {AssetId}", assetId);
                return ApiResponseDto<AssetResponseDto>.SuccessResponse(assetResponse, "Asset unassigned successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while unassigning asset with ID: {AssetId}", assetId);
                return ApiResponseDto<AssetResponseDto>.ErrorResponse("An error occurred while unassigning the asset");
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAssetAsync(int id)
        {
            try
            {
                var result = await _assetRepository.DeleteAsync(id);
                if (!result)
                {
                    return ApiResponseDto<bool>.ErrorResponse("Asset not found");
                }

                _logger.LogInformation("Asset deleted successfully with ID: {AssetId}", id);
                return ApiResponseDto<bool>.SuccessResponse(true, "Asset deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting asset with ID: {AssetId}", id);
                return ApiResponseDto<bool>.ErrorResponse("An error occurred while deleting the asset");
            }
        }

        public async Task<ApiResponseDto<object>> GetAssetRegistrationOptionsAsync()
        {
            try
            {
                var categories = await _categoryRepository.GetActiveAsync();
                var brands = await _brandRepository.GetActiveAsync();
                var locations = await _locationRepository.GetActiveAsync();

                var options = new
                {
                    Categories = _mapper.Map<IEnumerable<CategoryDto>>(categories),
                    Brands = _mapper.Map<IEnumerable<BrandDto>>(brands),
                    Locations = _mapper.Map<IEnumerable<LocationDto>>(locations),
                    AssetStatuses = Enum.GetValues<AssetStatus>().Select(s => new { Value = (int)s, Name = s.ToString() })
                };

                return ApiResponseDto<object>.SuccessResponse(options, "Asset registration options retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting asset registration options");
                return ApiResponseDto<object>.ErrorResponse("An error occurred while retrieving asset registration options");
            }
        }
    }
}