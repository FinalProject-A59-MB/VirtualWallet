using VirtualWallet.DATA.Models;

namespace VirtualWallet.WEB.Models.ViewModels
{
    public class CardTransactionViewModel
    {
        public string ActionTitle { get; set; }
        public string FormAction { get; set; }
        public int Id { get; set; }
        public int CardId { get; set; }
        public int WalletId { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Wallet> Wallets { get; set; }

        public ICollection<Card> Cards { get; set; }
    }

}
