namespace VirtualWallet.WEB.Models.DTOs
{
    public class CardTransactionRequestDto
    {
        public int CardId { get; set; }
        public int WalletId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }
    }

}
