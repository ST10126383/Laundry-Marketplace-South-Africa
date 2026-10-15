using LaundryMarketplace.Core.Entities;
using LaundryMarketplace.Core.Enums;
using LaundryMarketplace.Core.Interfaces;
using LaundryMarketplace.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LaundryMarketplace.Web.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly UserManager<ApplicationUserIdentity> _userManager;

        public OrdersController(IOrderService orderService, UserManager<ApplicationUserIdentity> userManager)
        {
            _orderService = orderService;
            _userManager = userManager;
        }

        // Customer: list my orders
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> MyOrders()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var orders = await _orderService.GetOrdersByCustomerAsync(user.Id);
            return View(orders);
        }

        // Customer: booking form
        [Authorize(Roles = "Customer")]
        [HttpGet]
        public IActionResult Book()
        {
            return View();
        }

        // Customer: submit booking
        [Authorize(Roles = "Customer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(
            string serviceType,
            decimal price,
            decimal weightKg,
            string? pickupAddress,
            string? deliveryAddress,
            string? specialInstructions,
            bool includeDonation,
            float donationWeightKg)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            // Basic validation
            if (string.IsNullOrWhiteSpace(serviceType) || price <= 0)
            {
                ModelState.AddModelError(string.Empty, "Please select a valid service and price.");
                return View();
            }
            if (includeDonation && donationWeightKg <= 0)
            {
                ModelState.AddModelError(string.Empty, "Please enter a valid donation weight.");
                return View();
            }

            try
            {
                var order = new Order
                {
                    CustomerId = user.Id,
                    PickupAddress = pickupAddress,
                    DeliveryAddress = deliveryAddress,
                    SpecialInstructions = specialInstructions
                };

                var items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ServiceType = serviceType,
                        Price = price,
                        WeightKg = weightKg
                    }
                };

                var created = await _orderService.CreateOrderAsync(order, items, includeDonation, donationWeightKg);

                TempData["Success"] = $"Order #{created.OrderId} created successfully. NPO allocation: R{created.NpoAllocationAmount:N2}";
                return RedirectToAction(nameof(Details), new { id = created.OrderId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Could not create order: {ex.Message}");
                return View();
            }
        }

        // Order details (customer or laundromat)
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            // Simple authorization: customer who owns it, or any laundromat/admin
            var isOwner = order.CustomerId == user.Id;
            var isStaff = User.IsInRole("Laundromat") || User.IsInRole("NpoAdmin") || User.IsInRole("SystemAdmin") || User.IsInRole("Driver");
            if (!isOwner && !isStaff)
                return Forbid();

            return View(order);
        }

        // Track order (public-ish for customer)
        [Authorize(Roles = "Customer,Driver,Laundromat,SystemAdmin")]
        public async Task<IActionResult> Track(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();
            return View(order);
        }
    }
}
