using VirtualWallet.WEB.Models.ViewModels.CardViewModels;
using VirtualWallet.WEB.Models.ViewModels.WalletViewModels;

namespace VirtualWallet.WEB.Models.ViewModels.AdminViewModels
{
    public class TransactionLogViewModel
    {
        public IEnumerable<CardTransactionViewModel> CardTransactions { get; set; }
        public IEnumerable<WalletTransactionViewModel> WalletTransactions { get; set; }
        public string SelectedTransactionType { get; set; }
        public string SearchQuery { get; set; }
    }
}
