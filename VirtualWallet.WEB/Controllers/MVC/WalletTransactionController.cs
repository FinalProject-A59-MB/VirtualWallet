using Microsoft.AspNetCore.Mvc;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Models;

namespace VirtualWallet.WEB.Controllers.MVC
{
    public class WalletTransactionController : BaseController
    {
        private readonly ITransactionHandlingService _transactionHandlingService;

        public WalletTransactionController(ITransactionHandlingService transactionHandlingService)
        {
            _transactionHandlingService = transactionHandlingService;
        }


        [HttpGet]
        [RequireAuthorization]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [RequireAuthorization]
        public async Task<IActionResult> Add(Wallet senderWallet, Wallet recipientWallet, decimal amount)
        {
            var transaction = await _transactionHandlingService.ProcessWalletToWalletTransactionAsync(senderWallet, recipientWallet, amount);

            return RedirectToAction("Index", "Wallet", new { id = senderWallet.Id });
        }
    }
}
