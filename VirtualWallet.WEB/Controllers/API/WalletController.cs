using Microsoft.AspNetCore.Mvc;
using VirtualWallet.DATA.Models;
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
        public IActionResult GetWalletById(int id)
        {
            var wallet = _walletService.GetWalletById(id);

            if (wallet == null)
            {
                return NotFound($"Wallet with ID {id} not found.");
            }

            return Ok(wallet);
        }

        [HttpGet("{userId}")]
        public IActionResult GetWalletsByUserId(int userId)
        {
            var wallets = _walletService.GetWalletsByUserId(userId);

            if (wallets == null)
            {
                return NotFound($"User with id {userId} has no wallets.");
            }

            return Ok(wallets);
        }

        [HttpGet("{walletName}")]
        public IActionResult GetWalletByName(string walletName)
        {
            var wallet = _walletService.GetWalletByName(walletName);

            if (wallet == null)
            {
                return NotFound($"Wallet with name {walletName} not found.");
            }

            return Ok(wallet);
        }

        [HttpPost("")]
        public ActionResult AddWallet([FromBody] Wallet wallet)
        {
            var user = (User)HttpContext.Items["User"];

            wallet.UserId = user.Id;

            _walletService.AddWallet(wallet);

            return CreatedAtAction(nameof(AddWallet), wallet);
        }

        [HttpPut("{walletId}")]
        public ActionResult UpdateWallet(int walletId, [FromBody] Wallet newWallet)
        {
            _walletService.UpdateWallet(walletId, newWallet);

            return Ok();
        }

        [HttpDelete("{walletId}")]
        public ActionResult RemoveWallet(int walletId)
        {
            _walletService.RemoveWallet(walletId);
            return Ok("Wallet removed successfully!");
        }
    }
}
