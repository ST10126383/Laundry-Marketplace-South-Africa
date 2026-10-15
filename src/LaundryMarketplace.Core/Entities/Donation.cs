using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaundryMarketplace.Core.Entities
{
    public class Donation
    {
        [Key]
        public int DonationId { get; set; }

        public int OrderId { get; set; }

        public float WeightKg { get; set; }

        [MaxLength(100)]
        public string? ItemCategory { get; set; }  // e.g. Clothing, Shoes, School Uniforms

        [MaxLength(50)]
        public string ConditionStatus { get; set; } = "Pending Assessment"; // Pending Assessment, Good, Fair, Poor

        public DateTime? ProcessedDate { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        // Navigation
        public Order? Order { get; set; }
        public ICollection<NpoFundLedger> LedgerEntries { get; set; } = new List<NpoFundLedger>();
    }
}
