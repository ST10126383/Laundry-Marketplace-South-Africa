using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaundryMarketplace.Core.Entities
{
    public class Laundromat
    {
        [Key]
        public int LaundromatId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string BusinessName { get; set; } = string.Empty;

        [Required, MaxLength(300)]
        public string Address { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? MetroZone { get; set; }  // e.g. Ekurhuleni, Johannesburg, Tshwane

        [Column(TypeName = "decimal(5,2)")]
        public decimal CommissionRate { get; set; } = 10.00m; // default platform commission %

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
