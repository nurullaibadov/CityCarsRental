using Cityrental.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cityrental.Domain.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string DriverLicenseNumber { get; set; } = string.Empty;
        public DateTime DriverLicenseExpiryDate { get; set; }
        public string? PassportNumber { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public UserRole Role { get; set; } = UserRole.User;
        public bool IsEmailConfirmed { get; set; }
        public bool IsActive { get; set; } = true;
        public string? ProfileImageUrl { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        // Navigation Properties
        public virtual ICollection<Rental> Rentals { get; set; } = new List<Rental>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

        // Computed Property
        public string FullName => $"{FirstName} {LastName}";

        public int Age
        {
            get
            {
                if (!DateOfBirth.HasValue) return 0;
                var today = DateTime.Today;
                var age = today.Year - DateOfBirth.Value.Year;
                if (DateOfBirth.Value.Date > today.AddYears(-age)) age--;
                return age;
            }
        }

        public bool IsDriverLicenseValid()
        {
            return DriverLicenseExpiryDate > DateTime.UtcNow;
        }
    }
}
