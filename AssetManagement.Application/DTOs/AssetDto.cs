using System.ComponentModel.DataAnnotations;
using AssetManagement.Domain.Entities;

namespace AssetManagement.Application.DTOs
{
    public class AssetRegistrationDto
    {
        [Required(ErrorMessage = "Asset name is required")]
        [StringLength(100, ErrorMessage = "Asset name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Serial number is required")]
        [StringLength(100, ErrorMessage = "Serial number cannot exceed 100 characters")]
        public string SerialNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid category")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Brand is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid brand")]
        public int BrandId { get; set; }

        [Required(ErrorMessage = "Location is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid location")]
        public int LocationId { get; set; }

        [StringLength(500, ErrorMessage = "Remarks cannot exceed 500 characters")]
        public string? Remarks { get; set; }
    }

    public class AssetResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string? Remarks { get; set; }
        public AssetStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }

        // Related entities
        public CategoryDto Category { get; set; } = null!;
        public BrandDto Brand { get; set; } = null!;
        public LocationDto Location { get; set; } = null!;
        public UserResponseDto CreatedByUser { get; set; } = null!;
        public UserResponseDto? AssignedToUser { get; set; }
    }

    public class AssetUpdateDto
    {
        [Required(ErrorMessage = "Asset name is required")]
        [StringLength(100, ErrorMessage = "Asset name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid category")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Brand is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid brand")]
        public int BrandId { get; set; }

        [Required(ErrorMessage = "Location is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid location")]
        public int LocationId { get; set; }

        [StringLength(500, ErrorMessage = "Remarks cannot exceed 500 characters")]
        public string? Remarks { get; set; }

        public AssetStatus Status { get; set; }
    }

    public class AssetAssignmentDto
    {
        [Required(ErrorMessage = "User ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid user")]
        public int UserId { get; set; }

        [StringLength(500, ErrorMessage = "Assignment remarks cannot exceed 500 characters")]
        public string? AssignmentRemarks { get; set; }
    }
}