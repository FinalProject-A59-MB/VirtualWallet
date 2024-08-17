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
        public async Task<IActionResult> Index(int id)
        {
            Wallet wallet = await _walletService.GetWalletByIdAsync(id);
            return View(wallet);
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

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var wallet = await _walletService.GetWalletByIdAsync(id);

            return View(wallet);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, Wallet wallet)
        {
            await _walletService.UpdateWalletAsync(id, wallet);

            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            await _walletService.RemoveWalletAsync(id);

            return Ok();
        }
    }
}
