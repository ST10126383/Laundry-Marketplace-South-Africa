using LaundryMarketplace.Core.Entities;
using LaundryMarketplace.Core.Enums;
using LaundryMarketplace.Core.Interfaces;
using LaundryMarketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LaundryMarketplace.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly INpoService _npoService;
        private const decimal NpoAllocationPercentage = 0.10m; // 10% documented rule

        public OrderService(ApplicationDbContext context, INpoService npoService)
        {
            _context = context;
            _npoService = npoService;
        }

        public async Task<Order> CreateOrderAsync(Order order, List<OrderItem> items, bool includeDonation = false, float donationWeightKg = 0)
        {
            if (items == null || !items.Any())
                throw new ArgumentException("Order must contain at least one item.");

            order.TotalAmount = items.Sum(i => i.Price);
            order.CommissionAmount = order.TotalAmount * 0.15m; // Example platform commission 15%
            order.NpoAllocationAmount = await CalculateNpoAllocation(order.TotalAmount, order.CommissionAmount);
            order.OrderStatus = OrderStatus.Pending;
            order.OrderDate = DateTime.UtcNow;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in items)
            {
                item.OrderId = order.OrderId;
                _context.OrderItems.Add(item);
            }

            if (includeDonation && donationWeightKg > 0)
            {
                var donation = new Donation
                {
                    OrderId = order.OrderId,
                    WeightKg = donationWeightKg,
                    ItemCategory = "Clothing",
                    ConditionStatus = "Pending Assessment"
                };
                _context.Donations.Add(donation);
            }

            await _context.SaveChangesAsync();

            // Record the 10% NPO allocation
            await _npoService.RecordAllocationAsync(order.OrderId, order.NpoAllocationAmount,
                $"10% allocation from Order #{order.OrderId}");

            return order;
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.Donation)
                .Include(o => o.Laundromat)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<IEnumerable<Order>> GetOrdersByCustomerAsync(string customerId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByLaundromatAsync(int laundromatId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.LaundromatId == laundromatId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task UpdateOrderStatusAsync(int orderId, OrderStatus newStatus, string? updatedByUserId = null)
        {
            var order = await _context.Orders.FindAsync(orderId)
                ?? throw new KeyNotFoundException($"Order {orderId} not found.");

            order.OrderStatus = newStatus;
            order.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public Task<decimal> CalculateNpoAllocation(decimal totalAmount, decimal commissionAmount)
        {
            // Documented business rule: 10% of the platform commission (or of total) goes to NPO
            // Using commission as the base for "profit" allocation
            var allocation = Math.Round(commissionAmount * NpoAllocationPercentage, 2);
            return Task.FromResult(allocation);
        }
    }
}
