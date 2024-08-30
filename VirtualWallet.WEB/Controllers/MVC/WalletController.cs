﻿using Microsoft.AspNetCore.Mvc;
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
            var model = new WalletRequestDto();
            return View(model);
        }

        [HttpPost]
        [RequireAuthorization]
        public async Task<IActionResult> Add(WalletRequestDto wallet)
        {
            wallet.UserId = CurrentUser.Id;

            Result<int> newWalletId = await _walletService.AddWalletAsync(_dtoMapper.ToWalletRequestDto(wallet));

            return RedirectToAction("Index", new { id = newWalletId.Value });
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

            await _walletService.UpdateWalletAsync(_dtoMapper.ToWalletRequestDto(wallet));

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
