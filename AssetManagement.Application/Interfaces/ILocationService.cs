using AssetManagement.Application.DTOs;

namespace AssetManagement.Application.Interfaces
{
    public interface ILocationService
    {
        Task<ApiResponseDto<LocationDto>> GetLocationByIdAsync(int id);
        Task<ApiResponseDto<IEnumerable<LocationDto>>> GetAllLocationsAsync();
        Task<ApiResponseDto<IEnumerable<LocationDto>>> GetActiveLocationsAsync();
        Task<ApiResponseDto<LocationDto>> CreateLocationAsync(CreateLocationDto createDto);
        Task<ApiResponseDto<LocationDto>> UpdateLocationAsync(int id, UpdateLocationDto updateDto);
        Task<ApiResponseDto<bool>> DeleteLocationAsync(int id);
    }
}