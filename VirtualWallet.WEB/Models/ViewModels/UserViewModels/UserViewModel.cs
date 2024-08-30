using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.WEB.Models.ViewModels.WalletViewModels;

namespace VirtualWallet.WEB.Models.ViewModels.UserViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public decimal? TotalBalance { get; set; }
        public UserVerificationStatus VerificationStatus { get; set; }
        public UserProfileViewModel UserProfile { get; set; }
        public ICollection<CardViewModel> Cards { get; set; }
        public ICollection<WalletViewModel> Wallets { get; set; }
        public WalletViewModel? MainWallet { get; set; }
        public ICollection<UserContact>? Contacts { get; set; }
    }
}
