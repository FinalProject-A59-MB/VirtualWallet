using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Helpers;
using System;

namespace VirtualWallet.DATA.Context
{
    public static class InitializeData
    {
        private static readonly Random _random = new Random();
        public static void Initialize(ApplicationDbContext context)
        {

            context.Database.EnsureCreated();

            if (!context.Users.Any())
            {
                var userList = new List<dynamic>
                {
                    new { FirstName = "John", LastName = "Doe", Email = "JohnDoe@example.com", Role = UserRole.Staff, VerificationStatus = UserVerificationStatus.Verified, DateOfBirth = new DateTime(1985, 5, 15), Street = "4567 Oak Ridge Drive", City = "Denver", State = "CO", PostalCode = "10002", Country = "USA", PhoneNumber = "1123467891", PhotoIdUrl = "/peopleImages/man1/man1_id.png", FaceIdUrl = "/peopleImages/man1/man1_photo.png" , PhotoUrl = "/peopleImages/man1/man1_profile.png" },
                    new { FirstName = "Jane", LastName = "Smith", Email = "JaneSmith@example.com", Role = UserRole.Staff, VerificationStatus = UserVerificationStatus.Verified, DateOfBirth = new DateTime(1990, 3, 22), Street = "Via della Conciliazione 10", City = "Rome", State = "Lazio", PostalCode = "00193", Country = "Italy", PhoneNumber = "1124567892", PhotoIdUrl = "/peopleImages/woman3/woman3_id.png", FaceIdUrl = "/peopleImages/woman3/woman3_photo.png" , PhotoUrl = "/peopleImages/woman3/man3_profile.png" },
                    new { FirstName = "Tom", LastName = "Johnson", Email = "TomJohnson@example.com", Role = UserRole.Blocked, VerificationStatus = UserVerificationStatus.Verified, DateOfBirth = new DateTime(1978, 7, 12), Street = "42 Hachiko-mae Square", City = "Shibuya", State = "Tokyo", PostalCode = "150-0001", Country = "Japan", PhoneNumber = "1234567893", PhotoIdUrl = "/peopleImages/man4/man4_id.png", FaceIdUrl = "/peopleImages/man4/man4_photo.png" , PhotoUrl = "/peopleImages/man4/man4_profile.png" },
                    new { FirstName = "Lisa", LastName = "Brown", Email = "LisaBrown@example.com", Role = UserRole.Blocked, VerificationStatus = UserVerificationStatus.Verified, DateOfBirth = new DateTime(1982, 11, 25), Street = "123 Queen Street", City = "Toronto", State = "Ontario", PostalCode = "M5H 2N2", Country = "Canada", PhoneNumber = "1234567894", PhotoIdUrl = "/peopleImages/woman4/man4_id.png", FaceIdUrl = "/peopleImages/woman4/woman4_photo.png" , PhotoUrl = "/peopleImages/woman4/woman4_profile.png" },
                    new { FirstName = "Harry", LastName = "Williams", Email = "HarryWilliams@example.com", Role = UserRole.VerifiedUser, VerificationStatus = UserVerificationStatus.Verified, DateOfBirth = new DateTime(1991, 9, 9), Street = "12 Rue de Rivoli", City = "Paris", State = "le-de-France", PostalCode = "75004", Country = "France", PhoneNumber = "1232527896",  PhotoIdUrl = "/peopleImages/man3/man3_id.png", FaceIdUrl = "/peopleImages/man3/man3_photo.png" , PhotoUrl = "/peopleImages/man3/man3_profile.png"},
                    new { FirstName = "Kate", LastName = "Wilson", Email = "KateWilson@example.com", Role = UserRole.VerifiedUser, VerificationStatus = UserVerificationStatus.Verified, DateOfBirth = new DateTime(1989, 2, 14), Street = "2380 Sycamore Drive", City = "Columbus", State = "OH", PostalCode = "43215", Country = "USA", PhoneNumber = "1234567896", PhotoIdUrl = "/peopleImages/woman2/woman2_id.png", FaceIdUrl = "/peopleImages/woman2/woman2_photo.png" , PhotoUrl = "/peopleImages/woman2/woman2_profile.png" },
                    new { FirstName = "Samuel", LastName = "Garcia", Email = "SamuelGarcia@example.com", Role = UserRole.VerifiedUser, VerificationStatus = UserVerificationStatus.Verified, DateOfBirth = new DateTime(1988, 12, 5), Street = "564 Willowbrook Drive", City = "Seattle", State = "WA", PostalCode = "98104", Country = "USA", PhoneNumber = "1234567897", PhotoIdUrl = "/peopleImages/man5/man5_id.png", FaceIdUrl = "/peopleImages/man5/man5_photo.png" , PhotoUrl = "/peopleImages/man5/man5_profile.png" },
                    new { FirstName = "Sophia", LastName = "Martin", Email = "SophiaMartin@example.com", Role = UserRole.VerifiedUser, VerificationStatus = UserVerificationStatus.Verified, DateOfBirth = new DateTime(1987, 3, 15), Street = "Konigsallee 5", City = "Dusseldorf", State = "North Rhine-Westphalia", PostalCode = "40212", Country = "Germany", PhoneNumber = "1234567898", PhotoIdUrl = "/peopleImages/woman5/woman5_id.png", FaceIdUrl = "/peopleImages/woman5/woman5_photo.png" , PhotoUrl = "/peopleImages/woman5/woman5_profile.png"},
                    new { FirstName = "Michael", LastName = "Davis", Email = "MichaelDavis@example.com", Role = UserRole.PendingVerification, VerificationStatus = UserVerificationStatus.PendingVerification, DateOfBirth = new DateTime(1992, 4, 20), Street = "123 Collins Street", City = "Melbourne", State = "Victoria", PostalCode = "3000", Country = "Australia", PhoneNumber = "1234567899", PhotoIdUrl = "/peopleImages/man2/man2_id.png", FaceIdUrl = "/peopleImages/man2/man2_photo.png" , PhotoUrl = "/peopleImages/man2/man2_profile.png" },
                    new { FirstName = "Emma", LastName = "Miller", Email = "EmmaMiller@example.com", Role = UserRole.PendingVerification, VerificationStatus = UserVerificationStatus.PendingVerification, DateOfBirth = new DateTime(1986, 6, 30), Street = "Rua Augusta 2385", City = "Sao Paulo", State = "Sao Paulo", PostalCode = "10011", Country = "Brazil", PhoneNumber = "1234567900", PhotoIdUrl = "/peopleImages/woman1/woman1_id.png", FaceIdUrl = "/peopleImages/woman1/woman1_photo.png" , PhotoUrl = "/peopleImages/woman1/woman1_profile.png" }
                };
                int counter = 0;
                var adminUser = new User
                {
                    Username = "admin",
                    Password = PasswordHasher.HashPassword("admin"),
                    Email = "admin@example.com",
                    Role = UserRole.Admin,
                    VerificationStatus = UserVerificationStatus.Verified,
                    PhotoIdUrl = "",
                    FaceIdUrl = "",
                    UserProfile = new UserProfile
                    {
                        FirstName = "Admin",
                        LastName = "Admin",
                        DateOfBirth = new DateTime(1992, 4, 20),
                        Street = "Admin Street",
                        City = "Admin City",
                        State = "Admin State",
                        PostalCode = "10001",
                        Country = "Admin Country",
                        PhoneNumber = "1234567890",
                        PhotoUrl = "",
                    }
                };
                context.Users.Add(adminUser);

                foreach (var user in userList)
                {
                    string username = GenerateUsername(user.FirstName, user.LastName, user.DateOfBirth, counter);

                    var newUser = new User
                    {
                        Username = username,
                        Password = PasswordHasher.HashPassword("password"),
                        Email = user.Email,
                        Role = user.Role,
                        VerificationStatus = user.VerificationStatus,
                        PhotoIdUrl = user.PhotoIdUrl,
                        FaceIdUrl = user.FaceIdUrl,
                        UserProfile = new UserProfile
                        {
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            DateOfBirth = user.DateOfBirth,
                            Street = user.Street,
                            City = user.City,
                            State = user.State,
                            PostalCode = user.PostalCode,
                            Country = user.Country,
                            PhoneNumber = user.PhoneNumber,
                            PhotoUrl = user.PhotoUrl,
                        }
                    };

                    context.Users.Add(newUser);
                    counter = (counter + 1) % 3;
                }

                context.SaveChanges();
            }


            if (!context.RealCards.Any())
            {
                var realCards = new List<RealCard>
                    {
                        // Visa cards
                        new RealCard { CardHolderName = "Carole Raynor", CardNumber = "5252295372344564", Issuer = "mastercard", ExpirationDate = DateTime.ParseExact("03/26", "MM/yy", null), Cvv = "566", Balance = 10000, CardType = CardType.Debit, Currency = CurrencyType.EUR, PaymentProcessorToken = "f1a2b3c4d5e6f7g8h9i0j1k2l3m4n5o6" },
                        new RealCard { CardHolderName = "David Little-Quigley", CardNumber = "5561648452405182", Issuer = "mastercard", ExpirationDate = DateTime.ParseExact("12/25", "MM/yy", null), Cvv = "163", Balance = 10000, CardType = CardType.Credit,Currency = CurrencyType.BGN, PaymentProcessorToken = "p6q7r8s9t0u1v2w3x4y5z6a7b8c9d0e1" },
                        new RealCard { CardHolderName = "Carroll Hauck", CardNumber = "5465592714992412", Issuer = "mastercard", ExpirationDate = DateTime.ParseExact("03/27", "MM/yy", null), Cvv = "615", Balance = 10000, CardType = CardType.Debit, Currency = CurrencyType.USD,PaymentProcessorToken = "f7g8h9i0j1k2l3m4n5o6p1a2b3c4d5e6" },
                        new RealCard { CardHolderName = "Olga Bayer-Ziemann", CardNumber = "5538650662283458", Issuer = "mastercard", ExpirationDate = DateTime.ParseExact("07/29", "MM/yy", null), Cvv = "952", Balance = 10000, CardType = CardType.Credit,Currency = CurrencyType.BGN, PaymentProcessorToken = "q9r0s1t2u3v4w5x6y7z8a9b0c1d2e3f4" },
                        new RealCard { CardHolderName = "Percy Conn", CardNumber = "5265373215659145", Issuer = "mastercard", ExpirationDate = DateTime.ParseExact("07/26", "MM/yy", null), Cvv = "168", Balance = 10000, CardType = CardType.Debit,Currency = CurrencyType.USD, PaymentProcessorToken = "k5l6m7n8o9p0q1r2s3t4u5v6w7x8y9z0" },
                        // Mastercard cards
                        new RealCard { CardHolderName = "Miss Mandy Orn", CardNumber = "4194143353329267", Issuer = "visa", ExpirationDate = DateTime.ParseExact("12/29", "MM/yy", null), Cvv = "948", Balance = 10000, CardType = CardType.Credit, Currency = CurrencyType.EUR,PaymentProcessorToken = "a1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6" },
                        new RealCard { CardHolderName = "Pearl Wolf-Corwin", CardNumber = "4551531347402371", Issuer = "visa", ExpirationDate = DateTime.ParseExact("07/28", "MM/yy", null), Cvv = "874", Balance = 10000, CardType = CardType.Debit, Currency = CurrencyType.BGN,PaymentProcessorToken = "g7h8i9j0k1l2m3n4o5p6q7r8s9t0u1v2" },
                        new RealCard { CardHolderName = "Toni Legros Sr.", CardNumber = "4823639117708018", Issuer = "visa", ExpirationDate = DateTime.ParseExact("02/28", "MM/yy", null), Cvv = "937", Balance = 10000, CardType = CardType.Credit,Currency = CurrencyType.USD, PaymentProcessorToken = "d5e6f7g8h9i0j1k2l3m4n5o6p7q8r9s0" },
                        new RealCard { CardHolderName = "Jacob Jerde", CardNumber = "4937424799514692", Issuer = "visa", ExpirationDate = DateTime.ParseExact("02/27", "MM/yy", null), Cvv = "050", Balance = 10000, CardType = CardType.Debit, Currency = CurrencyType.BGN,PaymentProcessorToken = "r3s4t5u6v7w8x9y0z1a2b3c4d5e6f7g8" },
                        new RealCard { CardHolderName = "Malcolm Ortiz", CardNumber = "4304178372051564", Issuer = "visa", ExpirationDate = DateTime.ParseExact("01/27", "MM/yy", null), Cvv = "105", Balance = 10000, CardType = CardType.Credit, Currency = CurrencyType.EUR,PaymentProcessorToken = "m8n9o0p1q2r3s4t5u6v7w8x9y0z1a2b3" },
                        // New Discover cards
                        new RealCard { CardHolderName = "Regina Botsford", CardNumber = "6537718804614275", Issuer = "discover", ExpirationDate = DateTime.ParseExact("12/26", "MM/yy", null), Cvv = "667", Balance = 10000, CardType = CardType.Credit, Currency = CurrencyType.USD, PaymentProcessorToken = "e1f2g3h4i5j6k7l8m9n0o1p2q3r4s5t6" },
                        new RealCard { CardHolderName = "Bert Friesen", CardNumber = "65386268514290461310", Issuer = "discover", ExpirationDate = DateTime.ParseExact("03/29", "MM/yy", null), Cvv = "683", Balance = 10000, CardType = CardType.Debit, Currency = CurrencyType.EUR, PaymentProcessorToken = "u7v8w9x0y1z2a3b4c5d6e7f8g9h0i1j2" },
                        new RealCard { CardHolderName = "Terry Carter", CardNumber = "60116240109505134304", Issuer = "discover", ExpirationDate = DateTime.ParseExact("08/29", "MM/yy", null), Cvv = "735", Balance = 10000, CardType = CardType.Credit, Currency = CurrencyType.BGN, PaymentProcessorToken = "k1l2m3n4o5p6q7r8s9t0u1v2w3x4y5z6" },
                        new RealCard { CardHolderName = "Homer West", CardNumber = "6011966457099428", Issuer = "discover", ExpirationDate = DateTime.ParseExact("06/28", "MM/yy", null), Cvv = "749", Balance = 10000, CardType = CardType.Debit, Currency = CurrencyType.USD, PaymentProcessorToken = "r1s2t3u4v5w6x7y8z9a0b1c2d3e4f5g6" },
                        new RealCard { CardHolderName = "Terry Olson", CardNumber = "6011165558224585", Issuer = "discover", ExpirationDate = DateTime.ParseExact("03/25", "MM/yy", null), Cvv = "753", Balance = 10000, CardType = CardType.Credit, Currency = CurrencyType.EUR, PaymentProcessorToken = "h1i2j3k4l5m6n7o8p9q0r1s2t3u4v5w6" },
                        // New JCB cards
                        new RealCard { CardHolderName = "Gordon MacGyver", CardNumber = "3528248702933773", Issuer = "jcb", ExpirationDate = DateTime.ParseExact("09/28", "MM/yy", null), Cvv = "141", Balance = 10000, CardType = CardType.Debit, Currency = CurrencyType.BGN, PaymentProcessorToken = "g1h2i3j4k5l6m7n8o9p0q1r2s3t4u5v6" },
                        new RealCard { CardHolderName = "Sophia Dietrich", CardNumber = "3529396800339739", Issuer = "jcb", ExpirationDate = DateTime.ParseExact("06/25", "MM/yy", null), Cvv = "537", Balance = 10000, CardType = CardType.Credit, Currency = CurrencyType.USD, PaymentProcessorToken = "x1y2z3a4b5c6d7e8f9g0h1i2j3k4l5m6" },
                        new RealCard { CardHolderName = "Jared Dicki-Jacobi", CardNumber = "3567766525203811", Issuer = "jcb", ExpirationDate = DateTime.ParseExact("08/28", "MM/yy", null), Cvv = "905", Balance = 10000, CardType = CardType.Debit, Currency = CurrencyType.EUR, PaymentProcessorToken = "n1o2p3q4r5s6t7u8v9w0x1y2z3a4b5c6" },
                        new RealCard { CardHolderName = "Rufus Adams", CardNumber = "3528202289202763", Issuer = "jcb", ExpirationDate = DateTime.ParseExact("04/26", "MM/yy", null), Cvv = "029", Balance = 10000, CardType = CardType.Credit, Currency = CurrencyType.BGN, PaymentProcessorToken = "p1q2r3s4t5u6v7w8x9y0z1a2b3c4d5e6" },
                        new RealCard { CardHolderName = "Antoinette Hermann DDS", CardNumber = "3529745379216985", Issuer = "jcb", ExpirationDate = DateTime.ParseExact("02/25", "MM/yy", null), Cvv = "661", Balance = 10000, CardType = CardType.Debit, Currency = CurrencyType.USD, PaymentProcessorToken = "l1m2n3o4p5q6r7s8t9u0v1w2x3y4z5a6" },
                        // New American Express cards
                        new RealCard { CardHolderName = "Eula Baumbach", CardNumber = "379986503859194", Issuer = "american_express", ExpirationDate = DateTime.ParseExact("02/28", "MM/yy", null), Cvv = "010", Balance = 10000, CardType = CardType.Credit, Currency = CurrencyType.EUR, PaymentProcessorToken = "o1p2q3r4s5t6u7v8w9x0y1z2a3b4c5d6" },
                        new RealCard { CardHolderName = "Erin Gibson", CardNumber = "346574765787434", Issuer = "american_express", ExpirationDate = DateTime.ParseExact("03/25", "MM/yy", null), Cvv = "379", Balance = 10000, CardType = CardType.Debit, Currency = CurrencyType.BGN, PaymentProcessorToken = "s1t2u3v4w5x6y7z8a9b0c1d2e3f4g5h6" },
                        new RealCard { CardHolderName = "Leona Ondricka", CardNumber = "347662360483753", Issuer = "american_express", ExpirationDate = DateTime.ParseExact("02/29", "MM/yy", null), Cvv = "279", Balance = 10000, CardType = CardType.Credit, Currency = CurrencyType.USD, PaymentProcessorToken = "u1v2w3x4y5z6a7b8c9d0e1f2g3h4i5j6" },
                        new RealCard { CardHolderName = "Marion Fritsch", CardNumber = "378195236673465", Issuer = "american_express", ExpirationDate = DateTime.ParseExact("09/28", "MM/yy", null), Cvv = "594", Balance = 10000, CardType = CardType.Debit, Currency = CurrencyType.BGN, PaymentProcessorToken = "y1z2a3b4c5d6e7f8g9h0i1j2k3l4m5n6" },
                        new RealCard { CardHolderName = "Al Lesch", CardNumber = "341073590763061", Issuer = "american_express", ExpirationDate = DateTime.ParseExact("11/27", "MM/yy", null), Cvv = "331", Balance = 10000, CardType = CardType.Credit, Currency = CurrencyType.EUR, PaymentProcessorToken = "a1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6" },
                        // New Maestro cards
                        new RealCard { CardHolderName = "Willie Gutkowski", CardNumber = "5018014971451687", Issuer = "maestro", ExpirationDate = DateTime.ParseExact("07/26", "MM/yy", null), Cvv = "621", Balance = 10000, CardType = CardType.Debit, Currency = CurrencyType.USD, PaymentProcessorToken = "z1a2b3c4d5e6f7g8h9i0j1k2l3m4n5o6" },
                        new RealCard { CardHolderName = "Ms. Dianna Emard", CardNumber = "6759985409640572", Issuer = "maestro", ExpirationDate = DateTime.ParseExact("06/26", "MM/yy", null), Cvv = "999", Balance = 10000, CardType = CardType.Credit, Currency = CurrencyType.EUR, PaymentProcessorToken = "c1d2e3f4g5h6i7j8k9l0m1n2o3p4q5r6" },
                        new RealCard { CardHolderName = "Doris Hintz DDS", CardNumber = "50386934318281141092", Issuer = "maestro", ExpirationDate = DateTime.ParseExact("06/25", "MM/yy", null), Cvv = "672", Balance = 10000, CardType = CardType.Debit, Currency = CurrencyType.BGN, PaymentProcessorToken = "m1n2o3p4q5r6s7t8u9v0w1x2y3z4a5b6" },
                        new RealCard { CardHolderName = "Celia Gleichner", CardNumber = "50387037772879110", Issuer = "maestro", ExpirationDate = DateTime.ParseExact("01/25", "MM/yy", null), Cvv = "461", Balance = 10000, CardType = CardType.Credit, Currency = CurrencyType.USD, PaymentProcessorToken = "e1f2g3h4i5j6k7l8m9n0o1p2q3r4s5t6" },
                        new RealCard { CardHolderName = "Lance Reinger", CardNumber = "58934126465224329", Issuer = "maestro", ExpirationDate = DateTime.ParseExact("02/27", "MM/yy", null), Cvv = "334", Balance = 10000, CardType = CardType.Debit, Currency = CurrencyType.EUR, PaymentProcessorToken = "q1r2s3t4u5v6w7x8y9z0a1b2c3d4e5f6" }
                    };


                context.RealCards.AddRange(realCards);
                context.SaveChanges();

            }
            if (!context.Wallets.Any())
            {

                var realCardTemplates = context.RealCards.ToList();
                var indicesList = Enumerable.Range(0, context.RealCards.Count()).ToList();
                var shuffledIndices = indicesList.OrderBy(x => _random.Next()).ToList();
                var indicesStack = new Stack<int>(shuffledIndices);

                foreach (var user in context.Users)
                {
                    for (int i = 0; i < 2 && indicesStack.Count > 0; i++)
                    {
                        var templateIndex = indicesStack.Pop();
                        var template = realCardTemplates[templateIndex];

                        var card = new Card
                        {
                            CardHolderName = $"{template.CardHolderName}",
                            CardNumber = $"{new string('*', 12)}{template.CardNumber.Substring(template.CardNumber.Length - 4)}",
                            Issuer = template.Issuer,
                            ExpirationDate = template.ExpirationDate,
                            Cvv = template.Cvv,
                            CardType = template.CardType,
                            Currency = template.Currency,
                            PaymentProcessorToken = template.PaymentProcessorToken,
                        };

                        user.Cards.Add(card);
                        var mainCard = user.Cards.First();
                        var wallet = new Wallet
                        {
                            Name = $"{user.Username}'s Main Wallet",
                            Balance = (decimal)(_random.NextDouble() * (1000 - 100) + 100),
                            Currency = mainCard.Currency,
                            UserId = user.Id,
                            WalletType = WalletType.Main

                        };

                        if (user.MainWallet == null)
                        {
                            user.MainWallet = wallet;
                        }
                        else
                        {
                            wallet.Name = $"{user.Username}'s Standart Wallet";
                            wallet.WalletType = WalletType.Standart;
                            wallet.Currency = GetRandomCurrency(mainCard.Currency);
                            user.Wallets.Add(wallet);
                        }
                    }



                }
                var realCards2 = new List<RealCard>
                    {
                        new RealCard { CardHolderName = "Fred Kunze", CardNumber = "5430204966607530", Issuer = "mastercard", ExpirationDate = DateTime.ParseExact("12/27", "MM/yy", null), Cvv = "198", Balance = 10000, CardType = CardType.Debit, Currency = CurrencyType.EUR, PaymentProcessorToken = "f1a2b3c4d5e6f7g822i0j1k2l3m4n5o6" },
                        new RealCard { CardHolderName = "Mildred Corwin", CardNumber = "5444839212087963", Issuer = "mastercard", ExpirationDate = DateTime.ParseExact("02/27", "MM/yy", null), Cvv = "057", Balance = 10000, CardType = CardType.Debit, Currency = CurrencyType.EUR, PaymentProcessorToken = "f1a2b3c4d5e6f733h9i0j1k2l3m4n5o6" },
                    };
                context.RealCards.AddRange(realCards2);
                context.SaveChanges();
            }

            if (context.Wallets.Any() && context.Cards.Any() && !context.CardTransactions.Any())
            {
                var wallets = context.Wallets.ToList();
                var cards = context.Cards.ToList();

                
                var walletTransactions = new List<WalletTransaction>();

                for (int i = 0; i < 50; i++) 
                {
                    var senderWallet = wallets[_random.Next(wallets.Count)];
                    var recipientWallet = wallets.Where(w => w.UserId != senderWallet.UserId).OrderBy(_ => _random.Next()).FirstOrDefault();

                    if (recipientWallet != null)
                    {
                        var amountSent = (decimal)(_random.NextDouble() * (500 - 10) + 10);
                        var amountReceived = amountSent;
                        var fee = amountSent * 0.02m;
                        if (recipientWallet.Currency == senderWallet.Currency)
                        {
                            fee = 0.00m;
                        }
                        

                        walletTransactions.Add(new WalletTransaction
                        {
                            AmountSent = amountSent,
                            AmountReceived = amountReceived - fee,
                            FeeAmount = fee,
                            CreatedAt = DateTime.UtcNow.AddDays(-_random.Next(0, 365)),
                            CurrencySent = senderWallet.Currency,
                            CurrencyReceived = recipientWallet.Currency,
                            Status = TransactionStatus.Completed,
                            SenderId = senderWallet.Id,
                            RecipientId = recipientWallet.Id,
                            VerificationCode = VerificationCode.Generate()
                        });
                    }
                }

                context.WalletTransactions.AddRange(walletTransactions);

                
                var cardTransactions = new List<CardTransaction>();

                for (int i = 0; i < 100; i++)
                {
                    var card = cards[_random.Next(cards.Count)];
                    var wallet = wallets.FirstOrDefault(w => w.UserId == card.User.Id);

                    if (wallet != null)
                    {
                        var amount = (decimal)(_random.NextDouble() * (300 - 20) + 20);
                        var fee = 0.00m;
                        var transactionType = (TransactionType)_random.Next(0, 2);
                        if (card.Currency != wallet.Currency && transactionType== TransactionType.Withdrawal)
                        {
                            fee = amount * 0.025m;
                            
                        }

                        cardTransactions.Add(new CardTransaction
                        {
                            Amount = amount,
                            CreatedAt = DateTime.UtcNow.AddDays(-_random.Next(0, 365)),
                            Currency = card.Currency,
                            Status = TransactionStatus.Completed,
                            UserId = card.User.Id,
                            WalletId = wallet.Id,
                            CardId = card.Id,
                            TransactionType = transactionType,
                            Fee = fee
                        });
                    }
                }

                context.CardTransactions.AddRange(cardTransactions);
                context.SaveChanges();


            }


        }


        private static string GenerateUsername(string firstName, string lastName, DateTime dateOfBirth, int num)
        {
            var options = new List<string>
            {
                $"{firstName}{lastName}{dateOfBirth.Year % 100}",
                $"{firstName}_{lastName}".ToLower(),
                $"{firstName}.{lastName}"
            };

            return options[num];
        }

        private static CurrencyType GetRandomCurrency(CurrencyType excludedCurrency)
        {
            var availableCurrencies = Enum.GetValues(typeof(CurrencyType)).Cast<CurrencyType>()
                                          .Where(c => c != excludedCurrency)
                                          .ToList();

            var randomCurrency = availableCurrencies[_random.Next(availableCurrencies.Count)];

            return randomCurrency;
        }

        private class VerificationCode
        {
            public static string Generate()
            {
                Random random = new Random();
                int verificationCode = random.Next(1000, 10000);
                return verificationCode.ToString();
            }
        }

    }
}

