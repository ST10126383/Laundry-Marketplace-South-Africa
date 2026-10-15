using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LaundryMarketplace.Core.Enums;

namespace LaundryMarketplace.Core.Entities
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public string CustomerId { get; set; } = string.Empty;

        public int? LaundromatId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CommissionAmount { get; set; }  // Platform commission

        [Column(TypeName = "decimal(18,2)")]
        public decimal NpoAllocationAmount { get; set; }  // 10% of profit allocated to NPO

        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;

        [MaxLength(500)]
        public string? PickupAddress { get; set; }

        [MaxLength(500)]
        public string? DeliveryAddress { get; set; }

        [MaxLength(1000)]
        public string? SpecialInstructions { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public Laundromat? Laundromat { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public Donation? Donation { get; set; }
    }
}
