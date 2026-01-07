using AssetManagement.Application.DTOs;

namespace AssetManagement.Application.Interfaces
{
    public interface IRoleService
    {
        Task<ApiResponseDto<RoleDto>> GetRoleByIdAsync(int id);
        Task<ApiResponseDto<RoleDto>> GetRoleByNameAsync(string name);
        Task<ApiResponseDto<IEnumerable<RoleDto>>> GetAllRolesAsync();
        Task<ApiResponseDto<IEnumerable<RoleDto>>> GetActiveRolesAsync();
        Task<ApiResponseDto<RoleDto>> CreateRoleAsync(CreateRoleDto createRoleDto);
        Task<ApiResponseDto<RoleDto>> UpdateRoleAsync(int id, UpdateRoleDto updateRoleDto);
        Task<ApiResponseDto<bool>> DeleteRoleAsync(int id);
    }
}