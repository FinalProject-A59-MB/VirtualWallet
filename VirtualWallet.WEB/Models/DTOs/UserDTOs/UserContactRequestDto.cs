using VirtualWallet.DATA.Models.Enums;

namespace VirtualWallet.WEB.Models.DTOs.UserDTOs
{
    public class UserContactRequestDto
    {
        public int UserId { get; set; }
        public int ContactId { get; set; }
        public DateTime AddedDate { get; set; }
        public FriendRequestStatus Status { get; set; }
        public int SenderId { get; set; }
        public string? Description { get; set; }
    }
}
