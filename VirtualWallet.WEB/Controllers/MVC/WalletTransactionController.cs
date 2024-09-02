using CloudinaryDotNet;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Immutable;
using Twilio.Jwt.AccessToken;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Services.Contracts;
using VirtualWallet.WEB.Models.ViewModels.WalletTransactionViewModels;

namespace VirtualWallet.WEB.Controllers.MVC
{
    [RequireAuthorization(minRequiredRoleLevel: 2)]
    public class WalletTransactionController : BaseController
    {
        private readonly IWalletTransactionService _walletTransactionService;
        private readonly IWalletService _walletService;
        private readonly IViewModelMapper _viewModelMapper;
        private readonly IEmailService _emailService;

        public WalletTransactionController(IWalletTransactionService walletTransactionService,
            IWalletService walletService,
            IViewModelMapper viewModelMapper,
            IEmailService emailService)
        {
            _walletTransactionService = walletTransactionService;
            _walletService = walletService;
            _viewModelMapper = viewModelMapper;
            _emailService = emailService;
        }


        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var result = await _walletTransactionService.GetTransactionByIdAsync(id);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction("Wallets", "User");
            }

            return View(_viewModelMapper.ToWalletTransactionViewModel(result.Value));
        }

        [HttpGet]
        public async Task<IActionResult> VerifyPayment(int id)
        {
            var result = await _walletTransactionService.GetTransactionByIdAsync(id);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction("Wallets", "User");
            }

            return View(_viewModelMapper.ToWalletTransactionViewModel(result.Value));
        }

        [HttpPost]
        public async Task<IActionResult> VerifyPayment(int transactionId, string code)
        {
            var result = await _walletTransactionService.GetTransactionByIdAsync(transactionId);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction("Wallets", "User");
            }

            if(code != result.Value.VerificationCode)
            {
                TempData["ErrorMessage"] = "Incorrect code";
                return RedirectToAction("VerifyPayment", new { id = transactionId });
            }

            var resultDeposit = await _walletTransactionService.DepositStep2Async(result.Value.SenderId, result.Value.RecipientId, result.Value.Id);

            //TODO CHECK RESULT
            if (!resultDeposit.IsSuccess)
            {
                TempData["ErrorMessage"] = resultDeposit.Error;
                return RedirectToAction("Wallets", "User");
            }

            return RedirectToAction("Details", new { id = resultDeposit.Value});
        }

        [HttpGet]
        public async Task<IActionResult> DepositInternally()
        {
            var result = await _walletService.GetWalletsByUserIdAsync(CurrentUser.Id);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction("Wallets", "User");
            }

            var vm = new DepositViewModel()
            {
                From = result.Value.Select(x => _viewModelMapper.ToWalletViewModel(x)),
                To = result.Value.Select(x => _viewModelMapper.ToWalletViewModel(x))
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> DepositExternally()
        {
            var result = await _walletService.GetWalletsByUserIdAsync(CurrentUser.Id);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction("Wallets", "User");
            }

            var vm = new DepositViewModel()
            {
                From = result.Value.Select(x => _viewModelMapper.ToWalletViewModel(x)),
            };

            return View(vm);
        }

        [HttpGet]
        public IActionResult RequestDeposit()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Deposit(int senderWalletId, int recipientWalletId, decimal amount)
        {
            if (senderWalletId == recipientWalletId)
            {
                TempData["ErrorMessage"] = "Sender and recipient can't be the same wallet!";
                return StatusCode(500, "Sender and recipient can't be the same wallet!");
            }

            var result = await _walletTransactionService.DepositStep1Async(senderWalletId, recipientWalletId, amount);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
                return StatusCode(500, result.Error);
            }

            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<int> RequestDeposit(int senderWalletId, decimal amount, string description)
        {
            var myMainWalletIdResult = await _walletService.GetWalletIdByUserDetailsAsync(CurrentUser.Username);

            var result = await _walletTransactionService.DepositStep1Async(senderWalletId, myMainWalletIdResult.Value, amount);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
            }

            var senderWalletResult = await _walletService.GetWalletByIdAsync(senderWalletId);

            if (!senderWalletResult.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
            }

            var senderEmail = senderWalletResult.Value?.User?.Email;


            if (string.IsNullOrEmpty(senderEmail))
            {
                TempData["ErrorMessage"] = "Incorrect user information.";
            }

            string transactionUrl = Url.Action("VerifyPayment", "WalletTransaction", new { id = result.Value }, Request.Scheme);
            string anchorTag = $"<a href=\"{transactionUrl}\">View Transaction</a>";

            await _emailService.SendEmailAsync(senderEmail, $"{CurrentUser.Username} is requesting payment", $"{CurrentUser.Username} sent you a payment request for {amount} BGN with a description: {description}. Please confirm the payment at {anchorTag}");

            return result.Value;
        }

    }
}
