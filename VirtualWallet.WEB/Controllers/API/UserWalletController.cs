using Microsoft.AspNetCore.Mvc;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Services;
using VirtualWallet.DATA.Services.Contracts;

namespace VirtualWallet.WEB.Controllers.API
{
    [ApiController]
    [Route("api/userWallet")]
    public class UserWalletController : ControllerBase
    {
        private readonly IUserWalletService _userWalletService;

        public UserWalletController(IUserWalletService userWalletService)
        {
            _userWalletService = userWalletService;
        }


        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserWalletsByUserId(int userId)
        {
            var wallets = await _userWalletService.GetUserWalletsByUserIdAsync(userId);

            if (wallets == null)
            {
                return NotFound($"User with id {userId} has no wallets.");
            }

            return Ok(wallets);
        }

        [HttpGet("{walletId}")]
        public async Task<IActionResult> GetUserWalletByWalletId(int walletId)
        {
            var wallet = await _userWalletService.GetUserWalletByWalletIdAsync(walletId);

            if (wallet == null)
            {
                return NotFound($"Wallet with id {walletId} doesn't exist.");
            }

            return Ok(wallet);
        }

        [HttpPost("")]
        public async Task<IActionResult> AddUserWallet([FromBody] UserWallet userWallet)
        {
            var user = (User)HttpContext.Items["User"];

            userWallet.UserId = user.Id;

            await _userWalletService.AddUserWalletAsync(userWallet);

            return CreatedAtAction(nameof(AddUserWallet), userWallet);
        }

        [HttpDelete("{walletId}")]
        public async Task<IActionResult> RemoveUserWallet(int walletId)
        {
            var wallet = await _userWalletService.GetUserWalletByWalletIdAsync(walletId);

            if (wallet == null)
            {
                return NotFound($"Wallet with ID {walletId} not found.");
            }

            await _userWalletService.RemoveUserWalletAsync(walletId);
            return Ok("Wallet removed successfully!");
        }
    }
}
