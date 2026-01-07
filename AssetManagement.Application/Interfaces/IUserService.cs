using AssetManagement.Application.DTOs;

namespace AssetManagement.Application.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponseDto<UserResponseDto>> RegisterUserAsync(UserRegistrationDto registrationDto);
        Task<ApiResponseDto<UserResponseDto>> GetUserByIdAsync(int id);
        Task<ApiResponseDto<UserResponseDto>> GetUserByEmailAsync(string email);
        Task<ApiResponseDto<IEnumerable<UserResponseDto>>> GetAllUsersAsync();
        Task<ApiResponseDto<bool>> DeleteUserAsync(int id);
    }
}