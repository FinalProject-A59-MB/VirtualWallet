using Microsoft.AspNetCore.Mvc;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Services.Contracts;

namespace VirtualWallet.WEB.Controllers.API
{
    /// <summary>
    /// Controller responsible for managing wallet transactions, including retrieval of transactions by sender, recipient, and ID.
    /// </summary>
    [ApiController]
    [Route("api/walletTransactions")]
    public class WalletTransactionController : ControllerBase
    {
        private readonly IWalletTransactionService _walletTransactionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="WalletTransactionController"/> class.
        /// </summary>
        /// <param name="walletTransactionService">Service for managing wallet transaction operations.</param>
        public WalletTransactionController(IWalletTransactionService walletTransactionService)
        {
            _walletTransactionService = walletTransactionService;
        }

        /// <summary>
        /// Retrieves all transactions initiated by a specific sender.
        /// </summary>
        /// <param name="senderId">The ID of the sender whose transactions are to be retrieved.</param>
        /// <returns>A list of transactions if found; otherwise, an error message.</returns>
        [HttpGet("{senderId}")]
        public async Task<IActionResult> GetTransactionsBySenderId(int senderId)
        {
            var transactions = await _walletTransactionService.GetTransactionsBySenderIdAsync(senderId);

            if (transactions == null)
            {
                return NotFound($"Sender with id {senderId} has no transactions.");
            }

            return Ok(transactions);
        }

        /// <summary>
        /// Retrieves all transactions received by a specific recipient.
        /// </summary>
        /// <param name="recipientId">The ID of the recipient whose transactions are to be retrieved.</param>
        /// <returns>A list of transactions if found; otherwise, an error message.</returns>
        [HttpGet("{recipientId}")]
        public async Task<IActionResult> GetTransactionsByRecipientId(int recipientId)
        {
            var transactions = await _walletTransactionService.GetTransactionsByRecipientIdAsync(recipientId);

            if (transactions == null)
            {
                return NotFound($"Recipient with id {recipientId} has no transactions.");
            }

            return Ok(transactions);
        }

        /// <summary>
        /// Retrieves a specific transaction by its ID.
        /// </summary>
        /// <param name="id">The ID of the transaction to retrieve.</param>
        /// <returns>The transaction details if found; otherwise, an error message.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var transaction = await _walletTransactionService.GetTransactionByIdAsync(id);

            if (transaction == null)
            {
                return NotFound($"Transaction with ID {id} not found.");
            }

            return Ok(transaction);
        }

        /// <summary>
        /// Adds a new wallet transaction.
        /// </summary>
        /// <param name="walletTransaction">The wallet transaction object to be added.</param>
        /// <returns>Not implemented exception, as the method is not yet functional.</returns>
        [HttpPost]
        public async Task<IActionResult> Add(WalletTransaction walletTransaction)
        {
            throw new NotImplementedException();
            //await _walletTransactionService.AddWalletTransactionAsync(walletTransaction);

            //return Ok();
        }
    }
}
