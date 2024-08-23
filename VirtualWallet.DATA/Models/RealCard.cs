using VirtualWallet.DATA.Models.Enums;

namespace VirtualWallet.DATA.Models
{
    public class RealCard
    {
        public int Id { get; set; }
        public string CardNumber { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Issuer { get; set; }
        public string CardHolderName { get; set; }
        public string Cvv { get; set; }
        public CardType CardType { get; set; }
        public decimal Balance { get; set; }
        public CurrencyType Currency { get; set; }
        public string PaymentProcessorToken { get; set; }
    }


}
