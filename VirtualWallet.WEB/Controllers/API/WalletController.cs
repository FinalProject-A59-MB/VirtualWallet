using Microsoft.AspNetCore.Mvc;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Services.Contracts;

namespace VirtualWallet.WEB.Controllers.API
{
    /// <summary>
    /// Controller responsible for managing wallet-related operations, including retrieval, creation, update, and deletion of wallets.
    /// </summary>
    [ApiController]
    [Route("api/wallet")]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        /// <summary>
        /// Initializes a new instance of the <see cref="WalletController"/> class.
        /// </summary>
        /// <param name="walletService">Service for managing wallet operations.</param>
        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        /// <summary>
        /// Retrieves a wallet by its unique ID.
        /// </summary>
        /// <param name="id">The ID of the wallet to retrieve.</param>
        /// <returns>The wallet details if found; otherwise, an error message.</returns>
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

        /// <summary>
        /// Retrieves all wallets associated with a specific user by their user ID.
        /// </summary>
        /// <param name="userId">The ID of the user whose wallets are to be retrieved.</param>
        /// <returns>A list of wallets associated with the user if found; otherwise, an error message.</returns>
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

        /// <summary>
        /// Retrieves a wallet by its name.
        /// </summary>
        /// <param name="walletName">The name of the wallet to retrieve.</param>
        /// <returns>The wallet details if found; otherwise, an error message.</returns>
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

        /// <summary>
        /// Adds a new wallet for the current user.
        /// </summary>
        /// <param name="wallet">The wallet object to be added.</param>
        /// <returns>The created wallet's details.</returns>
        [HttpPost("")]
        public async Task<IActionResult> AddWallet([FromBody] Wallet wallet)
        {
            var user = (User)HttpContext.Items["User"];

            wallet.UserId = user.Id;

            await _walletService.AddWalletAsync(wallet);

            return CreatedAtAction(nameof(AddWallet), wallet);
        }

        /// <summary>
        /// Updates an existing wallet.
        /// </summary>
        /// <param name="newWallet">The updated wallet object.</param>
        /// <returns>An HTTP status code indicating success.</returns>
        [HttpPut("{walletId}")]
        public async Task<IActionResult> UpdateWallet([FromBody] Wallet newWallet)
        {
            await _walletService.UpdateWalletAsync(newWallet);

            return Ok();
        }

        /// <summary>
        /// Removes a wallet by its unique ID.
        /// </summary>
        /// <param name="walletId">The ID of the wallet to remove.</param>
        /// <returns>A success message if the wallet is removed successfully.</returns>
        [HttpDelete("{walletId}")]
        public async Task<IActionResult> RemoveWallet(int walletId)
        {
            await _walletService.RemoveWalletAsync(walletId);
            return Ok("Wallet removed successfully!");
        }
    }
}
