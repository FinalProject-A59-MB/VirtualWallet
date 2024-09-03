using VirtualWallet.DATA.Models.Enums;

namespace VirtualWallet.DATA.Models
{
    public class UserContact
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int ContactId { get; set; }
        public User Contact { get; set; }

        public DateTime AddedDate { get; set; }

        public FriendRequestStatus Status { get; set; }
        public int SenderId { get; set; }
        public User Sender { get; set; }
        public string? Description { get; set; }
        public ICollection<WalletTransaction> MoneyRequests { get; set; }
    }

}
