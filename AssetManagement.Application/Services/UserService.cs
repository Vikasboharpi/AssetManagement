using AssetManagement.Application.DTOs;
using AssetManagement.Application.Interfaces;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace AssetManagement.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, IRoleRepository roleRepository, IMapper mapper, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponseDto<UserResponseDto>> RegisterUserAsync(UserRegistrationDto registrationDto)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _userRepository.ExistsAsync(registrationDto.Email, registrationDto.EmploymentId);
                if (existingUser)
                {
                    return ApiResponseDto<UserResponseDto>.ErrorResponse(
                        "User with this email or employment ID already exists");
                }

                // Validate role exists
                var role = await _roleRepository.GetByIdAsync(registrationDto.RoleId);
                if (role == null)
                {
                    return ApiResponseDto<UserResponseDto>.ErrorResponse("Invalid role selected");
                }

                if (!role.IsActive)
                {
                    return ApiResponseDto<UserResponseDto>.ErrorResponse("Selected role is not active");
                }

                // Create new user
                var user = new Users
                {
                    FullName = registrationDto.FullName,
                    Email = registrationDto.Email.ToLower(),
                    EmploymentId = registrationDto.EmploymentId,
                    MobileNumber = registrationDto.MobileNumber,
                    PasswordHash = HashPassword(registrationDto.Password),
                    Department = registrationDto.Department,
                    SubDepartment = registrationDto.SubDepartment,
                    RoleId = registrationDto.RoleId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var createdUser = await _userRepository.CreateAsync(user);
                var userResponse = _mapper.Map<UserResponseDto>(createdUser);

                _logger.LogInformation("User registered successfully with ID: {UserId}", createdUser.Id);

                return ApiResponseDto<UserResponseDto>.SuccessResponse(
                    userResponse, "User registered successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering user");
                return ApiResponseDto<UserResponseDto>.ErrorResponse(
                    "An error occurred while registering the user");
            }
        }

        public async Task<ApiResponseDto<UserResponseDto>> GetUserByIdAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return ApiResponseDto<UserResponseDto>.ErrorResponse("User not found");
                }

                var userResponse = _mapper.Map<UserResponseDto>(user);
                return ApiResponseDto<UserResponseDto>.SuccessResponse(userResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting user by ID: {UserId}", id);
                return ApiResponseDto<UserResponseDto>.ErrorResponse(
                    "An error occurred while retrieving the user");
            }
        }

        public async Task<ApiResponseDto<UserResponseDto>> GetUserByEmailAsync(string email)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null)
                {
                    return ApiResponseDto<UserResponseDto>.ErrorResponse("User not found");
                }

                var userResponse = _mapper.Map<UserResponseDto>(user);
                return ApiResponseDto<UserResponseDto>.SuccessResponse(userResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting user by email: {Email}", email);
                return ApiResponseDto<UserResponseDto>.ErrorResponse(
                    "An error occurred while retrieving the user");
            }
        }

        public async Task<ApiResponseDto<IEnumerable<UserResponseDto>>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                var userResponses = _mapper.Map<IEnumerable<UserResponseDto>>(users);
                return ApiResponseDto<IEnumerable<UserResponseDto>>.SuccessResponse(userResponses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all users");
                return ApiResponseDto<IEnumerable<UserResponseDto>>.ErrorResponse(
                    "An error occurred while retrieving users");
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteUserAsync(int id)
        {
            try
            {
                var result = await _userRepository.DeleteAsync(id);
                if (!result)
                {
                    return ApiResponseDto<bool>.ErrorResponse("User not found");
                }

                _logger.LogInformation("User deleted successfully with ID: {UserId}", id);
                return ApiResponseDto<bool>.SuccessResponse(true, "User deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting user with ID: {UserId}", id);
                return ApiResponseDto<bool>.ErrorResponse(
                    "An error occurred while deleting the user");
            }
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}