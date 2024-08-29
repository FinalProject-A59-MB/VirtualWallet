using VirtualWallet.DATA.Models;
using VirtualWallet.WEB.Models.ViewModels;

public interface IViewModelMapper
{
    public UserViewModel ToUserViewModel(User user);

    public User ToUser(RegisterViewModel model);

    public User ToUser(UserViewModel model);

    public UserProfile ToUserProfile(UserProfileViewModel model);
    public Card ToCard(CardViewModel model);

    public LoginViewModel ToLoginViewModel(User user);

    public UserProfileViewModel ToUserProfileViewModel(UserProfile profile);

    public CardViewModel ToCardViewModel(Card card);

    public WalletViewModel ToWalletViewModel(Wallet wallet);
    public Wallet ToWallet(WalletViewModel model);

    public CardTransactionViewModel ToCardTransactionViewModel(CardTransaction transaction);

    public CardTransaction ToCardTransaction(CardTransactionViewModel model);

    public UserVerificationViewModel ToUserVerificationViewModel(User u);

    public WalletTransactionViewModel ToWalletTransactionViewModel(WalletTransaction transaction, int userId);
}