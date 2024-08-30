namespace VirtualWallet.WEB.Models.DTOs.CardDTOs
{
    public class CardResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string CardHolderName { get; set; }
        public string CardNumber { get; set; }
        public string ExpirationDate { get; set; }
        public string Issuer { get; set; }
        public string Cvv { get; set; }
        public string CardType { get; set; }
        public string Currency { get; set; }
    }

}
