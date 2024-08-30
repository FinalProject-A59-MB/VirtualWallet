using VirtualWallet.DATA.Models;
using VirtualWallet.WEB.Models.DTOs.AuthDTOs;
using VirtualWallet.WEB.Models.DTOs.CardDTOs;
using VirtualWallet.WEB.Models.DTOs.UserDTOs;
using VirtualWallet.WEB.Models.DTOs.WalletDTOs;

public interface IDtoMapper
{
    public UserProfile ToUserProfile(UserProfileRequestDto dto);

    public UserProfileResponseDto ToUserProfileResponseDto(UserProfile profile);

    public User ToUser(UserAccountRequestDto dto, UserProfile profile);

    public UserAccountResponseDto ToUserAccountResponseDto(User user);

    public Wallet ToWalletRequestDto(WalletRequestDto dto);

    public WalletTransactionDto ToWalletTransactionDto(WalletTransaction transaction); 

    public WalletResponseDto ToWallet(Wallet wallet);

    public CardResponseDto ToCardDto(Card card);

    public CardTransactionResponseDto ToCardTransactionDto(CardTransaction transaction);

    public Card ToCard(CardRequestDto dto);

    public CardResponseDto ToCardResponseDto(Card card);

    public CardTransactionResponseDto ToCardTransactionResponseDto(CardTransaction transaction);

    public User ToUser(UserAccountRequestDto dto);

    public BlockedRecordResponseDto ToBlockedRecordResponseDto(BlockedRecord blockedRecord);

    public WalletResponseDto ToWalletResponseDto(Wallet wallet);

    public UserContact ToUserContact(UserContactRequestDto dto);
    public UserContactResponseDto ToUserContactResponseDto(UserContact contact);

    public BlockedRecord ToBlockedRecord(BlockedRecordRequestDto blockedRecordDto);
    public BlockedRecord ToUnblockRecord(UnblockRecordRequestDto unblockRecordDto);
    public UnblockRecordResponseDto ToUnblockRecordResponseDto(BlockedRecord blockedRecord);

    public User ToUser(RegisterRequestDto dto);

}