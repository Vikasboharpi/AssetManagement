namespace AssetManagement.Application.DTOs
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string EmploymentId { get; set; } = string.Empty;
        public string? MobileNumber { get; set; }
        public string Department { get; set; } = string.Empty;
        public string SubDepartment { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Role information
        public int RoleId { get; set; }
        public RoleDto Role { get; set; } = null!;
    }
}