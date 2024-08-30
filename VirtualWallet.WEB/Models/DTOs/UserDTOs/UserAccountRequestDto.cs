using VirtualWallet.DATA.Models.Enums;

namespace VirtualWallet.WEB.Models.DTOs.UserDTOs
{
    public class UserAccountRequestDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string? GoogleId { get; set; }
        public string? Password { get; set; }
        public UserVerificationStatus VerificationStatus { get; set; }
        public UserRole Role { get; set; }
        public int? MainWalletId { get; set; }
        public int? BlockedRecordId { get; set; }
        public string? PhotoIdUrl { get; set; }
        public string? FaceIdUrl { get; set; }
    }
}
