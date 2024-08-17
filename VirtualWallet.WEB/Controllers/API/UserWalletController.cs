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
        public IActionResult GetUserWalletsByUserId(int userId)
        {
            var wallets = _userWalletService.GetUserWalletsByUserId(userId);

            if (wallets == null)
            {
                return NotFound($"User with id {userId} has no wallets.");
            }

            return Ok(wallets);
        }

        [HttpGet("{walletId}")]
        public IActionResult GetUserWalletByWalletId(int walletId)
        {
            var wallet = _userWalletService.GetUserWalletByWalletId(walletId);

            if (wallet == null)
            {
                return NotFound($"Wallet with id {walletId} doesn't exist.");
            }

            return Ok(wallet);
        }

        [HttpPost("")]
        public ActionResult AddUserWallet([FromBody] UserWallet userWallet)
        {
            var user = (User)HttpContext.Items["User"];

            userWallet.UserId = user.Id;

            _userWalletService.AddUserWallet(userWallet);

            return CreatedAtAction(nameof(AddUserWallet), userWallet);
        }

        [HttpDelete("{walletId}")]
        public ActionResult RemoveUserWallet(int walletId)
        {
            var wallet = _userWalletService.GetUserWalletByWalletId(walletId);

            if (wallet == null)
            {
                return NotFound($"Wallet with ID {walletId} not found.");
            }

            _userWalletService.RemoveUserWallet(walletId);
            return Ok("Wallet removed successfully!");
        }
    }
}
