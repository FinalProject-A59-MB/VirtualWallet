using Microsoft.AspNetCore.Mvc;
using VirtualWallet.BUSINESS.Results;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Services;
using VirtualWallet.DATA.Services.Contracts;
using VirtualWallet.WEB.Models.DTOs.WalletDTOs;
using VirtualWallet.WEB.Models.ViewModels;

namespace VirtualWallet.WEB.Controllers.MVC
{
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
            var wallet = await _walletService.GetWalletByIdAsync(id);
            return View(wallet.Value);
        }

        [HttpGet]
        [RequireAuthorization]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [RequireAuthorization]
        public async Task<IActionResult> Add(WalletRequestDto wallet)
        {
            //Get loged in user id
            int userId = 0;

            wallet.UserId = userId;

            var newWalletId = await _walletService.AddWalletAsync(_dtoMapper.ToWallet(wallet));

            return RedirectToAction("Index", new { id = newWalletId});
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var wallet = await _walletService.GetWalletByIdAsync(id);

            return View(wallet);
        }

        [HttpPost]
        public async Task<IActionResult> Update(WalletRequestDto wallet)
        {
            int userId = 0;

            wallet.UserId = userId;

            await _walletService.UpdateWalletAsync(_dtoMapper.ToWallet(wallet));

            return RedirectToAction("Index", new { id = wallet.Id });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            await _walletService.RemoveWalletAsync(id);

            return Ok();
        }
    }
}
