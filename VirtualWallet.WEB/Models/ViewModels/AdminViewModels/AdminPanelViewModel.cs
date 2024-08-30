using VirtualWallet.WEB.Models.ViewModels.CardViewModels;
using VirtualWallet.WEB.Models.ViewModels.UserViewModels;
using VirtualWallet.WEB.Models.ViewModels.WalletViewModels;

namespace VirtualWallet.WEB.Models.ViewModels.AdminViewModels
{
    public class AdminPanelViewModel
    {
        public IEnumerable<UserViewModel> Users { get; set; }
        public int UsersTotalPages { get; set; }
        public int UsersCurrentPage { get; set; }

        public IEnumerable<WalletTransactionViewModel> WalletTransactions { get; set; }
        public int WalletTransactionsTotalPages { get; set; }
        public int WalletTransactionsCurrentPage { get; set; }

        public IEnumerable<CardTransactionViewModel> CardTransactions { get; set; }
        public int CardTransactionsTotalPages { get; set; }
        public int CardTransactionsCurrentPage { get; set; }
    }

}
