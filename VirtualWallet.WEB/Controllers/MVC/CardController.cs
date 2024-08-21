using Microsoft.AspNetCore.Mvc;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.DATA.Services;
using VirtualWallet.DATA.Services.Contracts;
using VirtualWallet.WEB.Models.ViewModels;

namespace VirtualWallet.WEB.Controllers.MVC
{
    public class CardController : Controller
    {
        private readonly ICardService _cardService;
        private readonly IUserService _userService;
        private readonly IPaymentProcessorService _paymentProcessorService;
        private readonly IViewModelMapper _viewModelMapper;
        private readonly IWalletService _walletService;
        public CardController(
            ICardService cardService, 
            IUserService userService, 
            IPaymentProcessorService paymentProcessorService,
            IViewModelMapper modelMapper,
            IWalletService walletService)
        {
            _cardService = cardService;
            _userService = userService;
            _paymentProcessorService = paymentProcessorService;
            _viewModelMapper = modelMapper;
            _walletService = walletService;
        }

        [HttpGet]
        public IActionResult AddCard(int userId)
        {
            var model = new CardViewModel { UserId = userId };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddCard(CardViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User user = HttpContext.Items["CurrentUser"] as User;

            var cardToVerify = new Card
            {
                CardHolderName = model.CardHolderName,
                CardNumber = model.CardNumber,
                Cvv = model.Cvv,
                ExpirationDate = model.ExpirationDate,
                Issuer = model.Issuer
            };

            var paymentProcessorToken = await _paymentProcessorService.VerifyAndRetrieveTokenAsync(cardToVerify);

            var card = new Card
            {
                CardHolderName = model.CardHolderName,
                CardNumber = model.CardNumber,
                Issuer = model.Issuer,
                ExpirationDate = model.ExpirationDate,
                Cvv = model.Cvv,
                PaymentProcessorToken = paymentProcessorToken,
                CardType = model.CardType
            };

            await _cardService.AddCardAsync(user, card);

            if (!user.MainWalletId.HasValue)
            {
                var wallet = new Wallet
                {
                    Name = "Main Wallet",
                    WalletType = WalletType.Main,
                    UserId = user.Id,
                    Currency = card.Currency
                };
                user.MainWalletId = wallet.Id;
                user.MainWallet = wallet;
                await _walletService.AddWalletAsync(wallet);
                user.Wallets.Add(wallet);
            }
            return RedirectToAction("Profile", "User", new { id = model.UserId });
        }
    

    [HttpGet]
        public async Task<IActionResult> DeleteCard(int cardId)
        {
            var card = await _cardService.GetCardByIdAsync(cardId);
            var model = _viewModelMapper.ToCardViewModel(card);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(CardViewModel model)
        {
            var card = await _cardService.GetCardByIdAsync(model.Id);
            await _cardService.DeleteCardAsync(model.Id);
            return RedirectToAction("Profile", "User", new { id = model.UserId });
        }

    }
}
