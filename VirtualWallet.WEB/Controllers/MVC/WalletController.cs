using Microsoft.AspNetCore.Mvc;
using VirtualWallet.BUSINESS.Results;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Services.Contracts;
using VirtualWallet.WEB.Models.DTOs.WalletDTOs;

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

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            Result<Wallet> wallet = await _walletService.GetWalletByIdAsync(id);

            return View(wallet.Value);
        }

        [HttpPost]
        public async Task<IActionResult> Update(WalletRequestDto wallet)
        {
            int userId = 0;

            wallet.UserId = userId;

            var result = await _walletService.UpdateWalletAsync(_dtoMapper.ToWalletRequestDto(wallet));

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
            }

            return RedirectToAction("Index", new { id = wallet.Id });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _walletService.RemoveWalletAsync(id);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction("Index", new { id = id });
            }

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
