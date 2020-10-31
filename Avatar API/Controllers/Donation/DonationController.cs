using Avatar_API.Data.Services;
using Avatar_API.Filter;
using Avatar_API.Session;
using Microsoft.AspNetCore.Mvc;

namespace Avatar_API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DonationController : Controller
    {
        private readonly PackageService _cashPackageService;
        private readonly SessionHandler _sessionHandler;

        public DonationController(PackageService cashPackageService,
                                  SessionHandler sessionHandler)
        {
            _sessionHandler = sessionHandler;
            _cashPackageService = cashPackageService;

        }

        public IActionResult Index()
        {
            ViewBag.Currency = _cashPackageService.GetCurrency();
            ViewData.Model = _cashPackageService.GetPackages();
            return View();
        }

        public IActionResult Gateway(int price)
        {
            ViewBag.Price = price;
            return View();
        }

        public IActionResult Success()
        {
            return View("Success");
        }

        public IActionResult Invalid()
        {
            return View("Invalid");
        }
    }
}
