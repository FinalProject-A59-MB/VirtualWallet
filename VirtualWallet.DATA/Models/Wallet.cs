using VirtualWallet.DATA.Models.Enums;

namespace VirtualWallet.DATA.Models
{
    public class Wallet
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string Name { get; set; }
        public WalletType WalletType { get; set; }
        public decimal Balance { get; set; }
        public CurrencyType Currency { get; set; }
        public ICollection<UserWallet> UserWallets { get; set; }
        public ICollection<WalletTransaction> WalletTransactions { get; set; }
        public ICollection<WalletTransaction> SentTransactions { get; set; } = new List<WalletTransaction>();
        public ICollection<WalletTransaction> ReceivedTransactions { get; set; } = new List<WalletTransaction>();
        public ICollection<CardTransaction> CardTransactions { get; set; }
    }
}
