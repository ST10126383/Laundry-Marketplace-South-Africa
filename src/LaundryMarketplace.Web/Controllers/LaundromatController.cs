using LaundryMarketplace.Core.Enums;
using LaundryMarketplace.Core.Interfaces;
using LaundryMarketplace.Infrastructure.Data;
using LaundryMarketplace.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LaundryMarketplace.Web.Controllers
{
    [Authorize(Roles = "Laundromat,SystemAdmin")]
    public class LaundromatController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUserIdentity> _userManager;

        public LaundromatController(
            IOrderService orderService,
            ApplicationDbContext context,
            UserManager<ApplicationUserIdentity> userManager)
        {
            _orderService = orderService;
            _context = context;
            _userManager = userManager;
        }

        // Dashboard – list orders for this laundromat (or all if SystemAdmin)
        public async Task<IActionResult> Index()
{
    var user = await _userManager.GetUserAsync(User);
    if (user == null) return Challenge();

    IEnumerable<Core.Entities.Order> orders;

    if (User.IsInRole("SystemAdmin"))
    {
        orders = await _context.Orders
            .Include(o => o.OrderItems)
            .Include(o => o.Donation)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }
    else
    {
        var laundromat = await _context.Laundromats
            .FirstOrDefaultAsync(l => l.UserId == user.Id);

        if (laundromat == null)
        {
            TempData["Error"] = "No laundromat profile linked to your account.";
            return View(Enumerable.Empty<Core.Entities.Order>());
        }

        // Show:
        // 1. Orders already assigned to this laundromat
        // 2. Pending orders that have not been accepted by anyone yet
        orders = await _context.Orders
            .Include(o => o.OrderItems)
            .Include(o => o.Donation)
            .Where(o => o.LaundromatId == laundromat.LaundromatId 
                     || (o.LaundromatId == null && o.OrderStatus == Core.Enums.OrderStatus.Pending))
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    return View(orders);
}

        // Update order status
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int orderId, OrderStatus newStatus)
        {
            try
            {
                await _orderService.UpdateOrderStatusAsync(orderId, newStatus);
                TempData["Success"] = $"Order #{orderId} status updated to {newStatus}.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        // Accept a pending order (assign to this laundromat)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptOrder(int orderId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var laundromat = await _context.Laundromats
                .FirstOrDefaultAsync(l => l.UserId == user.Id);

            if (laundromat == null)
            {
                TempData["Error"] = "No laundromat profile found.";
                return RedirectToAction(nameof(Index));
            }

            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction(nameof(Index));
            }

            if (order.OrderStatus != OrderStatus.Pending)
            {
                TempData["Error"] = "Only pending orders can be accepted.";
                return RedirectToAction(nameof(Index));
            }

            order.LaundromatId = laundromat.LaundromatId;
            order.OrderStatus = OrderStatus.PickedUp;
            order.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Order #{orderId} accepted and marked as Picked Up.";
            return RedirectToAction(nameof(Index));
        }
    }
}
