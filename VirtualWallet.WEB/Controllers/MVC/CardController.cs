using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VirtualWallet.BUSINESS.Results;
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
            var viewModel = _viewModelMapper.ToUserViewModel(CurrentUser);

            return View("UserCardsPartial", viewModel);
        }

        public async Task<IActionResult> ViewCard(int cardId)
        {
            var card = await _cardService.GetCardByIdAsync(cardId);
            var cardModel = _viewModelMapper.ToCardViewModel(card.Value);
            return View("_CardPartial", cardModel);
        }


        [HttpGet]
        public IActionResult Deposit()
        {
            if (!CurrentUser.Cards.Any())
            {
                TempData["InfoMessage"] = "Currently you do not have any cards. You will first need to add a card.";
                return RedirectToAction("AddCard", "Card");
            }
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
            if (!CurrentUser.Cards.Any())
            {
                TempData["InfoMessage"] = "Currently you do not have any cards. You will first need to add a card.";
                return RedirectToAction("AddCard", "Card");
            }
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
        public async Task<IActionResult> CardTransactions([FromQuery] CardTransactionQueryParameters filterParameters)
        {
            var cards = await _cardService.GetUserCardsAsync(CurrentUser.Id);
            ViewBag.Cards = cards.Value.Select(_viewModelMapper.ToCardViewModel).ToList();
            var totalCount = await _cardService.GetTotalCountAsync(filterParameters);

            ViewBag.TotalCount = totalCount.Value;
            ViewBag.PageSize = filterParameters.PageSize;
            ViewBag.PageNumber = filterParameters.PageNumber;
            ViewBag.FilterParameters = filterParameters;

            var transactions = await _cardService.FilterByAsync(filterParameters);
            if (!transactions.IsSuccess)
            {
                var transactionViewModels2 = new List<CardTransactionViewModel>();
                return View(transactionViewModels2);
            }

            var transactionViewModels = transactions.Value.Select(_viewModelMapper.ToCardTransactionViewModel).ToList();

            

            return View(transactionViewModels);
        }



    }
}
