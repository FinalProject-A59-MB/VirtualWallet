using VirtualWallet.DATA.Models.Enums;

namespace VirtualWallet.WEB.Models.DTOs.UserDTOs
{
    public class UserContactResponseDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public int ContactId { get; set; }
        public string ContactUsername { get; set; }
        public DateTime AddedDate { get; set; }
        public FriendRequestStatus Status { get; set; }
        public int SenderId { get; set; }
        public string SenderUsername { get; set; }
        public string? Description { get; set; }
    }
}
