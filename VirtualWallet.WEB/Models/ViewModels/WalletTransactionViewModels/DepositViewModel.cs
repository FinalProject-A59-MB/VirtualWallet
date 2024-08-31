using VirtualWallet.WEB.Models.ViewModels.WalletViewModels;

namespace VirtualWallet.WEB.Models.ViewModels.WalletTransactionViewModels
{
    public class DepositViewModel
    {
        public IEnumerable<WalletViewModel> From { get; set; }
        public IEnumerable<WalletViewModel> To {  get; set; }
    }
}
