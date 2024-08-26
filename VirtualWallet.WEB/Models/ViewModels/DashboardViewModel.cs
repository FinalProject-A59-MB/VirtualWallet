namespace VirtualWallet.WEB.Models.ViewModels
{
    public class DashboardViewModel
    {
        public UserViewModel User { get; set; }
        public decimal TotalBalance { get; set; }
        public List<WalletViewModel> Wallets { get; set; } = new List<WalletViewModel>();

        public string PartialViewContent { get; set; }
    }
}
