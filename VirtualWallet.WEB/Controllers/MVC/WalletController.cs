using Microsoft.AspNetCore.Mvc;
using VirtualWallet.DATA.Models;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Services.Contracts;

namespace VirtualWallet.WEB.Controllers.MVC
{
    public class WalletController : Controller
    { 
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }


        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Wallet wallet)
        {
            await _walletService.AddWalletAsync(wallet);

            return Ok();
        }
    }
}
