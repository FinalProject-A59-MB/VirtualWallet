using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.DATA.Models;

public interface IModelMapper
{
    public UserResponseDto ToUserResponseDto(User user);
    public UserProfileDto ToUserProfileDto(UserProfile profile);
    public UserContactDto ToUserContactDto(UserContact contact);
    public UserViewModel ToUserViewModel(User user);
    public UserProfileViewModel ToUserProfileViewModel(UserProfile profile);
    public CardDto ToCardDto(Card card);
    public CardViewModel ToCardViewModel(Card card);
    public CardTransactionDto ToCardTransactionDto(CardTransaction transaction);
    public User ToUser(UserRequestDto dto);
    public Card ToCard(CardRequestDto dto);
    public Wallet ToWallet(WalletRequestDto dto);
    public WalletTransactionDto ToWalletTransactionDto(WalletTransaction transaction);
    public WalletViewModel ToWalletViewModel(Wallet wallet);
    public WalletDto ToWalletDto(Wallet wallet);
}