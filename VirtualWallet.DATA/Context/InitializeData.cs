using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Helpers;

namespace VirtualWallet.DATA.Context
{
    public static class InitializeData
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (!context.Users.Any())
            {
                var users = new List<User>();

                // Admin User
                var adminUser = new User
                {
                    Username = "admin",
                    Password = PasswordHasher.HashPassword("admin"),
                    Email = "admin@example.com",
                    Role = UserRole.Admin,
                    VerificationStatus = UserVerificationStatus.Verified,
                    PhotoIdUrl = "test",
                    FaceIdUrl = "test",
                    UserProfile = new UserProfile
                    {
                        FirstName = "Admin",
                        LastName = "User",
                        DateOfBirth = new DateTime(1985, 1, 1),
                        Street = "123 Admin St",
                        City = "Adminville",
                        State = "Adminstate",
                        PostalCode = "12345",
                        Country = "Adminland",
                        PhoneNumber = "+1234567890",
                    }
                };
                users.Add(adminUser);

                // Active Users
                for (int i = 1; i <= 2; i++)
                {
                    var user = new User
                    {
                        Username = $"user{i}",
                        Password = PasswordHasher.HashPassword("password"),
                        Email = $"user{i}@example.com",
                        Role = i % 2 == 0 ? UserRole.VerifiedUser : UserRole.PendingVerification,
                        VerificationStatus = UserVerificationStatus.Verified,
                        PhotoIdUrl = "test",
                        FaceIdUrl = "test",
                        UserProfile = new UserProfile
                        {
                            FirstName = $"FirstName{i}",
                            LastName = $"LastName{i}",
                            DateOfBirth = new DateTime(1990 + i, 1, 1),
                            Street = $"Street {i}",
                            City = "City",
                            State = "State",
                            PostalCode = $"1000{i}",
                            Country = "Country",
                            PhoneNumber = $"+12345678{i}0",
                            
                        }
                    };
                    users.Add(user);
                }

                // Registered Users
                for (int i = 1; i <= 2; i++)
                {
                    var user = new User
                    {
                        Username = $"user{i}a",
                        Password = PasswordHasher.HashPassword("password"),
                        Email = $"user{i}a@example.com",
                        Role = i % 2 == 0 ? UserRole.VerifiedUser : UserRole.Staff,
                        VerificationStatus = UserVerificationStatus.Verified,
                        PhotoIdUrl = "test",
                        FaceIdUrl = "test",
                        UserProfile = new UserProfile
                        {
                            FirstName = $"FirstName{i}",
                            LastName = $"LastName{i}",
                            DateOfBirth = new DateTime(1990 + i, 1, 1),
                            Street = $"Street {i}",
                            City = "City",
                            State = "State",
                            PostalCode = $"1000{i}",
                            Country = "Country",
                            PhoneNumber = $"+12345678{i}0",
                            
                        }
                    };
                    users.Add(user);
                }

                // Registered Users (not verified)
                for (int i = 1; i <= 2; i++)
                {
                    var user = new User
                    {
                        Username = $"user{i}a",
                        Password = PasswordHasher.HashPassword("password"),
                        Email = $"user{i}a@example.com",
                        Role = i % 2 == 0 ? UserRole.PendingVerification : UserRole.RegisteredUser,
                        VerificationStatus = UserVerificationStatus.NotVerified,
                        PhotoIdUrl = "test",
                        FaceIdUrl = "test",
                        UserProfile = new UserProfile
                        {
                            FirstName = $"FirstName{i}",
                            LastName = $"LastName{i}",
                            DateOfBirth = new DateTime(1990 + i, 1, 1),
                            Street = $"Street {i}",
                            City = "City",
                            State = "State",
                            PostalCode = $"1000{i}",
                            Country = "Country",
                            PhoneNumber = $"+12345678{i}0",
                        }
                    };
                    users.Add(user);
                }

                // Registered Users (veirfied)
                for (int i = 1; i <= 2; i++)
                {
                    var user = new User
                    {
                        Username = $"user{i}b",
                        Password = PasswordHasher.HashPassword("password"),
                        Email = $"user{i}b@example.com",
                        Role = i % 2 == 0 ? UserRole.VerifiedUser : UserRole.RegisteredUser,
                        VerificationStatus = UserVerificationStatus.Verified,
                        PhotoIdUrl = "test",
                        FaceIdUrl = "test",
                        UserProfile = new UserProfile
                        {
                            FirstName = $"FirstName{i}",
                            LastName = $"LastName{i}",
                            DateOfBirth = new DateTime(1990 + i, 1, 1),
                            Street = $"Street {i}",
                            City = "City",
                            State = "State",
                            PostalCode = $"1000{i}",
                            Country = "Country",
                            PhoneNumber = $"+12345678{i}0",
                        }
                    };
                    users.Add(user);
                }

                context.Users.AddRange(users);
                context.SaveChanges();
            }


