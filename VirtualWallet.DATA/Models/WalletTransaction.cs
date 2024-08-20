using VirtualWallet.DATA.Models.Contract;
using VirtualWallet.DATA.Models.Enums;
using TransactionStatus = VirtualWallet.DATA.Models.Enums.TransactionStatus;

namespace VirtualWallet.DATA.Models
{
    public class WalletTransaction:ITransactionEvent
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public CurrencyType Currency { get; set; }
        public TransactionStatus Status { get; set; }
        public string Origin { get; set; } // Example "Sender.Name"
        public string Destination { get; set; } // Example "Receiver.Name"

        public int SenderId { get; set; }
        public Wallet Sender { get; set; }

        public int RecipientId { get; set; }
        public Wallet Recipient { get; set; }

        public TransactionCategory Category { get; set; }

    }



}
