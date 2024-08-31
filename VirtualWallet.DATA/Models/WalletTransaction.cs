using VirtualWallet.DATA.Models.Enums;
using TransactionStatus = VirtualWallet.DATA.Models.Enums.TransactionStatus;

namespace VirtualWallet.DATA.Models
{
    public class WalletTransaction
    {
        public int Id { get; set; }
        public decimal WithdrownAmount { get; set; }
        public decimal DepositedAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public CurrencyType Currency { get; set; }
        public TransactionStatus Status { get; set; }

        public int SenderId { get; set; }
        public Wallet Sender { get; set; }

        public int RecipientId { get; set; }
        public Wallet Recipient { get; set; }

        public string VerificationCode { get; set; }
    }

}
