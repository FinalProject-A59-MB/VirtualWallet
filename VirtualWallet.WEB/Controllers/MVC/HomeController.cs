using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.WEB.Models;
using VirtualWallet.WEB.Models.ViewModels;

namespace VirtualWallet.WEB.Controllers.MVC
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
