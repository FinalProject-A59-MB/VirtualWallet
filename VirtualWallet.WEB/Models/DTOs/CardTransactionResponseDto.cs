namespace VirtualWallet.WEB.Models.DTOs
{
    public class CardTransactionResponseDto
    {
        public int Id { get; set; }
        public int CardId { get; set; }
        public string CardNumber { get; set; }
        public int WalletId { get; set; }
        public string WalletName { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string TransactionType { get; set; }
        public string Status { get; set; }
    }

}
