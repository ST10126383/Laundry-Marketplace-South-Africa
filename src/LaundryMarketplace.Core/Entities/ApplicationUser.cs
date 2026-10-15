using System.ComponentModel.DataAnnotations;

namespace LaundryMarketplace.Core.Entities
{
    /// <summary>
    /// Domain representation of a user.
    /// Actual authentication is handled by ApplicationUserIdentity in Infrastructure.
    /// This class is kept for domain modelling clarity.
    /// </summary>
    public class ApplicationUser
    {
        public string Id { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;
    }
}
