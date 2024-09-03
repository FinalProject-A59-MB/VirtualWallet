using Microsoft.AspNetCore.Mvc;
using VirtualWallet.BUSINESS.Results;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.DATA.Services.Contracts;
using VirtualWallet.WEB.Models.DTOs.WalletDTOs;
using VirtualWallet.WEB.Models.ViewModels.CardViewModels;

namespace VirtualWallet.WEB.Controllers.MVC
{
    [RequireAuthorization(minRequiredRoleLevel: 1)]
    public class WalletController : BaseController
    {
        private readonly IWalletService _walletService;
        private readonly IDtoMapper _dtoMapper;
        private readonly IViewModelMapper _viewModelMapper;

        public WalletController(IWalletService walletService, IDtoMapper dtoMapper, IViewModelMapper viewModelMapper)
        {
            _walletService = walletService;
            _dtoMapper = dtoMapper;
            _viewModelMapper = viewModelMapper;
        }


        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            var result = await _walletService.GetWalletByIdAsync(id);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction("Wallets", "User");
            }

            ViewBag.IsUserWalletAdmin = CurrentUser.Id == result.Value.UserId;

            var vm = _viewModelMapper.ToWalletViewModel(result.Value);
            return View(vm);
        }

        [HttpGet]
        public IActionResult Add()
        {
            var model = new WalletRequestDto();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(WalletRequestDto wallet)
        {
            if (wallet.WalletType == DATA.Models.Enums.WalletType.Main)
            {
                TempData["ErrorMessage"] = "You already have a Main wallet.";
                return RedirectToAction("Wallets", "User");
            }

            wallet.UserId = CurrentUser.Id;

            Result<int> result = await _walletService.AddWalletAsync(_dtoMapper.ToWalletRequestDto(wallet));

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction("Wallets", "User");
            }

            return RedirectToAction("Index", new { id = result.Value });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateWallet(int id, string name)
        {
            Result<Wallet> wallet = await _walletService.GetWalletByIdAsync(id);
            var walletToUpdate = wallet.Value;
            walletToUpdate.Name = name;
            var result = await _walletService.UpdateWalletAsync(walletToUpdate);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction("Index", "Wallet", new { id = id });
            }

            return RedirectToAction("Index","Wallet", new { id = id });
        }


        [HttpGet]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var result = await _walletService.GetWalletByIdAsync(id);


            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction("Index", new { id = id });
            }

            if (result.Value.Balance>0)
            {
                TempData["ErrorMessage"] = "Your wallet still havs funds, please withdra all the funds before you can proceed.";
                var model = new CardTransactionViewModel();
                model.ActionTitle = "Withdraw Money";
                model.FormAction = "WithdrawFromWallet";
                model.Type = TransactionType.Withdrawal;
                return RedirectToAction("Withdraw", "Card", model);
            }

            var vm = _viewModelMapper.ToWalletViewModel(result.Value);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Attempt to delete the wallet
            var result = await _walletService.RemoveWalletAsync(id);

            // Check if the deletion was successful
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction("ConfirmDelete", new { id = id });
            }

            // Redirect to the user's wallet list after successful deletion
            TempData["SuccessMessage"] = "Wallet deleted successfully.";
            return RedirectToAction("Wallets", "User");
        }



        [HttpGet]
        public async Task<int> GetWalletIdByUserDetails(string details)
        {
            var result = await _walletService.GetWalletIdByUserDetailsAsync(details);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
            }

            return result.Value;
        }

        [HttpGet]
        public async Task<IActionResult> AddUser(int id)
        {
            Result<Wallet> wallet = await _walletService.GetWalletByIdAsync(id);

            ViewBag.WalletId = id;
            return View(wallet.Value);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(int walletId, string username)
        {
            var result = await _walletService.AddUserToJointWalletAsync(walletId, username);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
            }

            ViewBag.WalletId = walletId;

            return RedirectToAction("Index", new { id = walletId });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveUser(int walletId, string username)
        {
            var result = await _walletService.RemoveUserFromJointWalletAsync(walletId, username);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
            }

            return RedirectToAction("Index", new { id = walletId });
        }
    }
}
