using VirtualWallet.DATA.Models.Enums;

namespace VirtualWallet.DATA.Models
{
    public class WalletTransaction
    {
        public int Id { get; set; }
        public decimal AmountSent { get; set; }
        public decimal AmountReceived { get; set; }
        public bool Request { get; set; }
        public DateTime? CreatedAt { get; set; }
        public CurrencyType CurrencySent { get; set; }
        public CurrencyType CurrencyReceived { get; set; }
        public TransactionStatus Status { get; set; }
        public int SenderId { get; set; }
        public Wallet Sender { get; set; }
        public int RecipientId { get; set; }
        public Wallet Recipient { get; set; }
        public string VerificationCode { get; set; }
    }

}
