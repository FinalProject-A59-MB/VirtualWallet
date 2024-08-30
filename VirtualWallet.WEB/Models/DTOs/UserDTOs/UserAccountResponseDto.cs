using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.WEB.Models.DTOs.WalletDTOs;

namespace VirtualWallet.WEB.Models.DTOs.UserDTOs
{
    public class UserAccountResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public UserVerificationStatus VerificationStatus { get; set; }
        public UserRole Role { get; set; }
        public string? GoogleId { get; set; }
        public UserProfileResponseDto UserProfile { get; set; }
        public decimal TotalBalance { get; set; }
        public WalletResponseDto? MainWallet { get; set; }
        public BlockedRecordResponseDto? BlockedRecord { get; set; }
        public string? PhotoIdUrl { get; set; }
        public string? FaceIdUrl { get; set; }
    }

}
