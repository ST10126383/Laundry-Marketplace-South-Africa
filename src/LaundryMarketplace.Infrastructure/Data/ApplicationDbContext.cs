using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LaundryMarketplace.Core.Entities;
using LaundryMarketplace.Infrastructure.Identity;

namespace LaundryMarketplace.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUserIdentity>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Laundromat> Laundromats => Set<Laundromat>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<Donation> Donations => Set<Donation>();
        public DbSet<NpoFundLedger> NpoFundLedgers => Set<NpoFundLedger>();
        public DbSet<UniformProcurement> UniformProcurements => Set<UniformProcurement>();

        protected override void OnModelCreating(ModelBuilder builder)
{
    base.OnModelCreating(builder);

    // Laundromat
    builder.Entity<Laundromat>(e =>
    {
        e.HasKey(x => x.LaundromatId);
        e.Property(x => x.CommissionRate).HasPrecision(5, 2);
    });

    // Order
    builder.Entity<Order>(e =>
    {
        e.HasKey(x => x.OrderId);
        e.HasOne(x => x.Laundromat)
            .WithMany(l => l.Orders)
            .HasForeignKey(x => x.LaundromatId)
            .OnDelete(DeleteBehavior.SetNull);
        e.Property(x => x.TotalAmount).HasPrecision(18, 2);
        e.Property(x => x.CommissionAmount).HasPrecision(18, 2);
        e.Property(x => x.NpoAllocationAmount).HasPrecision(18, 2);
    });

    // OrderItem
    builder.Entity<OrderItem>(e =>
    {
        e.HasKey(x => x.ItemId);
        e.HasOne(x => x.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        e.Property(x => x.Price).HasPrecision(18, 2);
        e.Property(x => x.WeightKg).HasPrecision(10, 2);   // fixes the warning
    });

    // Donation (0..1 with Order)
    builder.Entity<Donation>(e =>
    {
        e.HasKey(x => x.DonationId);
        e.HasOne(x => x.Order)
            .WithOne(o => o.Donation)
            .HasForeignKey<Donation>(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    });

    // NpoFundLedger  ← THIS IS THE IMPORTANT FIX
    builder.Entity<NpoFundLedger>(e =>
    {
        e.HasKey(x => x.LedgerId);

        e.HasOne(x => x.Donation)
            .WithMany(d => d.LedgerEntries)
            .HasForeignKey(x => x.DonationId)
            .OnDelete(DeleteBehavior.SetNull);

        // Changed from SetNull to NoAction to avoid multiple cascade paths
        e.HasOne(x => x.Order)
            .WithMany()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.NoAction);

        e.Property(x => x.AllocationAmount).HasPrecision(18, 2);
        e.Property(x => x.BalanceAfter).HasPrecision(18, 2);
    });

    // UniformProcurement
    builder.Entity<UniformProcurement>(e =>
    {
        e.HasKey(x => x.ProcurementId);
        e.Property(x => x.UnitCost).HasPrecision(18, 2);
        e.Property(x => x.TotalCost).HasPrecision(18, 2);
    });
}
    }
}
