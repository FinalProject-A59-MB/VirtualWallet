using VirtualWallet.DATA.Models;
using VirtualWallet.WEB.Models.ViewModels;

public interface IViewModelMapper
{
    public UserViewModel ToUserViewModel(User user);

    public User ToUser(RegisterViewModel model);

    public LoginViewModel ToLoginViewModel(User user);

    public UserProfileViewModel ToUserProfileViewModel(UserProfile profile);

    public CardViewModel ToCardViewModel(Card card);

    //public WalletViewModel ToWalletViewModel(Wallet wallet); TODO

}