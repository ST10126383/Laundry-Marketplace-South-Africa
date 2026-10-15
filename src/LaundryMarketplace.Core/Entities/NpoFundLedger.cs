using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaundryMarketplace.Core.Entities
{
    public class NpoFundLedger
    {
        [Key]
        public int LedgerId { get; set; }

        public int? DonationId { get; set; }

        public int? OrderId { get; set; }  // Link to the commercial order that generated the allocation

        [Column(TypeName = "decimal(18,2)")]
        public decimal AllocationAmount { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18,2)")]
        public decimal BalanceAfter { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }

        [MaxLength(50)]
        public string TransactionType { get; set; } = "Allocation"; // Allocation, Procurement, Adjustment

        // Navigation
        public Donation? Donation { get; set; }
        public Order? Order { get; set; }
    }
}
