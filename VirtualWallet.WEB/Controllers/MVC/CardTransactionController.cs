using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.BUSINESS.Results;
using VirtualWallet.WEB.Controllers.MVC;
using VirtualWallet.WEB.Models.ViewModels.CardViewModels;

namespace VirtualWallet.WEB.Controllers
{
    [RequireAuthorization(minRequiredRoleLevel: 1)]
    public class CardTransactionController : BaseController
    {
        private readonly ICardTransactionService _cardTransactionService;

        public CardTransactionController(ICardTransactionService cardTransactionService)
        {
            _cardTransactionService = cardTransactionService;
        }

        [HttpGet]
        public IActionResult CardTransactions(int walletId)
        {
            var viewModel = new CardTransactionViewModel
            {
                WalletId = walletId,
            };

            return View("CardTransactions.cshtml", viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Deposit(CardTransactionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("CardTransactions.cshtml", model);
            }

            var result = await _cardTransactionService.DepositAsync(model.CardId, model.WalletId, model.Amount);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
                return View("CardTransactions.cshtml", model);
            }

            return RedirectToAction("TransactionSuccess", new { transactionId = result.Value.Id });
        }

        [HttpPost]
        public async Task<IActionResult> Withdraw(CardTransactionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("CardTransactions.cshtml", model);
            }

            var result = await _cardTransactionService.WithdrawAsync(model.WalletId, model.CardId, model.Amount);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
                return View("CardTransactions.cshtml", model);
            }

            return RedirectToAction("TransactionSuccess", new { transactionId = result.Value.Id });
        }

        public IActionResult TransactionSuccess(int transactionId)
        {
            return View(transactionId);
        }
    }
}
