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

        public static Card GetTestCard()
        {
            return new Card
            {
                Id = 1,
                Name = "Test Card",
                CardNumber = "1234567812345678",
                ExpirationDate = new DateTime(2025, 12, 31),
                CardHolderName = "Test User",
                Currency = CurrencyType.USD,
                Issuer = "Test Bank",
                Cvv = "123",
                UserId = 1,
                User = GetTestUser(), 
                CardType = CardType.Credit,
                PaymentProcessorToken = "token_12345",
                CardTransactions = new List<CardTransaction>()
            };
        }

        public static Card GetTestCard2()
        {
            return new Card
            {
                Id = 2,
                Name = "Test Card2",
                CardNumber = "12345678123456782",
                ExpirationDate = new DateTime(2025, 12, 31),
                CardHolderName = "Test User2",
                Currency = CurrencyType.USD,
                Issuer = "Test Bank2",
                Cvv = "1232",
                UserId = 2,
                User = GetTestUser2(),
                CardType = CardType.Credit,
                PaymentProcessorToken = "token_123452",
                CardTransactions = new List<CardTransaction>()
            };
        }

        public static CardTransaction GetTestCardTransaction()
        {
            return new CardTransaction
            {
                Id = 1,
                Amount = 100.00m,
                CreatedAt = DateTime.Now,
                Currency = CurrencyType.USD,
                Status = TransactionStatus.Completed,
                UserId = 1,
                User = GetTestUser(),
                WalletId = 1,
                Wallet = GetTestWallet(),
                CardId = 1,
                Card = GetTestCard(),
                TransactionType = TransactionType.Withdrawal,
                Fee = 2.00m
            };
        }

        public static CardTransaction GetTestCardTransaction2()
        {
            return new CardTransaction
            {
                Id = 2,
                Amount = 200.00m,
                CreatedAt = DateTime.Now,
                Currency = CurrencyType.USD,
                Status = TransactionStatus.Completed,
                UserId = 2,
                User = GetTestUser2(),
                WalletId = 2,
                Wallet = GetTestWallet2(),
                CardId = 2,
                Card = GetTestCard2(),
                TransactionType = TransactionType.Withdrawal,
                Fee = 2.02m
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

        public static Wallet GetTestWallet2()
        {
            return new Wallet
            {
                Id = 2,
                Name = "Main Wallet2",
                Balance = 200.0m,
                Currency = CurrencyType.USD,
                WalletType = WalletType.Main,
                UserId = 2
            };
        }

        public static WalletTransaction GetTestWalletTransaction()
        {
            return new WalletTransaction
            {
                Id = 1,
                WithdrownAmount = 100m,
                DepositedAmount = 100m,
                CreatedAt = DateTime.UtcNow,
                Currency = CurrencyType.USD,
                Status = TransactionStatus.Pending,
                SenderId = 1,
                Sender = GetTestWallet(),
                RecipientId = 2,
                Recipient = GetTestWallet(),
                VerificationCode = "1234"
            };
        }

        public static WalletTransaction GetTestWalletTransaction2()
        {
            return new WalletTransaction
            {
                Id = 2,
                WithdrownAmount = 200m,
                DepositedAmount = 200m,
                CreatedAt = DateTime.UtcNow,
                Currency = CurrencyType.USD,
                Status = TransactionStatus.Pending,
                SenderId = 2,
                Sender = GetTestWallet2(),
                RecipientId = 1,
                Recipient = GetTestWallet2(),
                VerificationCode = "12342"
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
