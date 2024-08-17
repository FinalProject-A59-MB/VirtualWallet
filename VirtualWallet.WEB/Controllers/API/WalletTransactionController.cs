using Microsoft.AspNetCore.Mvc;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Services.Contracts;

namespace VirtualWallet.WEB.Controllers.API
{
    [ApiController]
    [Route("api/walletTransactions")]
    public class WalletTransactionController : ControllerBase
    {
        private readonly IWalletTransactionService _walletTransactionService;

        public WalletTransactionController(IWalletTransactionService walletTransactionService)
        {
            _walletTransactionService = walletTransactionService;
        }

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

        [HttpPost]
        public async Task<IActionResult> Add(WalletTransaction walletTransaction)
        {
            await _walletTransactionService.AddWalletTransactionAsync(walletTransaction);

            return Ok();
        }
    }
}
