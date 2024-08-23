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
        public ICollection<RecurringPayment> RecurringPayments { get; set; } = new List<RecurringPayment>();
        public int? MainWalletId { get; set; }
        public Wallet? MainWallet { get; set; }
        public int? BlockedRecordId { get; set; }
        public BlockedRecord? BlockedRecord { get; set; }
    }


}
