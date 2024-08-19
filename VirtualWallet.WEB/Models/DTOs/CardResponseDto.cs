namespace VirtualWallet.WEB.Models.DTOs
{
    public class CardResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CardNumber { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string CardHolderName { get; set; }

        public string Cvv { get; set; }

        public int UserId { get; set; }

        public string PaymentProcessorToken { get; set; }

        public string CardType { get; set; }

    }

}
