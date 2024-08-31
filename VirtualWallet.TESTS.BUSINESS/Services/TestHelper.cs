using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.DATA.Models;
using Microsoft.AspNetCore.Identity;
using VirtualWallet.DATA.Helpers;

namespace VirtualWallet.TESTS.BUSINESS.Services
{
    public static class TestHelper
    {


        public static User GetTestUser()
        {
            return new User
            {
                Id = 1,
                Username = "testuser",
                Email = "testuser@example.com",
                Password = PasswordHasher.HashPassword("hashedpassword"),
                Role = UserRole.RegisteredUser,
                VerificationStatus = UserVerificationStatus.NotVerified,
                UserProfile = GetTestUserProfile(),
                Wallets = new List<Wallet> { GetTestWallet() },
                Contacts = new List<UserContact>(),
                MainWalletId = 1
            };
        }

        public static UserProfile GetTestUserProfile()
        {
            return new UserProfile
            {
                Id = 1,
                FirstName = "Test",
                LastName = "User",
                PhoneNumber = "1234567890",
                DateOfBirth = new DateTime(1990, 1, 1),
                Street = "123 Main St",
                City = "Test City",
                State = "TS",
                PostalCode = "12345",
                Country = "Test Country",
                UserId = 1
            };
        }


        public static User GetTestUser2()
        {
            return new User
            {
                Id = 2,
                Username = "testuser2",
                Email = "testuser2@example.com",
                Password = PasswordHasher.HashPassword("hashedpassword2"),
                Role = UserRole.RegisteredUser,
                VerificationStatus = UserVerificationStatus.NotVerified,
                UserProfile = GetTestUserProfile2(),
                Wallets = new List<Wallet> { GetTestWallet() },
                Contacts = new List<UserContact>(),
                MainWalletId = 2
            };
        }

        public static UserProfile GetTestUserProfile2()
        {
            return new UserProfile
            {
                Id = 2,
                FirstName = "Test2",
                LastName = "User2",
                PhoneNumber = "12345678902",
                DateOfBirth = new DateTime(1990, 1, 2),
                Street = "1232 Main St",
                City = "Test City 2",
                State = "TS 2",
                PostalCode = "123452",
                Country = "Test Country 2",
                UserId = 2
            };
        }

        public static Wallet GetTestWallet()
        {
            return new Wallet
            {
                Id = 1,
                Name = "Main Wallet",
                Balance = 100.0m,
                Currency = CurrencyType.USD,
                WalletType = WalletType.Main,
                UserId = 1
            };
        }

        public static BlockedRecord GetTestBlockedRecord()
        {
            return new BlockedRecord
            {
                Id = 1,
                UserId = 1,
                Reason = "Violation of terms",
                BlockedDate = DateTime.UtcNow
            };
        }

        public static UserContact GetTestUserContact()
        {
            return new UserContact
            {
                UserId = 1,
                ContactId = 2,
                AddedDate = DateTime.UtcNow,
                Status = FriendRequestStatus.Pending,
                SenderId = 1
            };
        }

    }
}
