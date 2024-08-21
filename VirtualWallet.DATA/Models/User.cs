using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualWallet.DATA.Models.Contract;
using VirtualWallet.DATA.Models.Enums;

namespace VirtualWallet.DATA.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public UserProfile UserProfile { get; set; }
        public UserVerificationStatus VerificationStatus { get; set; }
        public string PhotoIdUrl { get; set; } = "default";
        public string FaceIdUrl { get; set; } = "default";
        public DateTime? DeletedAt { get; set; }
        public UserRole Role { get; set; }
        public string? GoogleId { get; set; }
        public ICollection<Card> Cards { get; set; } = new List<Card>();
        public ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
        public ICollection<UserWallet> UserWallets { get; set; } = new List<UserWallet>();
        public ICollection<UserContact> Contacts { get; set; } = new List<UserContact>();
        public ICollection<CardTransaction> CardTransactions { get; set; } = new List<CardTransaction>();
        public ICollection<WalletTransaction> WalletTransactions { get; set; } = new List<WalletTransaction>();
        public ICollection<RecurringPayment> RecurringPayments { get; set; } = new List<RecurringPayment>();
        public ICollection<BlockedRecord> BlockedRecords { get; set; } = new List<BlockedRecord>();
        public int? MainWalletId { get; set; }
        public Wallet? MainWallet { get; set; }
    }

}
