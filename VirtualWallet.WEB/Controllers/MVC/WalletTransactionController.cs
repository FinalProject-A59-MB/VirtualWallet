using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using VirtualWallet.BUSINESS.Results;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Services.Contracts;
using VirtualWallet.WEB.Models.ViewModels.WalletTransactionViewModels;
using VirtualWallet.WEB.Models.ViewModels.WalletViewModels;

namespace VirtualWallet.WEB.Controllers.MVC
{
    public class WalletTransactionController : BaseController
    {
        private readonly IWalletTransactionService _walletTransactionService;
        private readonly IWalletService _walletService;
        private readonly IViewModelMapper _viewModelMapper;

        public WalletTransactionController(IWalletTransactionService walletTransactionService,
            IWalletService walletService,
            IViewModelMapper viewModelMapper)
        {
            _walletTransactionService = walletTransactionService;
            _walletService = walletService;
            _viewModelMapper = viewModelMapper;
        }


        [HttpGet]
        [RequireAuthorization]
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        [RequireAuthorization]
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
        [RequireAuthorization]
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
        [RequireAuthorization]
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
        [RequireAuthorization]
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
        [RequireAuthorization]
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

        [HttpPost]
        [RequireAuthorization]
        public async Task<int> Deposit(int senderWalletId, int recipientWalletId, decimal amount)
        {
            var result = await _walletTransactionService.DepositStep1Async(senderWalletId, recipientWalletId, amount);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
            }

            return result.Value;
        }

    }
}
