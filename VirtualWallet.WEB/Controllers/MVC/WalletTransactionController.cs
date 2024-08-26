using Microsoft.AspNetCore.Mvc;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Services.Contracts;

namespace VirtualWallet.WEB.Controllers.MVC
{
    public class WalletTransactionController : BaseController
    {
        private readonly IWalletTransactionService _walletTransactionService;

        public WalletTransactionController(IWalletTransactionService walletTransactionService)
        {
            _walletTransactionService = walletTransactionService;
        }


        [HttpGet]
        [RequireAuthorization]
        public IActionResult Deposit()
        {
            return View();
        }

        [HttpPost]
        [RequireAuthorization]
        public async Task<IActionResult> Deposit(int senderWalletId, int recipientWalletId, decimal amount)
        {
            await _walletTransactionService.DepositAsync(senderWalletId, recipientWalletId, amount);

            return RedirectToAction("Index", "Wallet", new { id = senderWalletId});
        }
    }
}
