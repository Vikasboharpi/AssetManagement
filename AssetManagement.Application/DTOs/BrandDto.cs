using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Application.DTOs
{
    public class BrandDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateBrandDto
    {
        [Required(ErrorMessage = "Brand name is required")]
        [StringLength(100, ErrorMessage = "Brand name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class UpdateBrandDto
    {
        [Required(ErrorMessage = "Brand name is required")]
        [StringLength(100, ErrorMessage = "Brand name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
        public string? Description { get; set; }

        public bool IsActive { get; set; }
    }
}