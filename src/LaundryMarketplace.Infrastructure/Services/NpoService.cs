using LaundryMarketplace.Core.Entities;
using LaundryMarketplace.Core.Interfaces;
using LaundryMarketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LaundryMarketplace.Infrastructure.Services
{
    public class NpoService : INpoService
    {
        private readonly ApplicationDbContext _context;

        public NpoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<NpoFundLedger> RecordAllocationAsync(int orderId, decimal amount, string description)
        {
            var currentBalance = await GetCurrentBalanceAsync();
            var newBalance = currentBalance + amount;

            var entry = new NpoFundLedger
            {
                OrderId = orderId,
                AllocationAmount = amount,
                BalanceAfter = newBalance,
                TransactionDate = DateTime.UtcNow,
                Description = description,
                TransactionType = "Allocation"
            };

            _context.NpoFundLedgers.Add(entry);
            await _context.SaveChangesAsync();
            return entry;
        }

        public async Task<IEnumerable<NpoFundLedger>> GetLedgerEntriesAsync()
        {
            return await _context.NpoFundLedgers
                .OrderByDescending(l => l.TransactionDate)
                .ToListAsync();
        }

        public async Task<decimal> GetCurrentBalanceAsync()
        {
            var last = await _context.NpoFundLedgers
                .OrderByDescending(l => l.LedgerId)
                .FirstOrDefaultAsync();
            return last?.BalanceAfter ?? 0m;
        }

        public async Task<UniformProcurement> CreateProcurementAsync(UniformProcurement procurement)
        {
            if (procurement.UniformQuantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");
            if (procurement.UnitCost <= 0)
                throw new ArgumentException("Unit cost must be greater than zero.");

            procurement.TotalCost = procurement.UniformQuantity * procurement.UnitCost;
            procurement.CreatedAt = DateTime.UtcNow;
            procurement.Status = "Pending";

            var currentBalance = await GetCurrentBalanceAsync();
            if (procurement.TotalCost > currentBalance)
                throw new InvalidOperationException($"Insufficient NPO funds. Available: R{currentBalance:N2}, Required: R{procurement.TotalCost:N2}");

            _context.UniformProcurements.Add(procurement);

            // Deduct from ledger
            var ledger = new NpoFundLedger
            {
                AllocationAmount = -procurement.TotalCost,
                BalanceAfter = currentBalance - procurement.TotalCost,
                TransactionDate = DateTime.UtcNow,
                Description = $"Procurement #{procurement.ProcurementId} - {procurement.SupplierName} ({procurement.UniformQuantity} uniforms)",
                TransactionType = "Procurement"
            };
            _context.NpoFundLedgers.Add(ledger);

            await _context.SaveChangesAsync();
            return procurement;
        }

        public async Task<IEnumerable<UniformProcurement>> GetProcurementsAsync()
        {
            return await _context.UniformProcurements
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<(int TotalDonations, float TotalWeightKg, int UniformsProcured, decimal TotalFundsAllocated)> GetImpactMetricsAsync()
        {
            var totalDonations = await _context.Donations.CountAsync();
            var totalWeight = await _context.Donations.SumAsync(d => (float?)d.WeightKg) ?? 0f;
            var uniforms = await _context.UniformProcurements.SumAsync(p => (int?)p.UniformQuantity) ?? 0;
            var totalAllocated = await _context.NpoFundLedgers
                .Where(l => l.TransactionType == "Allocation")
                .SumAsync(l => (decimal?)l.AllocationAmount) ?? 0m;

            return (totalDonations, totalWeight, uniforms, totalAllocated);
        }
    }
}
