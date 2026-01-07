using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagement.Domain.Entities
{
    public class Asset
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string SerialNumber { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Remarks { get; set; }
        
        // Foreign Keys
        [Required]
        public int CategoryId { get; set; }
        
        [Required]
        public int BrandId { get; set; }
        
        [Required]
        public int LocationId { get; set; }
        
        [Required]
        public int CreatedByUserId { get; set; }
        
        public int? AssignedToUserId { get; set; }
        
        // Navigation Properties
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; } = null!;
        
        [ForeignKey("BrandId")]
        public virtual Brand Brand { get; set; } = null!;
        
        [ForeignKey("LocationId")]
        public virtual Location Location { get; set; } = null!;
        
        [ForeignKey("CreatedByUserId")]
        public virtual Users CreatedByUser { get; set; } = null!;
        
        [ForeignKey("AssignedToUserId")]
        public virtual Users? AssignedToUser { get; set; }
        
        // Asset Status
        public AssetStatus Status { get; set; } = AssetStatus.Available;
        
        // Audit fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsActive { get; set; } = true;
    }
    
    public enum AssetStatus
    {
        Available = 1,
        Assigned = 2,
        UnderMaintenance = 3,
        Retired = 4,
        Lost = 5,
        Damaged = 6
    }
}