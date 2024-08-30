using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Models.Enums;

namespace VirtualWallet.WEB.Models.ViewModels.CardViewModels
{
    public class CardTransactionViewModel
    {
        public string? ActionTitle { get; set; }
        public string? FormAction { get; set; }
        public int Id { get; set; }
        public int CardId { get; set; }
        public Card? Card { get; set; }
        public int WalletId { get; set; }
        public Wallet? Wallet { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public decimal? Fee { get; set; }
    }

}
