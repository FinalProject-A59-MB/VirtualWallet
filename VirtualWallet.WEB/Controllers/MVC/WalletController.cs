using Microsoft.AspNetCore.Mvc;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Services.Contracts;
using VirtualWallet.WEB.Models.DTOs;

namespace VirtualWallet.WEB.Controllers.MVC
{
    public class WalletController : Controller
    { 
        private readonly IWalletService _walletService;
        private readonly IDtoMapper _dtoMapper;

        public WalletController(IWalletService walletService, IDtoMapper dtoMapper)
        {
            _walletService = walletService;
            _dtoMapper = dtoMapper;
        }


        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            Wallet wallet = await _walletService.GetWalletByIdAsync(id);
            return View(wallet);
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
