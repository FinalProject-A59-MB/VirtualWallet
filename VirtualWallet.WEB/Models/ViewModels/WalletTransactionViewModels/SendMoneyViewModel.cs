using VirtualWallet.WEB.Models.ViewModels.WalletViewModels;

namespace VirtualWallet.WEB.Models.ViewModels.WalletTransactionViewModels
{
    public class SendMoneyViewModel
    {
        public IEnumerable<WalletViewModel>? From { get; set; }
        public int SenderWalletId { get; set; }
        public int RecipientId { get; set; }
        public decimal Amount { get; set; }
        public string? RecipientName { get; set; }
        public string? RecipientEmail { get; set; }
        public string? VerificationCode { get; set; }    
    }
}
