using Microsoft.AspNetCore.Identity;
using LaundryMarketplace.Core.Entities;

namespace LaundryMarketplace.Infrastructure.Identity
{
    /// <summary>
    /// Identity user that maps to the domain ApplicationUser concept.
    /// </summary>
    public class ApplicationUserIdentity : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Optional navigation to domain entities (populated via UserId)
    }
}
