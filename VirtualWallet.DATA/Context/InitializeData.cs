using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            if (context.Users.Any())
            {
                return; 
            }

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
                    PhotoUrl = "test",
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
                    Role = i % 2 == 0 ? UserRole.ActiveUser : UserRole.ActiveUser,
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
                        PhotoUrl = "test",
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
                    Role = i % 2 == 0 ? UserRole.ActiveUser : UserRole.RegisteredUser,
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
                        PhotoUrl = "test",
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
                    Role = i % 2 == 0 ? UserRole.ActiveUser : UserRole.RegisteredUser,
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
                        PhotoUrl = "test",
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
                    Role = i % 2 == 0 ? UserRole.ActiveUser : UserRole.RegisteredUser,
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
                        PhotoUrl = "test",
                    }
                };
                users.Add(user);
            }

            context.Users.AddRange(users);
            context.SaveChanges();
        }
    }
}
