using AssetManagement.Application.DTOs;
using AssetManagement.Application.Interfaces;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace AssetManagement.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<RoleService> _logger;

        public RoleService(IRoleRepository roleRepository, IMapper mapper, ILogger<RoleService> logger)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponseDto<RoleDto>> GetRoleByIdAsync(int id)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(id);
                if (role == null)
                {
                    return ApiResponseDto<RoleDto>.ErrorResponse("Role not found");
                }

                var roleDto = _mapper.Map<RoleDto>(role);
                return ApiResponseDto<RoleDto>.SuccessResponse(roleDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting role by ID: {RoleId}", id);
                return ApiResponseDto<RoleDto>.ErrorResponse("An error occurred while retrieving the role");
            }
        }

        public async Task<ApiResponseDto<RoleDto>> GetRoleByNameAsync(string name)
        {
            try
            {
                var role = await _roleRepository.GetByNameAsync(name);
                if (role == null)
                {
                    return ApiResponseDto<RoleDto>.ErrorResponse("Role not found");
                }

                var roleDto = _mapper.Map<RoleDto>(role);
                return ApiResponseDto<RoleDto>.SuccessResponse(roleDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting role by name: {RoleName}", name);
                return ApiResponseDto<RoleDto>.ErrorResponse("An error occurred while retrieving the role");
            }
        }

        public async Task<ApiResponseDto<IEnumerable<RoleDto>>> GetAllRolesAsync()
        {
            try
            {
                var roles = await _roleRepository.GetAllAsync();
                var roleDtos = _mapper.Map<IEnumerable<RoleDto>>(roles);
                return ApiResponseDto<IEnumerable<RoleDto>>.SuccessResponse(roleDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all roles");
                return ApiResponseDto<IEnumerable<RoleDto>>.ErrorResponse("An error occurred while retrieving roles");
            }
        }

        public async Task<ApiResponseDto<IEnumerable<RoleDto>>> GetActiveRolesAsync()
        {
            try
            {
                var roles = await _roleRepository.GetActiveRolesAsync();
                var roleDtos = _mapper.Map<IEnumerable<RoleDto>>(roles);
                return ApiResponseDto<IEnumerable<RoleDto>>.SuccessResponse(roleDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting active roles");
                return ApiResponseDto<IEnumerable<RoleDto>>.ErrorResponse("An error occurred while retrieving active roles");
            }
        }

        public async Task<ApiResponseDto<RoleDto>> CreateRoleAsync(CreateRoleDto createRoleDto)
        {
            try
            {
                // Check if role already exists
                var existingRole = await _roleRepository.ExistsAsync(createRoleDto.Name);
                if (existingRole)
                {
                    return ApiResponseDto<RoleDto>.ErrorResponse("Role with this name already exists");
                }

                var role = new Role
                {
                    Name = createRoleDto.Name,
                    Description = createRoleDto.Description,
                    IsActive = createRoleDto.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var createdRole = await _roleRepository.CreateAsync(role);
                var roleDto = _mapper.Map<RoleDto>(createdRole);

                _logger.LogInformation("Role created successfully with ID: {RoleId}", createdRole.Id);
                return ApiResponseDto<RoleDto>.SuccessResponse(roleDto, "Role created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating role");
                return ApiResponseDto<RoleDto>.ErrorResponse("An error occurred while creating the role");
            }
        }

        public async Task<ApiResponseDto<RoleDto>> UpdateRoleAsync(int id, UpdateRoleDto updateRoleDto)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(id);
                if (role == null)
                {
                    return ApiResponseDto<RoleDto>.ErrorResponse("Role not found");
                }

                // Check if another role with the same name exists
                var existingRole = await _roleRepository.GetByNameAsync(updateRoleDto.Name);
                if (existingRole != null && existingRole.Id != id)
                {
                    return ApiResponseDto<RoleDto>.ErrorResponse("Another role with this name already exists");
                }

                role.Name = updateRoleDto.Name;
                role.Description = updateRoleDto.Description;
                role.IsActive = updateRoleDto.IsActive;

                var updatedRole = await _roleRepository.UpdateAsync(role);
                var roleDto = _mapper.Map<RoleDto>(updatedRole);

                _logger.LogInformation("Role updated successfully with ID: {RoleId}", updatedRole.Id);
                return ApiResponseDto<RoleDto>.SuccessResponse(roleDto, "Role updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating role with ID: {RoleId}", id);
                return ApiResponseDto<RoleDto>.ErrorResponse("An error occurred while updating the role");
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteRoleAsync(int id)
        {
            try
            {
                var result = await _roleRepository.DeleteAsync(id);
                if (!result)
                {
                    return ApiResponseDto<bool>.ErrorResponse("Role not found");
                }

                _logger.LogInformation("Role deleted successfully with ID: {RoleId}", id);
                return ApiResponseDto<bool>.SuccessResponse(true, "Role deleted successfully");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Cannot delete role with ID: {RoleId} - has users assigned", id);
                return ApiResponseDto<bool>.ErrorResponse("Cannot delete role that has users assigned to it");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting role with ID: {RoleId}", id);
                return ApiResponseDto<bool>.ErrorResponse("An error occurred while deleting the role");
            }
        }
    }
}