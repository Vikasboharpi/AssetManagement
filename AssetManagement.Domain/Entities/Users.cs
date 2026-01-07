using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagement.Domain.Entities
{
    public class Users
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(20)]
        public string EmploymentId { get; set; } = string.Empty;
        
        [MaxLength(15)]
        public string? MobileNumber { get; set; }
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string Department { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string SubDepartment { get; set; } = string.Empty;
        
        // Role relationship
        [Required]
        public int RoleId { get; set; }
        
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; } = null!;
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
