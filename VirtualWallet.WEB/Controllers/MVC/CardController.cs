﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VirtualWallet.BUSINESS.Services;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.DATA.Services.Contracts;
using VirtualWallet.WEB.Models.ViewModels;

namespace VirtualWallet.WEB.Controllers.MVC
{
    [HandleServiceResult]
    public class CardController : BaseController
    {
        private readonly ICardService _cardService;
        private readonly IUserService _userService;
        private readonly IPaymentProcessorService _paymentProcessorService;
        private readonly IViewModelMapper _viewModelMapper;
        private readonly IWalletService _walletService;
        private readonly ICardTransactionService _cardTransactionService;

        public CardController(
            ICardService cardService,
            IUserService userService,
            IPaymentProcessorService paymentProcessorService,
            IViewModelMapper modelMapper,
            IWalletService walletService,
            ICardTransactionService cardTransactionService)
        {
            _cardService = cardService;
            _userService = userService;
            _paymentProcessorService = paymentProcessorService;
            _viewModelMapper = modelMapper;
            _walletService = walletService;
            _cardTransactionService = cardTransactionService;
        }

        [HttpGet]
        public IActionResult AddCard(int userId)
        {
            var model = new CardViewModel { UserId = userId };
            return View("AddCard", model);
        }

        [HttpPost]
        public async Task<IActionResult> AddCard(CardViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var cardToVerify = new Card
            {
                CardHolderName = model.CardHolderName,
                CardNumber = model.CardNumber,
                Cvv = model.Cvv,
                ExpirationDate = model.ExpirationDate,
                Issuer = model.Issuer
            };

            var tokenResult = await _paymentProcessorService.VerifyAndRetrieveTokenAsync(cardToVerify);

            if (!tokenResult.IsSuccess)
            {
                TempData["ErrorMessage"] = tokenResult.Error;
                return View("AddCard", model);
            }

            var card = new Card
            {
                CardHolderName = model.CardHolderName,
                CardNumber = model.CardNumber,
                Issuer = model.Issuer,
                ExpirationDate = model.ExpirationDate,
                Cvv = model.Cvv,
                PaymentProcessorToken = tokenResult.Value,
                CardType = model.CardType
            };

            var addCardResult = await _cardService.AddCardAsync(CurrentUser, card);

            if (!addCardResult.IsSuccess)
            {
                TempData["ErrorMessage"] = addCardResult.Error;
                return View("AddCard", model);
            }

            if (!CurrentUser.MainWalletId.HasValue)
            {
                var wallet = new Wallet
                {
                    Name = "Main Wallet",
                    WalletType = WalletType.Main,
                    UserId = CurrentUser.Id,
                    Currency = card.Currency
                };
                CurrentUser.MainWalletId = wallet.Id;
                CurrentUser.MainWallet = wallet;

                var walletResult = await _walletService.AddWalletAsync(wallet);

                CurrentUser.Wallets.Add(wallet);
            }

            return RedirectToAction("Profile", "User");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCard(int cardId)
        {
            var cardResult = await _cardService.GetCardByIdAsync(cardId);

            if (!cardResult.IsSuccess)
            {
                TempData["ErrorMessage"] = cardResult.Error;
                return RedirectToAction("Profile", "User");
            }

            var model = _viewModelMapper.ToCardViewModel(cardResult.Value);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(CardViewModel model)
        {
            var deleteResult = await _cardService.DeleteCardAsync(model.Id);

            if (!deleteResult.IsSuccess)
            {
                TempData["ErrorMessage"] = deleteResult.Error;
                return RedirectToAction("Profile", "User");
            }

            return RedirectToAction("Cards", "User");
        }

        [RequireAuthorization]
        public async Task<IActionResult> Cards()
        {
            var user = CurrentUser;
            var viewModel = _viewModelMapper.ToUserViewModel(user);

            return View("UserCardsPartial", viewModel);
        }

        [HttpGet]
        public IActionResult Deposit()
        {
            var model = new CardTransactionViewModel
            {
                ActionTitle = "Deposit Money",
                FormAction = "DepositToWallet",
            };

            return View("CardTransactionFormPartial", model);
        }

        [HttpPost]
        public async Task<IActionResult> DepositToWallet(CardTransactionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ActionTitle = "Deposit Money";
                model.FormAction = "DepositToWallet";
                return View("CardTransactionFormPartial", model);
            }

            var result = await _cardTransactionService.DepositAsync(model.CardId, model.WalletId, model.Amount);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Deposit completed successfully.";
                return RedirectToAction("Wallets", "Wallet");
            }
            else
            {
                TempData["ErrorMessage"] = result.Error;
                return View("CardTransactionFormPartial", model);
            }
        }

        [HttpGet]
        public IActionResult Withdraw()
        {
            var model = new CardTransactionViewModel
            {
                ActionTitle = "Withdraw Money",
                FormAction = "WithdrawFromWallet",
            };

            return View("CardTransactionFormPartial", model);
        }

        [HttpPost]
        public async Task<IActionResult> WithdrawFromWallet(CardTransactionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ActionTitle = "Withdraw Money";
                model.FormAction = "WithdrawToWallet";
                return View("CardTransactionFormPartial", model);
            }

            var result = await _cardTransactionService.WithdrawAsync(model.CardId, model.WalletId, model.Amount);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Withdraw completed successfully.";
                return RedirectToAction("Wallets", "Wallet");
            }
            else
            {
                TempData["ErrorMessage"] = result.Error;
                return View("CardTransactionFormPartial", model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> CardTransactions()
        {
            var userId = CurrentUser.Id;
            var transactionsResult = await _cardService.GetCardTransactionsByUserIdAsync(userId);

            if (!transactionsResult.IsSuccess)
            {
                TempData["ErrorMessage"] = transactionsResult.Error;
                return RedirectToAction("Profile", "User");
            }

            var transactionViewModels = transactionsResult.Value.Select(transaction => new CardTransactionViewModel
            {
                CreatedAt = transaction.CreatedAt,
                Amount = transaction.Amount,
                WalletId = transaction.WalletId,    
                CardId = transaction.CardId,
            }).ToList();

            return View("CardTransactions", transactionViewModels);
        }

        [HttpGet]
        public async Task<IActionResult> CardTransactions(string searchQuery = null)
        {
            var userId = CurrentUser.Id;
            var cardTransactionsResult = await _cardService.GetCardTransactionsByUserIdAsync(userId);

            if (!cardTransactionsResult.IsSuccess)
            {
                TempData["ErrorMessage"] = cardTransactionsResult.Error;
                return RedirectToAction("Profile", "User");
            }

            var transactionViewModels = cardTransactionsResult.Value.Select(transaction => new CardTransactionViewModel
            {
                CreatedAt = transaction.CreatedAt,
                Amount = transaction.Amount
            }).ToList();

            return View(transactionViewModels);
        }




    }
}
