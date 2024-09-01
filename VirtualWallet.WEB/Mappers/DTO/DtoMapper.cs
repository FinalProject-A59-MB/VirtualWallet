using VirtualWallet.DATA.Models;
using VirtualWallet.WEB.Models.DTOs.AuthDTOs;
using VirtualWallet.WEB.Models.DTOs.CardDTOs;
using VirtualWallet.WEB.Models.DTOs.UserDTOs;
using VirtualWallet.WEB.Models.DTOs.WalletDTOs;

public class DtoMapper : IDtoMapper
{
    public DtoMapper()
    {
    }

    public UserProfile ToUserProfile(UserProfileRequestDto dto)
    {
        return new UserProfile
        {
            Id = dto.Id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhotoUrl = dto.PhotoUrl,
            PhoneNumber = dto.PhoneNumber,
            DateOfBirth = dto.DateOfBirth,
            Street = dto.Street,
            City = dto.City,
            State = dto.State,
            Country = dto.Country,
            PostalCode = dto.PostalCode,
            UserId = dto.Id
        };
    }

    public UserProfileResponseDto ToUserProfileResponseDto(UserProfile profile)
    {
        return new UserProfileResponseDto
        {
            Id = profile.Id,
            FirstName = profile.FirstName,
            LastName = profile.LastName,
            PhotoUrl = profile.PhotoUrl,
            PhoneNumber = profile.PhoneNumber,
            DateOfBirth = profile.DateOfBirth,
            Street = profile.Street,
            City = profile.City,
            State = profile.State,
            Country = profile.Country,
            PostalCode = profile.PostalCode
        };
    }


    public User ToUser(UserAccountRequestDto dto, UserProfile profile)
    {
        return new User
        {
            Id = dto.Id,
            Username = dto.Username,
            Email = dto.Email,
            GoogleId = dto.GoogleId,
            Password = dto.Password,
            VerificationStatus = dto.VerificationStatus,
            Role = dto.Role,
            UserProfile = profile
        };
    }


    public User ToUser(UserAccountRequestDto dto)
    {
        var user = new User
        {
            Id = dto.Id,
            Username = dto.Username,
            Email = dto.Email,
            GoogleId = dto.GoogleId,
            VerificationStatus = dto.VerificationStatus,
            Role = dto.Role,
            MainWalletId = dto.MainWalletId,
            BlockedRecordId = dto.BlockedRecordId,
        };

        if (dto.Password != null)
        {
            user.Password = dto.Password;
        }

        if (dto.PhotoIdUrl != null)
        {
            user.PhotoIdUrl = dto.PhotoIdUrl;
        }

        if (dto.FaceIdUrl != null)
        {
            user.FaceIdUrl = dto.FaceIdUrl;
        }

        return user;
    }

