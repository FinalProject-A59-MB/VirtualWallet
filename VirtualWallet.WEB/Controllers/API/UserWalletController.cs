using Microsoft.AspNetCore.Mvc;
using VirtualWallet.DATA.Services.Contracts;

namespace VirtualWallet.WEB.Controllers.API
{
    [ApiController]
    [Route("api/userWallet")]
    public class UserWalletController : BaseController
    {
        private readonly IUserWalletService _userWalletService;

        public UserWalletController(IUserWalletService userWalletService)
        {
            _userWalletService = userWalletService;
        }

        [HttpGet("byUserId/{userId}")]
        public async Task<IActionResult> GetUserWalletsByUserId(int userId)
        {
            var result = await _userWalletService.GetUserWalletsByUserIdAsync(userId);

            if (!result.IsSuccess)
            {
                return NotFound(result.Error);
            }

            return Ok(result);
        }

        [HttpGet("byWalletId/{walletId}")]
        public async Task<IActionResult> GetUserWalletByWalletId(int walletId)
        {
            var result = await _userWalletService.GetUserWalletByWalletIdAsync(walletId);

            if (!result.IsSuccess)
            {
                return NotFound(result.Error);
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddUserWallet(int walletId, int userId)
        {
            await _userWalletService.AddUserWalletAsync(walletId, userId);

            return Ok("User added successfully!");
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveUserWallet(int walletId, int userId)
        {
            await _userWalletService.RemoveUserWalletAsync(walletId, userId);
            return Ok("User removed successfully!");
        }
    }
}
