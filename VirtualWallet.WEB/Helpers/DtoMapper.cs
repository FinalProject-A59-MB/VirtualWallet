using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.WEB.Models.DTOs;

public class DtoMapper : IDtoMapper
{
    public DtoMapper()
    {
    }

    public UserResponseDto ToUserResponseDto(User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role.ToString(),
            VerificationStatus = user.VerificationStatus.ToString(),
            UserProfile = ToUserProfileDto(user.UserProfile),
            Cards = user.Cards?.Select(ToCardDto).ToList(),
            //Wallets = user.UserWallets?.Select(uw => ToWalletDto(uw.Wallet)).ToList(),
            Contacts = user.Contacts?.Select(uc => ToUserContactDto(uc)).ToList(),
        };
    }

    public UserProfileDto ToUserProfileDto(UserProfile profile)
    {
        return new UserProfileDto
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
            PostalCode = profile.PostalCode,
            Country = profile.Country,
        };
    }

    public UserContactDto ToUserContactDto(UserContact contact)
    {
        return new UserContactDto
        {
            UserId = contact.UserId,
            ContactId = contact.ContactId,
            ContactName = contact.Contact.Username,
            AddedDate = contact.AddedDate,
        };
    }

    public CardResponseDto ToCardDto(Card card)
    {
        return new CardResponseDto
        {
            Id = card.Id,
            Name = card.Name,
            CardNumber = card.CardNumber,
            ExpirationDate = card.ExpirationDate,
            CardHolderName = card.CardHolderName,
            Cvv = card.Cvv,
            UserId = card.UserId,
            PaymentProcessorToken = card.PaymentProcessorToken,
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
            TransactionType = transaction.TransactionType.ToString(),
            Status = transaction.Status.ToString(),
        };
    }

    public User ToUser(UserRequestDto dto)
    {
        return new User
        {
            Username = dto.Username,
            Email = dto.Email,
            Password = dto.Password,
            Role = UserRole.RegisteredUser,
        };
    }

    public UserRequestDto ToUserDto(User dto)
    {
        return new UserRequestDto
        {
            Username = dto.Username,
            Email = dto.Email,
            Password = dto.Password,
            Role = UserRole.RegisteredUser,
        };
    }

    public Card ToCard(CardRequestDto dto)
    {
        return new Card
        {
            Name = dto.Name,
            CardNumber = dto.CardNumber,
            ExpirationDate = dto.ExpirationDate,
            CardHolderName = dto.CardHolderName,
            Cvv = dto.CheckNumber,
            UserId = dto.UserId,
        };
    }

    public Wallet ToWallet(WalletRequestDto dto)
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
            Amount = transaction.Amount,
            CreatedAt = transaction.CreatedAt,
            Status = transaction.Status.ToString(),
            SenderId = transaction.SenderId,
            SenderName = transaction.Sender.Name,
            RecipientId = transaction.RecipientId,
            RecipientName = transaction.Recipient.Name,
            Category = transaction.Category.ToString(),
        };
    }

    public WalletResponseDto ToWalletDto(Wallet wallet)
    {
        return new WalletResponseDto
        {
            Id = wallet.Id,
            UserId = wallet.UserId,
            User = wallet.User,
            Name = wallet.Name,
            Currency = wallet.Currency,
            WalletType = wallet.WalletType,
            Balance = wallet.Balance
        };
    }
}
