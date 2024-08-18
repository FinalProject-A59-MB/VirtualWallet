namespace VirtualWallet.WEB.Models.ViewModels
{
    public class CardTransactionViewModel
    {
        public int Id { get; set; }
        public int CardId { get; set; }
        public int WalletId { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string TransactionType { get; set; }
        public string Status { get; set; }
    }

}
