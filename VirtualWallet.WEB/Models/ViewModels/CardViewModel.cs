namespace VirtualWallet.WEB.Models.ViewModels
{
    public class CardViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CardNumber { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string CardHolderName { get; set; }
        public string CardType { get; set; }

        public string Cvv { get; set; }
        public string PaymentProcessorToken { get; set; }
    }

}
