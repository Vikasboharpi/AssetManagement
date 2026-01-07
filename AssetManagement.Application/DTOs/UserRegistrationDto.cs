using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Application.DTOs
{
    public class UserRegistrationDto
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Employment ID is required")]
        [StringLength(20, ErrorMessage = "Employment ID cannot exceed 20 characters")]
        public string EmploymentId { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid mobile number format")]
        [StringLength(15, ErrorMessage = "Mobile number cannot exceed 15 characters")]
        public string? MobileNumber { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password confirmation is required")]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Department is required")]
        [RegularExpression(@"^(IT Department|Non-IT Department)$", 
            ErrorMessage = "Department must be either 'IT Department' or 'Non-IT Department'")]
        public string Department { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sub-department is required")]
        public string SubDepartment { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid role")]
        public int RoleId { get; set; }
    }
}