using LaundryMarketplace.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LaundryMarketplace.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly INpoService _npoService;

        public HomeController(INpoService npoService)
        {
            _npoService = npoService;
        }

        public async Task<IActionResult> Index()
        {
            var metrics = await _npoService.GetImpactMetricsAsync();
            ViewBag.TotalDonations = metrics.TotalDonations;
            ViewBag.TotalWeightKg = metrics.TotalWeightKg;
            ViewBag.UniformsProcured = metrics.UniformsProcured;
            ViewBag.TotalFundsAllocated = metrics.TotalFundsAllocated;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
