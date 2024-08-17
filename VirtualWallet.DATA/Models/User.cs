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
        public string PhotoIdUrl { get; set; }
        public string FaceIdUrl { get; set; }
        public DateTime? DeletedAt { get; set; }
        public UserRole Role { get; set; }
        public string? GoogleId { get; set; }
        public ICollection<Card> Cards { get; set; }
        public ICollection<Wallet> Wallets { get; set; }
        public ICollection<UserWallet> UserWallets { get; set; }
        public ICollection<UserContact> Contacts { get; set; }
        public ICollection<CardTransaction> CardTransactions { get; set; }
        public ICollection<WalletTransaction> WalletTransactions { get; set; }
        public ICollection<RecurringPayment> RecurringPayments { get; set; }
        public ICollection<BlockedRecord> BlockedRecords { get; set; }
        public int? DefaultWalletId { get; set; }
        public Wallet DefaultWallet { get; set; }
    }
}
