using VirtualWallet.DATA.Models.Enums;

namespace VirtualWallet.WEB.Models.ViewModels.WalletViewModels
{
    public class WalletViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public WalletType WalletType { get; set; }
        public decimal Balance { get; set; }
        public CurrencyType Currency { get; set; }
        public IEnumerable<WalletTransactionViewModel> WalletTransactions { get; set; } = Enumerable.Empty<WalletTransactionViewModel>();
        public IEnumerable<UserWalletViewModel> JointUsers { get; set; } = Enumerable.Empty<UserWalletViewModel>();
    }
}