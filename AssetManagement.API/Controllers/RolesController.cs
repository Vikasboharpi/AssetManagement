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
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly ILogger<RolesController> _logger;

        public RolesController(IRoleService roleService, ILogger<RolesController> logger)
        {
            _roleService = roleService;
            _logger = logger;
        }

        /// <summary>
        /// Get all roles
        /// </summary>
        /// <returns>List of all roles</returns>
        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<RoleDto>>>> GetAllRoles()
        {
            var result = await _roleService.GetAllRolesAsync();

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get all active roles
        /// </summary>
        /// <returns>List of active roles</returns>
        [HttpGet("active")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<RoleDto>>>> GetActiveRoles()
        {
            var result = await _roleService.GetActiveRolesAsync();

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get role by ID
        /// </summary>
        /// <param name="id">Role ID</param>
        /// <returns>Role information</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<RoleDto>>> GetRoleById(int id)
        {
            var result = await _roleService.GetRoleByIdAsync(id);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get role by name
        /// </summary>
        /// <param name="name">Role name</param>
        /// <returns>Role information</returns>
        [HttpGet("by-name/{name}")]
        public async Task<ActionResult<ApiResponseDto<RoleDto>>> GetRoleByName(string name)
        {
            var result = await _roleService.GetRoleByNameAsync(name);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Create a new role (Admin only)
        /// </summary>
        /// <param name="createRoleDto">Role creation details</param>
        /// <returns>Created role information</returns>
        [HttpPost]
        [AdminOnly]
        public async Task<ActionResult<ApiResponseDto<RoleDto>>> CreateRole(
            [FromBody] CreateRoleDto createRoleDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponseDto<RoleDto>.ErrorResponse(
                    "Validation failed", errors));
            }

            var result = await _roleService.CreateRoleAsync(createRoleDto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetRoleById), 
                new { id = result.Data!.Id }, result);
        }

        /// <summary>
        /// Update an existing role (Admin only)
        /// </summary>
        /// <param name="id">Role ID</param>
        /// <param name="updateRoleDto">Role update details</param>
        /// <returns>Updated role information</returns>
        [HttpPut("{id}")]
        [AdminOnly]
        public async Task<ActionResult<ApiResponseDto<RoleDto>>> UpdateRole(
            int id, [FromBody] UpdateRoleDto updateRoleDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponseDto<RoleDto>.ErrorResponse(
                    "Validation failed", errors));
            }

            var result = await _roleService.UpdateRoleAsync(id, updateRoleDto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Delete role by ID (Admin only)
        /// </summary>
        /// <param name="id">Role ID</param>
        /// <returns>Deletion result</returns>
        [HttpDelete("{id}")]
        [AdminOnly]
        public async Task<ActionResult<ApiResponseDto<bool>>> DeleteRole(int id)
        {
            var result = await _roleService.DeleteRoleAsync(id);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}