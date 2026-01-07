using AssetManagement.Application.DTOs;

namespace AssetManagement.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponseDto<LoginResponseDto>> LoginAsync(LoginDto loginDto);
        Task<ApiResponseDto<bool>> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        string GenerateJwtToken(UserResponseDto user);
    }
}