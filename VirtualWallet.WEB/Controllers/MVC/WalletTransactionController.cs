using CloudinaryDotNet;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Immutable;
using Twilio.Jwt.AccessToken;
using VirtualWallet.BUSINESS.Results;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Services.Contracts;
using VirtualWallet.WEB.Models.ViewModels.WalletTransactionViewModels;
using VirtualWallet.WEB.Models.ViewModels.WalletViewModels;

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
        public async Task<int> Deposit(int senderWalletId, int recipientWalletId, decimal amount)
        {
            var result = await _walletTransactionService.DepositStep1Async(senderWalletId, recipientWalletId, amount);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
            }

            return result.Value;
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

        //[HttpGet]
        //public async Task<IActionResult> Index([FromQuery] WalletTransactionQueryParameters filterParameters)
        //{
        //    Result<IEnumerable<Wallet>> walletsResult = await _walletService.GetUserWalletsAsync(CurrentUser.Id);
        //    if (!walletsResult.IsSuccess)
        //    {
        //        TempData["InfoMessage"] = "Currently, you do not have any wallets. You will first need to create a wallet.";
        //        return RedirectToAction("CreateWallet", "Wallet");
        //    }

        //    await SetupViewBagForWalletTransactionsAsync(filterParameters);

        //    List<WalletTransactionViewModel> transactionViewModels = await GetTransactionViewModelsAsync(filterParameters);
        //    if (transactionViewModels == null)
        //    {
        //        TempData["InfoMessage"] = "No transactions found with current parameters";
        //        transactionViewModels = new List<WalletTransactionViewModel>();
        //    }

        //    return View("~/Views/Wallet/WalletTransactions.cshtml", transactionViewModels);
        //}

        //private async Task SetupViewBagForWalletTransactionsAsync(WalletTransactionQueryParameters filterParameters)
        //{
        //    var totalCountResult = await _walletTransactionService.GetTotalCountAsync(filterParameters, CurrentUser.Id);
        //    ViewBag.TotalCount = totalCountResult.IsSuccess ? totalCountResult.Value : 0;
        //    ViewBag.PageSize = filterParameters.PageSize;
        //    ViewBag.PageNumber = filterParameters.PageNumber;
        //    ViewBag.FilterParameters = filterParameters;

        //    var walletsResult = await _walletService.GetUserWalletsAsync(CurrentUser.Id);
        //    if (walletsResult.IsSuccess)
        //    {
        //        ViewBag.Wallets = walletsResult.Value.Select(_viewModelMapper.ToWalletViewModel).ToList();
        //    }
        //}

        //private async Task<List<WalletTransactionViewModel>> GetTransactionViewModelsAsync(WalletTransactionQueryParameters filterParameters)
        //{
        //    var transactionsResult = await _walletTransactionService.FilterByAsync(filterParameters, CurrentUser.Id);
        //    if (transactionsResult.IsSuccess)
        //    {
        //        return transactionsResult.Value.Select(_viewModelMapper.ToWalletTransactionViewModel).ToList();
        //    }
        //    return null;
        //}
    }

}
