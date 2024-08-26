using VirtualWallet.DATA.Models;

namespace VirtualWallet.WEB.Models.ViewModels
{
    public class TransactionLogViewModel
    {
        public IEnumerable<CardTransactionViewModel> CardTransactions { get; set; }
        public IEnumerable<WalletTransactionViewModel> WalletTransactions { get; set; }
        public string SelectedTransactionType { get; set; }
        public string SearchQuery { get; set; } 
    }
}
