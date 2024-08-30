using VirtualWallet.DATA.Models;
using VirtualWallet.WEB.Models.DTOs.CardDTOs;
using VirtualWallet.WEB.Models.DTOs.UserDTOs;
using VirtualWallet.WEB.Models.DTOs.WalletDTOs;

public interface IDtoMapper
{
    public UserResponseDto ToUserResponseDto(User user);

    public UserProfileDto ToUserProfileDto(UserProfile profile);

    public UserContactDto ToUserContactDto(UserContact contact);

    public CardResponseDto ToCardDto(Card card);

    public CardTransactionResponseDto ToCardTransactionDto(CardTransaction transaction);

    public User ToUser(UserRequestDto dto);

    public Card ToCard(CardRequestDto dto);

    public Wallet ToWallet(WalletRequestDto dto);

    public WalletTransactionDto ToWalletTransactionDto(WalletTransaction transaction); 

    public WalletResponseDto ToWalletDto(Wallet wallet);

    public UserRequestDto ToUserDto(User dto);

    public CardResponseDto ToCardResponseDto(Card card);

    public CardTransactionResponseDto ToCardTransactionResponseDto(CardTransaction transaction);

}