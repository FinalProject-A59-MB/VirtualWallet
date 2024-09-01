using Microsoft.AspNetCore.Mvc;
using VirtualWallet.DATA.Services.Contracts;

namespace VirtualWallet.WEB.Controllers.API
{
    /// <summary>
    /// Controller responsible for managing user wallets, including retrieving and modifying wallet-user relationships.
    /// </summary>
    [ApiController]
    [Route("api/userWallet")]
    public class UserWalletControllerApi : BaseControllerApi
    {
        private readonly IUserWalletService _userWalletService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserWalletControllerApi"/> class.
        /// </summary>
        /// <param name="userWalletService">Service for managing user-wallet relationships.</param>
        public UserWalletControllerApi(IUserWalletService userWalletService)
        {
            _userWalletService = userWalletService;
        }

        /// <summary>
        /// Retrieves all wallets associated with a specific user by their user ID.
        /// </summary>
        /// <param name="userId">The ID of the user whose wallets are to be retrieved.</param>
        /// <returns>A list of wallets associated with the user if found; otherwise, an error message.</returns>
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

        /// <summary>
        /// Retrieves a specific wallet by its wallet ID.
        /// </summary>
        /// <param name="walletId">The ID of the wallet to retrieve.</param>
        /// <returns>The wallet details if found; otherwise, an error message.</returns>
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

        /// <summary>
        /// Associates a user with a wallet.
        /// </summary>
        /// <param name="walletId">The ID of the wallet to associate with the user.</param>
        /// <param name="userId">The ID of the user to associate with the wallet.</param>
        /// <returns>A success message if the association is successful.</returns>
        [HttpPost]
        public async Task<IActionResult> AddUserWallet(int walletId, int userId)
        {
            await _userWalletService.AddUserWalletAsync(walletId, userId);
            return Ok("User added successfully!");
        }

        /// <summary>
        /// Removes the association between a user and a wallet.
        /// </summary>
        /// <param name="walletId">The ID of the wallet to disassociate from the user.</param>
        /// <param name="userId">The ID of the user to disassociate from the wallet.</param>
        /// <returns>A success message if the removal is successful.</returns>
        [HttpDelete]
        public async Task<IActionResult> RemoveUserWallet(int walletId, int userId)
        {
            await _userWalletService.RemoveUserWalletAsync(walletId, userId);
            return Ok("User removed successfully!");
        }
    }
}
