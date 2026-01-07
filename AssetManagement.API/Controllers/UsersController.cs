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
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, IRoleService roleService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _roleService = roleService;
            _logger = logger;
        }

        /// <summary>
        /// Register a new user (Admin and IT Support only)
        /// </summary>
        /// <param name="registrationDto">User registration details</param>
        /// <returns>Registered user information</returns>
        [HttpPost("register")]
        [AdminAndIT]
        public async Task<ActionResult<ApiResponseDto<UserResponseDto>>> RegisterUser(
            [FromBody] UserRegistrationDto registrationDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponseDto<UserResponseDto>.ErrorResponse(
                    "Validation failed", errors));
            }

            var result = await _userService.RegisterUserAsync(registrationDto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetUserById), 
                new { id = result.Data!.Id }, result);
        }

        /// <summary>
        /// Get user by ID (Admin and IT Support only)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User information</returns>
        [HttpGet("{id}")]
        [AdminAndIT]
        public async Task<ActionResult<ApiResponseDto<UserResponseDto>>> GetUserById(int id)
        {
            var result = await _userService.GetUserByIdAsync(id);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get user by email (Admin and IT Support only)
        /// </summary>
        /// <param name="email">User email</param>
        /// <returns>User information</returns>
        [HttpGet("by-email/{email}")]
        [AdminAndIT]
        public async Task<ActionResult<ApiResponseDto<UserResponseDto>>> GetUserByEmail(string email)
        {
            var result = await _userService.GetUserByEmailAsync(email);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get all users (Admin and IT Support only)
        /// </summary>
        /// <returns>List of all users</returns>
        [HttpGet]
        [AdminAndIT]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<UserResponseDto>>>> GetAllUsers()
        {
            var result = await _userService.GetAllUsersAsync();

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Delete user by ID (Admin only)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Deletion result</returns>
        [HttpDelete("{id}")]
        [AdminOnly]
        public async Task<ActionResult<ApiResponseDto<bool>>> DeleteUser(int id)
        {
            var result = await _userService.DeleteUserAsync(id);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get available departments, sub-departments, and roles (Public access for registration)
        /// </summary>
        /// <returns>Department and role options</returns>
        [HttpGet("registration-options")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponseDto<object>>> GetRegistrationOptions()
        {
            var rolesResult = await _roleService.GetActiveRolesAsync();
            
            var options = new
            {
                Departments = new
                {
                    ITDepartment = new
                    {
                        Name = "IT Department",
                        SubDepartments = new[]
                        {
                            "Hardware IT",
                            "Software IT",
                            "Network Administration",
                            "Cybersecurity",
                            "Database Administration"
                        }
                    },
                    NonITDepartment = new
                    {
                        Name = "Non-IT Department",
                        SubDepartments = new[]
                        {
                            "HR",
                            "Aadhar",
                            "KYC",
                            "GST",
                            "Finance",
                            "Marketing",
                            "Sales",
                            "Operations",
                            "Legal",
                            "Administration"
                        }
                    }
                },
                Roles = rolesResult.Success ? rolesResult.Data : new List<RoleDto>()
            };

            return Ok(ApiResponseDto<object>.SuccessResponse(options, 
                "Registration options retrieved successfully"));
        }

        /// <summary>
        /// Get available departments and sub-departments (Public access for registration)
        /// </summary>
        /// <returns>Department options</returns>
        [HttpGet("departments")]
        [AllowAnonymous]
        public ActionResult<ApiResponseDto<object>> GetDepartments()
        {
            var departments = new
            {
                ITDepartment = new
                {
                    Name = "IT Department",
                    SubDepartments = new[]
                    {
                        "Hardware IT",
                        "Software IT",
                        "Network Administration",
                        "Cybersecurity",
                        "Database Administration"
                    }
                },
                NonITDepartment = new
                {
                    Name = "Non-IT Department",
                    SubDepartments = new[]
                    {
                        "HR",
                        "Aadhar",
                        "KYC",
                        "GST",
                        "Finance",
                        "Marketing",
                        "Sales",
                        "Operations",
                        "Legal",
                        "Administration"
                    }
                }
            };

            return Ok(ApiResponseDto<object>.SuccessResponse(departments, 
                "Departments retrieved successfully"));
        }

        /// <summary>
        /// Get sub-departments for a specific department type (Public access for registration)
        /// </summary>
        /// <param name="departmentType">Department type (IT or NonIT)</param>
        /// <returns>Sub-department options for the specified department</returns>
        [HttpGet("departments/{departmentType}/sub-departments")]
        [AllowAnonymous]
        public ActionResult<ApiResponseDto<object>> GetSubDepartments(string departmentType)
        {
            var subDepartments = departmentType.ToLower() switch
            {
                "it" or "itdepartment" => new[]
                {
                    "Hardware IT",
                    "Software IT", 
                    "Network Administration",
                    "Cybersecurity",
                    "Database Administration"
                },
                "nonit" or "nonitdepartment" => new[]
                {
                    "HR",
                    "Aadhar", 
                    "KYC",
                    "GST",
                    "Finance",
                    "Marketing",
                    "Sales", 
                    "Operations",
                    "Legal",
                    "Administration"
                },
                _ => Array.Empty<string>()
            };

            if (!subDepartments.Any())
            {
                return BadRequest(ApiResponseDto<object>.ErrorResponse(
                    "Invalid department type. Use 'IT' or 'NonIT'"));
            }

            var result = new
            {
                DepartmentType = departmentType,
                SubDepartments = subDepartments
            };

            return Ok(ApiResponseDto<object>.SuccessResponse(result, 
                $"Sub-departments for {departmentType} retrieved successfully"));
        }
    }
}