    public UserAccountResponseDto ToUserAccountResponseDto(User user)
    {
        return new UserAccountResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            VerificationStatus = user.VerificationStatus,
            Role = user.Role,
            GoogleId = user.GoogleId,
            UserProfile = ToUserProfileResponseDto(user.UserProfile),
            TotalBalance = user.Wallets.Sum(w => w.Balance),
            MainWallet = user.MainWallet != null ? ToWalletResponseDto(user.MainWallet) : null,
            BlockedRecord = user.BlockedRecord != null ? ToBlockedRecordResponseDto(user.BlockedRecord) : null,
            PhotoIdUrl = user.PhotoIdUrl,
            FaceIdUrl = user.FaceIdUrl
        };
    }
    public UserContact ToUserContact(UserContactRequestDto dto)
    {
        return new UserContact
        {
            UserId = dto.UserId,
            ContactId = dto.ContactId,
            AddedDate = dto.AddedDate,
            Status = dto.Status,
            SenderId = dto.SenderId,
            Description = dto.Description
        };
    }


    public UserContactResponseDto ToUserContactResponseDto(UserContact contact)
    {
        return new UserContactResponseDto
        {
            UserId = contact.UserId,
            Username = contact.User.Username,
            ContactId = contact.ContactId,
            ContactUsername = contact.Contact.Username,
            AddedDate = contact.AddedDate,
            Status = contact.Status,
            SenderId = contact.SenderId,
            SenderUsername = contact.Sender.Username,
            Description = contact.Description
        };
    }



    public BlockedRecordResponseDto ToBlockedRecordResponseDto(BlockedRecord blockedRecord)
    {
        return new BlockedRecordResponseDto
        {
            UserId = blockedRecord.UserId,
            Reason = blockedRecord.Reason,
            Username = blockedRecord.User.Username
        };
    }
    public BlockedRecord ToBlockedRecord(BlockedRecordRequestDto blockedRecordDto)
    {
        return new BlockedRecord
        {
            UserId = blockedRecordDto.UserId,
            Reason = blockedRecordDto.Reason,
        };
    }

    public BlockedRecord ToUnblockRecord(UnblockRecordRequestDto unblockRecordDto)
    {
        return new BlockedRecord
        {
            UserId = unblockRecordDto.UserId,
            Reason = unblockRecordDto.Reason,
        };
    }

    public UnblockRecordResponseDto ToUnblockRecordResponseDto(BlockedRecord blockedRecord)
    {
        return new UnblockRecordResponseDto
        {
            UserId = blockedRecord.UserId,
            Username = blockedRecord.User.Username,
            Reason = blockedRecord.Reason,
        };
    }

    public User ToUser(RegisterRequestDto dto)
    {
        return new User
        {
            Username = dto.Username,
            Email = dto.Email,
            Password = dto.Password,
        };
    }



    public WalletResponseDto ToWalletResponseDto(Wallet wallet)
    {
        return new WalletResponseDto
        {
            Id = wallet.Id,
            UserId = wallet.UserId,
            Name = wallet.Name,
            WalletType = wallet.WalletType,
            Balance = wallet.Balance,
            Currency = wallet.Currency,
        };
    }



    public CardResponseDto ToCardDto(Card card)
    {
        return new CardResponseDto
        {
            Id = card.Id,
            Name = card.Name,
            CardNumber = card.CardNumber,
            ExpirationDate = card.ExpirationDate.ToString(),
            CardHolderName = card.CardHolderName,
            Cvv = card.Cvv,
            UserId = card.UserId,
            CardType = card.CardType.ToString(),
        };
    }

    public CardTransactionResponseDto ToCardTransactionDto(CardTransaction transaction)
    {
        return new CardTransactionResponseDto
        {
            Id = transaction.Id,
            CardId = transaction.CardId,
            WalletId = transaction.WalletId,
            Amount = transaction.Amount,
            CreatedAt = transaction.CreatedAt,
            TransactionType = transaction.TransactionType,
            Status = transaction.Status.ToString(),
        };
    }

    public CardTransactionResponseDto ToCardTransactionResponseDto(CardTransaction transaction)
    {
        return new CardTransactionResponseDto
        {
            Id = transaction.Id,
            CardId = transaction.CardId,
            WalletId = transaction.WalletId,
            Amount = transaction.Amount,
            Fee = transaction.Fee,
            Currency = transaction.Currency.ToString(),
            TransactionType = transaction.TransactionType,
            Status = transaction.Status.ToString(),
            CreatedAt = transaction.CreatedAt
        };
    }

    public Card ToCard(CardRequestDto dto)
    {
        return new Card
        {
            Name = dto.Name,
            CardNumber = dto.CardNumber,
            ExpirationDate = DateTime.ParseExact(dto.ExpirationDate, "MM/yy", null),
            CardHolderName = dto.CardHolderName,
            Cvv = dto.Cvv,
            UserId = dto.UserId,
            Issuer = dto.Issuer
        };
    }

    public Wallet ToWalletRequestDto(WalletRequestDto dto)
    {
        return new Wallet
        {
            Id = dto.Id,
            Name = dto.Name,
            UserId = dto.UserId,
            Currency = dto.Currency,
            WalletType = dto.WalletType
        };
    }

    public WalletTransactionDto ToWalletTransactionDto(WalletTransaction transaction)
    {
        return new WalletTransactionDto
        {
            Id = transaction.Id,
            Amount = transaction.DepositedAmount,
            CreatedAt = transaction.CreatedAt,
            Status = transaction.Status.ToString(),
            SenderId = transaction.SenderId,
            SenderName = transaction.Sender.Name,
            RecipientId = transaction.RecipientId,
            RecipientName = transaction.Recipient.Name,
        };
    }

    public WalletResponseDto ToWallet(Wallet wallet)
    {
        return new WalletResponseDto
        {
            Id = wallet.Id,
            UserId = wallet.UserId,
            Name = wallet.Name,
            Currency = wallet.Currency,
            WalletType = wallet.WalletType,
            Balance = wallet.Balance
        };
    }

    public CardResponseDto ToCardResponseDto(Card card)
    {
        return new CardResponseDto
        {
            Id = card.Id,
            Name = card.Name,
            CardNumber = card.CardNumber,
            ExpirationDate = card.ExpirationDate.ToString("MM/yy"),
            CardHolderName = card.CardHolderName,
            Cvv = card.Cvv,
            CardType = card.CardType.ToString(),
            Currency = card.Currency.ToString(),
        };
    }


}
