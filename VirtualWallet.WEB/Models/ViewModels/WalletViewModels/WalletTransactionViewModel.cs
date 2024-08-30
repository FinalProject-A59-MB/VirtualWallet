using Twilio.Rest.Video.V1.Room.Participant;

namespace VirtualWallet.WEB.Models.ViewModels.WalletViewModels
{
    public class WalletTransactionViewModel
    {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string Sender { get; set; }
        public string Recipient { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Direction { get; set; }
        public string WalletName { get; set; }

        public string SortBy { get; set; }

        public string SortOrder { get; set; }
    }
}
