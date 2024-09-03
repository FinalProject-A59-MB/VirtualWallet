namespace VirtualWallet.WEB.Models.ViewModels.WalletTransactionViewModels
{
    public class RequestMoneyViewModel
    {
        public int SenderId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }

}
