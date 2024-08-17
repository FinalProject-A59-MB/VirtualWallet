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
        public IActionResult GetTransactionsBySenderId(int senderId)
        {
            var transactions = _walletTransactionService.GetTransactionsBySenderId(senderId);

            if (transactions == null)
            {
                return NotFound($"Sender with id {senderId} has no transactions.");
            }

            return Ok(transactions);
        }

        [HttpGet("{recipientId}")]
        public IActionResult GetTransactionsByRecipientId(int recipientId)
        {
            var transactions = _walletTransactionService.GetTransactionsByRecipientId(recipientId);

            if (transactions == null)
            {
                return NotFound($"Recipient with id {recipientId} has no transactions.");
            }

            return Ok(transactions);
        }

        [HttpGet("{id}")]
        public IActionResult GetTransactionById(int id)
        {
            var transaction = _walletTransactionService.GetTransactionById(id);

            if (transaction == null)
            {
                return NotFound($"Transaction with ID {id} not found.");
            }

            return Ok(transaction);
        }

        [HttpPost]
        public ActionResult AddWalletTransaction(WalletTransaction walletTransaction)
        {
            _walletTransactionService.AddWalletTransaction(walletTransaction);

            return Ok();
        }
    }
}
