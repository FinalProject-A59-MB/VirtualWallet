using System;
using System.Collections.Generic;
using System.Linq;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.WEB.Models.DTOs;
using VirtualWallet.WEB.Models.ViewModels;

public class ViewModelMapper : IViewModelMapper
{
    public ViewModelMapper()
    {
    }

    public User ToUser(RegisterViewModel model)
    {
        return new User
        {
            Username = model.Username,
            Email = model.Email,
            Password = model.Password,
            
        };
    }

    public LoginViewModel ToLoginViewModel(User user)
    {
        throw new NotImplementedException();
    }

    public UserViewModel ToUserViewModel(User user)
    {
        return new UserViewModel
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role.ToString(),
            UserProfile = ToUserProfileViewModel(user.UserProfile),
            Cards = user.Cards?.Select(ToCardViewModel).ToList(),
            //Wallets = user.UserWallets?.Select(uw => ToWalletViewModel(uw.Wallet)).ToList(),
        };
    }

    public UserProfileViewModel ToUserProfileViewModel(UserProfile profile)
    {
        return new UserProfileViewModel
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

    public CardViewModel ToCardViewModel(Card card)
    {
        return new CardViewModel
        {
            Id = card.Id,
            Name = card.Name,
            CardNumber = card.CardNumber,
            ExpirationDate = card.ExpirationDate,
            CardHolderName = card.CardHolderName,
            CheckNumber = card.CheckNumber,
            CardType = card.CardType.ToString(),
        };
    }

    //public WalletViewModel ToWalletViewModel(Wallet wallet) TODO
    //{
    //}
}
