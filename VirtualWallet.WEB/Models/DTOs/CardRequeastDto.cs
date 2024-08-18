namespace VirtualWallet.WEB.Models.DTOs
{
    public class CardRequestDto
    {
        public string Name { get; set; }
        public string CardNumber { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string CardHolderName { get; set; }
        public string CheckNumber { get; set; }
        public int UserId { get; set; }
        public string CardType { get; set; }
    }

}
