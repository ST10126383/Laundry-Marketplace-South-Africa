using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaundryMarketplace.Core.Entities
{
    public class UniformProcurement
    {
        [Key]
        public int ProcurementId { get; set; }

        [Required]
        public string NpoAdminId { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string SupplierName { get; set; } = string.Empty;

        public int UniformQuantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitCost { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalCost { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Ordered, Received, Distributed

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? CompletedAt { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