            if (!context.RealCards.Any())
            {
                var realCards = new List<RealCard>
                    {
                        new RealCard { CardHolderName = "Carole Raynor", CardNumber = "5252295372344564", Issuer = "mastercard", ExpirationDate = DateTime.ParseExact("03/26", "MM/yy", null), Cvv = "566", Balance = 10000, CardType = CardType.Debit, Currency = CurrencyType.EUR, PaymentProcessorToken = "f1a2b3c4d5e6f7g8h9i0j1k2l3m4n5o6" },
                        new RealCard { CardHolderName = "David Little-Quigley", CardNumber = "5561648452405182", Issuer = "mastercard", ExpirationDate = DateTime.ParseExact("12/25", "MM/yy", null), Cvv = "163", Balance = 10000, CardType = CardType.Credit,Currency = CurrencyType.BGN, PaymentProcessorToken = "p6q7r8s9t0u1v2w3x4y5z6a7b8c9d0e1" },
                        new RealCard { CardHolderName = "Carroll Hauck", CardNumber = "5465592714992412", Issuer = "mastercard", ExpirationDate = DateTime.ParseExact("03/27", "MM/yy", null), Cvv = "615", Balance = 10000, CardType = CardType.Debit, Currency = CurrencyType.USD,PaymentProcessorToken = "f7g8h9i0j1k2l3m4n5o6p1a2b3c4d5e6" },
                        new RealCard { CardHolderName = "Olga Bayer-Ziemann", CardNumber = "5538650662283458", Issuer = "mastercard", ExpirationDate = DateTime.ParseExact("07/29", "MM/yy", null), Cvv = "952", Balance = 10000, CardType = CardType.Credit,Currency = CurrencyType.BGN, PaymentProcessorToken = "q9r0s1t2u3v4w5x6y7z8a9b0c1d2e3f4" },
                        new RealCard { CardHolderName = "Percy Conn", CardNumber = "5265373215659145", Issuer = "mastercard", ExpirationDate = DateTime.ParseExact("07/26", "MM/yy", null), Cvv = "168", Balance = 10000, CardType = CardType.Debit,Currency = CurrencyType.USD, PaymentProcessorToken = "k5l6m7n8o9p0q1r2s3t4u5v6w7x8y9z0" },
                        new RealCard { CardHolderName = "Miss Mandy Orn", CardNumber = "4194143353329267", Issuer = "visa", ExpirationDate = DateTime.ParseExact("12/29", "MM/yy", null), Cvv = "948", Balance = 10000, CardType = CardType.Credit, Currency = CurrencyType.EUR,PaymentProcessorToken = "a1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6" },
                        new RealCard { CardHolderName = "Pearl Wolf-Corwin", CardNumber = "4551531347402371", Issuer = "visa", ExpirationDate = DateTime.ParseExact("07/28", "MM/yy", null), Cvv = "874", Balance = 10000, CardType = CardType.Debit, Currency = CurrencyType.BGN,PaymentProcessorToken = "g7h8i9j0k1l2m3n4o5p6q7r8s9t0u1v2" },
                        new RealCard { CardHolderName = "Toni Legros Sr.", CardNumber = "4823639117708018", Issuer = "visa", ExpirationDate = DateTime.ParseExact("02/28", "MM/yy", null), Cvv = "937", Balance = 10000, CardType = CardType.Credit,Currency = CurrencyType.USD, PaymentProcessorToken = "d5e6f7g8h9i0j1k2l3m4n5o6p7q8r9s0" },
                        new RealCard { CardHolderName = "Jacob Jerde", CardNumber = "4937424799514692", Issuer = "visa", ExpirationDate = DateTime.ParseExact("02/27", "MM/yy", null), Cvv = "050", Balance = 10000, CardType = CardType.Debit, Currency = CurrencyType.BGN,PaymentProcessorToken = "r3s4t5u6v7w8x9y0z1a2b3c4d5e6f7g8" },
                        new RealCard { CardHolderName = "Malcolm Ortiz", CardNumber = "4304178372051564", Issuer = "visa", ExpirationDate = DateTime.ParseExact("01/27", "MM/yy", null), Cvv = "105", Balance = 10000, CardType = CardType.Credit, Currency = CurrencyType.EUR,PaymentProcessorToken = "m8n9o0p1q2r3s4t5u6v7w8x9y0z1a2b3" }
                    };

                context.RealCards.AddRange(realCards);
                context.SaveChanges();
            }

        }
    }
}
