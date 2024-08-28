using VirtualWallet.DATA.Models.Contract;
using VirtualWallet.DATA.Models.Enums;

namespace VirtualWallet.DATA.Models
{
    public class CardTransaction
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public CurrencyType Currency { get; set; }
        public TransactionStatus Status { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int WalletId { get; set; }
        public Wallet Wallet { get; set; }     
        public int CardId { get; set; }
        public Card Card { get; set; }        
        public TransactionType TransactionType { get; set; }
        
    }

}
