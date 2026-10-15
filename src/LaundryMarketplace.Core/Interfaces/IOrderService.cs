using LaundryMarketplace.Core.Entities;
using LaundryMarketplace.Core.Enums;

namespace LaundryMarketplace.Core.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(Order order, List<OrderItem> items, bool includeDonation = false, float donationWeightKg = 0);
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task<IEnumerable<Order>> GetOrdersByCustomerAsync(string customerId);
        Task<IEnumerable<Order>> GetOrdersByLaundromatAsync(int laundromatId);
        Task UpdateOrderStatusAsync(int orderId, OrderStatus newStatus, string? updatedByUserId = null);
        Task<decimal> CalculateNpoAllocation(decimal totalAmount, decimal commissionAmount);
    }
}
