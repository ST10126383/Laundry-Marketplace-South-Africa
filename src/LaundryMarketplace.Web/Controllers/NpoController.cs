using LaundryMarketplace.Core.Entities;
using LaundryMarketplace.Core.Interfaces;
using LaundryMarketplace.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LaundryMarketplace.Web.Controllers
{
    [Authorize(Roles = "NpoAdmin,SystemAdmin")]
    public class NpoController : Controller
    {
        private readonly INpoService _npoService;
        private readonly UserManager<ApplicationUserIdentity> _userManager;

        public NpoController(INpoService npoService, UserManager<ApplicationUserIdentity> userManager)
        {
            _npoService = npoService;
            _userManager = userManager;
        }

        // Dashboard
        public async Task<IActionResult> Index()
        {
            var balance = await _npoService.GetCurrentBalanceAsync();
            var metrics = await _npoService.GetImpactMetricsAsync();
            var recentLedger = (await _npoService.GetLedgerEntriesAsync()).Take(10);

            ViewBag.Balance = balance;
            ViewBag.TotalDonations = metrics.TotalDonations;
            ViewBag.TotalWeightKg = metrics.TotalWeightKg;
            ViewBag.UniformsProcured = metrics.UniformsProcured;
            ViewBag.TotalFundsAllocated = metrics.TotalFundsAllocated;

            return View(recentLedger);
        }

        // Full ledger
        public async Task<IActionResult> Ledger()
        {
            var entries = await _npoService.GetLedgerEntriesAsync();
            ViewBag.Balance = await _npoService.GetCurrentBalanceAsync();
            return View(entries);
        }

        // Procurement list
        public async Task<IActionResult> Procurements()
        {
            var list = await _npoService.GetProcurementsAsync();
            return View(list);
        }

        // Create procurement form
        [HttpGet]
        public IActionResult CreateProcurement()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProcurement(string supplierName, int uniformQuantity, decimal unitCost, string? notes)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            if (string.IsNullOrWhiteSpace(supplierName) || uniformQuantity <= 0 || unitCost <= 0)
            {
                ModelState.AddModelError(string.Empty, "Please provide valid supplier, quantity and unit cost.");
                return View();
            }

            try
            {
                var procurement = new UniformProcurement
                {
                    NpoAdminId = user.Id,
                    SupplierName = supplierName,
                    UniformQuantity = uniformQuantity,
                    UnitCost = unitCost,
                    Notes = notes
                };

                var created = await _npoService.CreateProcurementAsync(procurement);
                TempData["Success"] = $"Procurement created. Total cost R{created.TotalCost:N2} deducted from NPO fund.";
                return RedirectToAction(nameof(Procurements));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View();
            }
        }
    }
}
