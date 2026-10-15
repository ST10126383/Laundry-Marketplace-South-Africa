using LaundryMarketplace.Core.Entities;

namespace LaundryMarketplace.Core.Interfaces
{
    public interface INpoService
    {
        Task<NpoFundLedger> RecordAllocationAsync(int orderId, decimal amount, string description);
        Task<IEnumerable<NpoFundLedger>> GetLedgerEntriesAsync();
        Task<decimal> GetCurrentBalanceAsync();
        Task<UniformProcurement> CreateProcurementAsync(UniformProcurement procurement);
        Task<IEnumerable<UniformProcurement>> GetProcurementsAsync();
        Task<(int TotalDonations, float TotalWeightKg, int UniformsProcured, decimal TotalFundsAllocated)> GetImpactMetricsAsync();
    }
}
