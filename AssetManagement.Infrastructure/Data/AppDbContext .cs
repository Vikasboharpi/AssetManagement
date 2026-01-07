using AssetManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace AssetManagement.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Users> Employees => Set<Users>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Asset> Assets => Set<Asset>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Brand> Brands => Set<Brand>();
        public DbSet<Location> Locations => Set<Location>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User-Role relationship
            modelBuilder.Entity<Users>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Asset relationships
            modelBuilder.Entity<Asset>()
                .HasOne(a => a.Category)
                .WithMany(c => c.Assets)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Asset>()
                .HasOne(a => a.Brand)
                .WithMany(b => b.Assets)
                .HasForeignKey(a => a.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Asset>()
                .HasOne(a => a.Location)
                .WithMany(l => l.Assets)
                .HasForeignKey(a => a.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Asset>()
                .HasOne(a => a.CreatedByUser)
                .WithMany()
                .HasForeignKey(a => a.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Asset>()
                .HasOne(a => a.AssignedToUser)
                .WithMany()
                .HasForeignKey(a => a.AssignedToUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure unique constraints
            modelBuilder.Entity<Users>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Users>()
                .HasIndex(u => u.EmploymentId)
                .IsUnique();

            modelBuilder.Entity<Role>()
                .HasIndex(r => r.Name)
                .IsUnique();

            modelBuilder.Entity<Asset>()
                .HasIndex(a => a.SerialNumber)
                .IsUnique();

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Name)
                .IsUnique();

            modelBuilder.Entity<Brand>()
                .HasIndex(b => b.Name)
                .IsUnique();

            modelBuilder.Entity<Location>()
                .HasIndex(l => l.Name)
                .IsUnique();

            // Seed default roles
            var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin", Description = "System Administrator with full access", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Role { Id = 2, Name = "Manager", Description = "Department Manager with management access", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Role { Id = 3, Name = "Employee", Description = "Regular Employee with basic access", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Role { Id = 4, Name = "IT Support", Description = "IT Support staff with technical access", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate }
            );

            // Seed default admin user
            // Default password: Admin@123
            var adminPasswordHash = HashPassword("Admin@123");
            
            modelBuilder.Entity<Users>().HasData(
                new Users
                {
                    Id = 1,
                    FullName = "System Administrator",
                    Email = "admin@assetmanagement.com",
                    EmploymentId = "ADMIN001",
                    MobileNumber = "+1234567890",
                    PasswordHash = adminPasswordHash,
                    Department = "IT Department",
                    SubDepartment = "System Administration",
                    RoleId = 1, // Admin role
                    IsActive = true,
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                }
            );

            // Seed default categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Computers", Description = "Desktop and laptop computers", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Category { Id = 2, Name = "Mobile Devices", Description = "Smartphones and tablets", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Category { Id = 3, Name = "Network Equipment", Description = "Routers, switches, and network devices", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Category { Id = 4, Name = "Printers", Description = "Printers and scanners", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Category { Id = 5, Name = "Furniture", Description = "Office furniture and equipment", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate }
            );

            // Seed default brands
            modelBuilder.Entity<Brand>().HasData(
                new Brand { Id = 1, Name = "Dell", Description = "Dell Technologies", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Brand { Id = 2, Name = "HP", Description = "Hewlett-Packard", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Brand { Id = 3, Name = "Lenovo", Description = "Lenovo Group", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Brand { Id = 4, Name = "Apple", Description = "Apple Inc.", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Brand { Id = 5, Name = "Samsung", Description = "Samsung Electronics", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Brand { Id = 6, Name = "Cisco", Description = "Cisco Systems", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate }
            );

            // Seed default locations
            modelBuilder.Entity<Location>().HasData(
                new Location { Id = 1, Name = "Head Office", Address = "123 Main Street", City = "New York", State = "NY", PostalCode = "10001", Country = "USA", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Location { Id = 2, Name = "Branch Office - Mumbai", Address = "456 Business District", City = "Mumbai", State = "Maharashtra", PostalCode = "400001", Country = "India", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Location { Id = 3, Name = "Branch Office - Delhi", Address = "789 Corporate Plaza", City = "Delhi", State = "Delhi", PostalCode = "110001", Country = "India", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Location { Id = 4, Name = "Warehouse", Address = "321 Industrial Area", City = "Pune", State = "Maharashtra", PostalCode = "411001", Country = "India", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate }
            );
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
