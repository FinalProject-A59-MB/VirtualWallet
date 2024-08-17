using Microsoft.AspNetCore.Mvc;
using VirtualWallet.DATA.Models;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Services.Contracts;

namespace VirtualWallet.WEB.Controllers.API
{
    [ApiController]
    [Route("api/wallet")]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWalletById(int id)
        {
            var wallet = await _walletService.GetWalletByIdAsync(id);

            if (wallet == null)
            {
                return NotFound($"Wallet with ID {id} not found.");
            }

            return Ok(wallet);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetWalletsByUserId(int userId)
        {
            var wallets = await _walletService.GetWalletsByUserIdAsync(userId);

            if (wallets == null)
            {
                return NotFound($"User with id {userId} has no wallets.");
            }

            return Ok(wallets);
        }

        [HttpGet("{walletName}")]
        public async Task<IActionResult> GetWalletByName(string walletName)
        {
            var wallet = await _walletService.GetWalletByNameAsync(walletName);

            if (wallet == null)
            {
                return NotFound($"Wallet with name {walletName} not found.");
            }

            return Ok(wallet);
        }

        [HttpPost("")]
        public async Task<IActionResult> AddWallet([FromBody] Wallet wallet)
        {
            var user = (User)HttpContext.Items["User"];

            wallet.UserId = user.Id;

            await _walletService.AddWalletAsync(wallet);

            return CreatedAtAction(nameof(AddWallet), wallet);
        }

        [HttpPut("{walletId}")]
        public async Task<IActionResult> UpdateWallet([FromBody] Wallet newWallet)
        {
            await _walletService.UpdateWalletAsync(newWallet);

            return Ok();
        }

        [HttpDelete("{walletId}")]
        public async Task<IActionResult> RemoveWallet(int walletId)
        {
            await _walletService.RemoveWalletAsync(walletId);
            return Ok("Wallet removed successfully!");
        }
    }
}
