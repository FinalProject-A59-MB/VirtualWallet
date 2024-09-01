using VirtualWallet.DATA.Models.Enums;

namespace VirtualWallet.WEB.Models.ViewModels.WalletViewModels
{
    public class WalletTransactionViewModel
    {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string Sender { get; set; }
        public string Recipient { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public TransactionStatus Status { get; set; }
    }
}
