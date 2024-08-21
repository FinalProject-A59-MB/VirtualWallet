using VirtualWallet.DATA.Models.Enums;

namespace VirtualWallet.WEB.Models.ViewModels
{
    public class UserVerificationViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string PhotoIdUrl { get; set; }
        public string LicenseIdUrl { get; set; }
        public UserVerificationStatus VerificationStatus { get; set; }
    }
}
