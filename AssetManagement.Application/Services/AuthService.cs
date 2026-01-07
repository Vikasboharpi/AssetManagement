using AssetManagement.Application.DTOs;
using AssetManagement.Application.Interfaces;
using AssetManagement.Domain.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AssetManagement.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            IMapper mapper,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<ApiResponseDto<LoginResponseDto>> LoginAsync(LoginDto loginDto)
        {
            try
            {
                // Find user by email
                var user = await _userRepository.GetByEmailAsync(loginDto.Email);
                if (user == null)
                {
                    _logger.LogWarning("Login attempt with non-existent email: {Email}", loginDto.Email);
                    return ApiResponseDto<LoginResponseDto>.ErrorResponse("Invalid email or password");
                }

                // Check if user is active
                if (!user.IsActive)
                {
                    _logger.LogWarning("Login attempt with inactive user: {Email}", loginDto.Email);
                    return ApiResponseDto<LoginResponseDto>.ErrorResponse("Account is deactivated");
                }

                // Verify password
                var hashedPassword = HashPassword(loginDto.Password);
                if (user.PasswordHash != hashedPassword)
                {
                    _logger.LogWarning("Invalid password attempt for user: {Email}", loginDto.Email);
                    return ApiResponseDto<LoginResponseDto>.ErrorResponse("Invalid email or password");
                }

                // Generate JWT token
                var userDto = _mapper.Map<UserResponseDto>(user);
                var token = GenerateJwtToken(userDto);
                var expiresAt = DateTime.UtcNow.AddHours(
                    int.Parse(_configuration["Jwt:ExpiryHours"] ?? "8"));

                var loginResponse = new LoginResponseDto
                {
                    Token = token,
                    ExpiresAt = expiresAt,
                    User = userDto
                };

                _logger.LogInformation("User logged in successfully: {Email}", loginDto.Email);
                return ApiResponseDto<LoginResponseDto>.SuccessResponse(loginResponse, "Login successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login for email: {Email}", loginDto.Email);
                return ApiResponseDto<LoginResponseDto>.ErrorResponse("An error occurred during login");
            }
        }

        public async Task<ApiResponseDto<bool>> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return ApiResponseDto<bool>.ErrorResponse("User not found");
                }

                // Verify current password
                var hashedCurrentPassword = HashPassword(currentPassword);
                if (user.PasswordHash != hashedCurrentPassword)
                {
                    return ApiResponseDto<bool>.ErrorResponse("Current password is incorrect");
                }

                // Update password
                user.PasswordHash = HashPassword(newPassword);
                await _userRepository.UpdateAsync(user);

                _logger.LogInformation("Password changed successfully for user ID: {UserId}", userId);
                return ApiResponseDto<bool>.SuccessResponse(true, "Password changed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while changing password for user ID: {UserId}", userId);
                return ApiResponseDto<bool>.ErrorResponse("An error occurred while changing password");
            }
        }

        public string GenerateJwtToken(UserResponseDto user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.Name),
                new Claim("EmploymentId", user.EmploymentId),
                new Claim("Department", user.Department),
                new Claim("RoleId", user.RoleId.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(int.Parse(jwtSettings["ExpiryHours"]!)),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}