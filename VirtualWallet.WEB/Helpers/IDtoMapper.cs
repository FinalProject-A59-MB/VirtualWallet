using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.DATA.Models;
using VirtualWallet.WEB.Models.DTOs;

public interface IDtoMapper
{
    public UserResponseDto ToUserResponseDto(User user);

    public UserProfileDto ToUserProfileDto(UserProfile profile);

    public UserContactDto ToUserContactDto(UserContact contact);

    public CardResponseDto ToCardDto(Card card);

    public CardTransactionResponseDto ToCardTransactionDto(CardTransaction transaction);

    public User ToUser(UserRequestDto dto);

    public Card ToCard(CardRequestDto dto);

    //public Wallet ToWallet(WalletRequestDto dto); TODO


    //public WalletTransactionDto ToWalletTransactionDto(WalletTransaction transaction); TODO


    //public WalletDto ToWalletDto(Wallet wallet); TODO

}