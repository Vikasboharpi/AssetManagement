using AssetManagement.Application.DTOs;
using AssetManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace AssetManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// User login
        /// </summary>
        /// <param name="loginDto">Login credentials</param>
        /// <returns>JWT token and user information</returns>
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponseDto<LoginResponseDto>>> Login(
            [FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponseDto<LoginResponseDto>.ErrorResponse(
                    "Validation failed", errors));
            }

            var result = await _authService.LoginAsync(loginDto);

            if (!result.Success)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Change user password
        /// </summary>
        /// <param name="changePasswordDto">Password change details</param>
        /// <returns>Success status</returns>
        [HttpPost("change-password")]
        [Authorize]
        public async Task<ActionResult<ApiResponseDto<bool>>> ChangePassword(
            [FromBody] ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponseDto<bool>.ErrorResponse(
                    "Validation failed", errors));
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized(ApiResponseDto<bool>.ErrorResponse("Invalid user token"));
            }

            var result = await _authService.ChangePasswordAsync(
                userId, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get current user information
        /// </summary>
        /// <returns>Current user details</returns>
        [HttpGet("me")]
        [Authorize]
        public ActionResult<ApiResponseDto<object>> GetCurrentUser()
        {
            var userInfo = new
            {
                Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                Name = User.FindFirst(ClaimTypes.Name)?.Value,
                Email = User.FindFirst(ClaimTypes.Email)?.Value,
                Role = User.FindFirst(ClaimTypes.Role)?.Value,
                EmploymentId = User.FindFirst("EmploymentId")?.Value,
                Department = User.FindFirst("Department")?.Value
            };

            return Ok(ApiResponseDto<object>.SuccessResponse(userInfo, "User information retrieved"));
        }

        /// <summary>
        /// Logout (client-side token invalidation)
        /// </summary>
        /// <returns>Success message</returns>
        [HttpPost("logout")]
        [Authorize]
        public ActionResult<ApiResponseDto<bool>> Logout()
        {
            // In a stateless JWT implementation, logout is handled client-side
            // by removing the token from storage
            return Ok(ApiResponseDto<bool>.SuccessResponse(true, 
                "Logout successful. Please remove the token from client storage."));
        }
    }

    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Current password is required")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]", 
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password confirmation is required")]
        [Compare("NewPassword", ErrorMessage = "New password and confirmation password do not match")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}