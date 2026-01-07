using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Application.DTOs
{
    public class LocationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateLocationDto
    {
        [Required(ErrorMessage = "Location name is required")]
        [StringLength(100, ErrorMessage = "Location name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string? Address { get; set; }

        [StringLength(50, ErrorMessage = "City cannot exceed 50 characters")]
        public string? City { get; set; }

        [StringLength(50, ErrorMessage = "State cannot exceed 50 characters")]
        public string? State { get; set; }

        [StringLength(20, ErrorMessage = "Postal code cannot exceed 20 characters")]
        public string? PostalCode { get; set; }

        [StringLength(50, ErrorMessage = "Country cannot exceed 50 characters")]
        public string? Country { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class UpdateLocationDto
    {
        [Required(ErrorMessage = "Location name is required")]
        [StringLength(100, ErrorMessage = "Location name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string? Address { get; set; }

        [StringLength(50, ErrorMessage = "City cannot exceed 50 characters")]
        public string? City { get; set; }

        [StringLength(50, ErrorMessage = "State cannot exceed 50 characters")]
        public string? State { get; set; }

        [StringLength(20, ErrorMessage = "Postal code cannot exceed 20 characters")]
        public string? PostalCode { get; set; }

        [StringLength(50, ErrorMessage = "Country cannot exceed 50 characters")]
        public string? Country { get; set; }

        public bool IsActive { get; set; }
    }
}