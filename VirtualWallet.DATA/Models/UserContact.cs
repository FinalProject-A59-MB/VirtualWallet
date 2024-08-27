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

        public FriendRequestStatus Status { get; set; } // Existing property

        // New properties to track the sender of the friend request
        public int SenderId { get; set; }
        public User Sender { get; set; } // Navigation property for the sender

        public string? Description { get; set; } // New optional property
    }

}
