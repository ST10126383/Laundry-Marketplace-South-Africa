using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaundryMarketplace.Core.Entities
{
    public class OrderItem
    {
        [Key]
        public int ItemId { get; set; }

        public int OrderId { get; set; }

        [Required, MaxLength(100)]
        public string ServiceType { get; set; } = string.Empty; // e.g. Wash & Fold, Dry Clean, Ironing

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public decimal WeightKg { get; set; }

        [MaxLength(200)]
        public string? Notes { get; set; }

        // Navigation
        public Order? Order { get; set; }
    }
}
