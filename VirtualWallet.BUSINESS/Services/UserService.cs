using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VirtualWallet.BUSINESS.Helpers;
using VirtualWallet.BUSINESS.Results;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Helpers;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.DATA.Repositories.Contracts;
using VirtualWallet.DATA.Services.Contracts;

namespace VirtualWallet.DATA.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrencyService _currencyService;

        public UserService(
            IUserRepository userRepository,
            ICurrencyService currencyService
            )
        {
            _userRepository = userRepository;
            _currencyService = currencyService;
        }

        public async Task<Result<User>> RegisterUserAsync(User userToRegister)
        {
            string username = userToRegister.Username;
            string email = userToRegister.Email;
            string password = userToRegister.Password;

            Result emailValidationResult = EmailValidator.Validate(email);
            if (!emailValidationResult.IsSuccess)
            {
                return Result<User>.Failure(emailValidationResult.Error);
            }

            Result passwordValidationResult = PasswordValidator.Validate(password);
            if (!passwordValidationResult.IsSuccess)
            {
                return Result<User>.Failure(passwordValidationResult.Error);
            }

            User existingUser = await _userRepository.GetUserByUsernameAsync(username);
            if (existingUser != null)
            {
                return Result<User>.Failure("Username already exists.");
            }

            existingUser = await _userRepository.GetUserByEmailAsync(email);
            if (existingUser != null)
            {
                return Result<User>.Failure("Email already exists.");
            }

            userToRegister.Password = PasswordHasher.HashPassword(password);
            userToRegister.Role = UserRole.RegisteredUser;
            userToRegister.VerificationStatus = UserVerificationStatus.NotVerified;

            await _userRepository.AddUserAsync(userToRegister);

            UserProfile userProfile = new UserProfile
            {
                UserId = userToRegister.Id,
                FirstName = "",
                LastName = "",
            };
            await _userRepository.AddUserProfileAsync(userProfile);

            userToRegister.UserProfile = userProfile;

            return Result<User>.Success(userToRegister);
        }


        public async Task<Result<IEnumerable<User>>> GetUsers()
        {
            IEnumerable<User> users = await _userRepository.GetAllUsers();
            return Result<IEnumerable<User>>.Success(users);
        }

        public async Task<Result<User>> GetUserByIdAsync(int userId)
        {
            User user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return Result<User>.Failure("User not found.");
            }
            return Result<User>.Success(user);
        }

        public async Task<Result<User>> GetUserByUsernameAsync(string userName)
        {
            User user = await _userRepository.GetUserByUsernameAsync(userName);
            if (user == null)
            {
                return Result<User>.Failure("User not found.");
            }
            return Result<User>.Success(user);
        }

        public async Task<Result<User>> GetUserByEmailAsync(string userName)
        {
            User user = await _userRepository.GetUserByEmailAsync(userName);
            if (user == null)
            {
                return Result<User>.Failure("User not found.");
            }
            return Result<User>.Success(user);
        }

        public async Task<Result<UserProfile>> GetUserProfileAsync(int userId)
        {
            UserProfile userProfile = await _userRepository.GetUserProfileAsync(userId);
            if (userProfile == null)
            {
                return Result<UserProfile>.Failure("User profile not found.");
            }
            return Result<UserProfile>.Success(userProfile);
        }

        public async Task<Result> UpdateUserAsync(User user)
        {
            await _userRepository.UpdateUserAsync(user);
            return Result.Success();
        }

        public async Task<Result> UpdateUserProfileAsync(UserProfile userProfile)
        {
            await _userRepository.UpdateUserProfileAsync(userProfile);
            return Result.Success();
        }

        public async Task<Result> DeleteUserAsync(int userId)
        {
            User user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return Result.Failure("User not found.");
            }

            await _userRepository.DeleteUserAsync(userId);
            return Result.Success();
        }

        public async Task<Result> SendFriendRequestAsync(int userId, int contactId)
        {
            UserContact existingRequest = await _userRepository.GetUserContactAsync(userId, contactId);

            if (existingRequest != null)
            {
                return Result.Failure("Friend request already sent.");
            }

            UserContact senderContact = new UserContact
            {
                UserId = userId,
                ContactId = contactId,
                SenderId = userId,
                AddedDate = DateTime.UtcNow,
                Status = FriendRequestStatus.Pending
            };

            UserContact receiverContact = new UserContact
            {
                UserId = contactId,
                ContactId = userId,
                SenderId = userId,
                AddedDate = DateTime.UtcNow,
                Status = FriendRequestStatus.Pending
            };

            await _userRepository.AddContactAsync(senderContact);
            await _userRepository.AddContactAsync(receiverContact);

            return Result.Success();
        }

        public async Task<Result> AcceptFriendRequestAsync(int userId, int contactId)
        {
            UserContact friendRequest = await _userRepository.GetUserContactAsync(contactId, userId);

            if (friendRequest == null || friendRequest.Status != FriendRequestStatus.Pending)
            {
                return Result.Failure("Friend request not found.");
            }

            friendRequest.Status = FriendRequestStatus.Accepted;
            await _userRepository.UpdateContactAsync(friendRequest);

            UserContact reciprocalFriendRequest = await _userRepository.GetUserContactAsync(userId, contactId);

            if (reciprocalFriendRequest == null)
            {
                reciprocalFriendRequest = new UserContact
                {
                    UserId = userId,
                    ContactId = contactId,
                    SenderId = contactId,
                    AddedDate = DateTime.UtcNow,
                    Status = FriendRequestStatus.Accepted
                };

                await _userRepository.AddContactAsync(reciprocalFriendRequest);
            }
            else
            {
                reciprocalFriendRequest.Status = FriendRequestStatus.Accepted;
                await _userRepository.UpdateContactAsync(reciprocalFriendRequest);
            }

            return Result.Success();
        }


        public async Task<Result<IEnumerable<UserContact>>> GetPendingFriendRequestsAsync(int userId)
        {
            IEnumerable<UserContact> requests = await _userRepository.GetPendingFriendRequestsAsync(userId);
            return Result<IEnumerable<UserContact>>.Success(requests);
        }

        public async Task<Result<IEnumerable<User>>> GetFriendsAsync(int userId)
        {
            List<User> friends = await _userRepository.GetUserContactsAsync(userId);
            return Result<IEnumerable<User>>.Success(friends);
        }

        public async Task<Result> DenyFriendRequestAsync(int userId, int contactId)
        {
            UserContact friendRequest = await _userRepository.GetUserContactAsync(contactId, userId);

            if (friendRequest == null || friendRequest.Status != FriendRequestStatus.Pending)
            {
                return Result.Failure("Friend request not found.");
            }

            await _userRepository.RemoveContactAsync(friendRequest);

            return Result.Success();
        }

        public async Task<Result<IEnumerable<User>>> SearchUsersAsync(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return Result<IEnumerable<User>>.Failure("Search term cannot be empty.");
            }

            IEnumerable<User> users = await _userRepository.SearchUsersAsync(searchTerm);

            if (users == null || !users.Any())
            {
                return Result<IEnumerable<User>>.Failure("No users found matching the search criteria.");
            }

            return Result<IEnumerable<User>>.Success(users);
        }


        public async Task<Result> UpdateContact(int userId, int contactId, string description)
        {
            UserContact contact = await _userRepository.GetUserContactAsync(userId, contactId);

            if (contact == null)
            {
                return Result.Failure("Contact not found.");
            }

            contact.Description = description;
            await _userRepository.UpdateContactAsync(contact);

            return Result.Success();
        }

        public async Task<Result> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            Result<User> userResult = await GetUserByIdAsync(userId);
            if (!userResult.IsSuccess)
            {
                return Result.Failure("User not found.");
            }

            User user = userResult.Value;

            if (!PasswordHasher.VerifyPassword(currentPassword, user.Password))
            {
                return Result.Failure("Current password is incorrect.");
            }

            string hashedNewPassword = PasswordHasher.HashPassword(newPassword);

            user.Password = hashedNewPassword;
            await _userRepository.UpdateUserAsync(user);

            return Result.Success();
        }


        public async Task<Result> ChangeEmailAsync(int userId, string newEmail, string currentPassword)
        {
            Result<User> userResult = await GetUserByIdAsync(userId);
            if (!userResult.IsSuccess)
            {
                return Result.Failure("User not found.");
            }

            User user = userResult.Value;

            if (!PasswordHasher.VerifyPassword(currentPassword, user.Password))
            {
                return Result.Failure("Current password is incorrect.");
            }

            User existingUser = await _userRepository.GetUserByEmailAsync(newEmail);
            if (existingUser != null && existingUser.Id != userId)
            {
                return Result.Failure("Email is already in use by another account.");
            }

            user.Email = newEmail;
            await _userRepository.UpdateUserAsync(user);

            return Result.Success();
        }

        public async Task<Result<IEnumerable<User>>> FilterUsersAsync(UserQueryParameters parameters)
        {
            IEnumerable<User> query = await _userRepository.GetAllUsers();

            if (!string.IsNullOrEmpty(parameters.Username))
            {
                query = query.Where(u => u.Username.Contains(parameters.Username));
            }

            if (!string.IsNullOrEmpty(parameters.Email))
            {
                query = query.Where(u => u.Email.Contains(parameters.Email));
            }

            if (!string.IsNullOrEmpty(parameters.PhoneNumber))
            {
                query = query.Where(u => u.UserProfile.PhoneNumber.Contains(parameters.PhoneNumber));
            }

            if (parameters.VerificationStatus != 0)
            {
                query = query.Where(u => u.VerificationStatus == parameters.VerificationStatus);
            }

            if (parameters.Role != 0)
            {
                query = query.Where(u => u.Role == parameters.Role);
            }

            int skip = (parameters.PageNumber - 1) * parameters.PageSize;
            query = query.Skip(skip).Take(parameters.PageSize);

            IEnumerable<User> users = query;

            return users.Any()
                ? Result<IEnumerable<User>>.Success(users)
                : Result<IEnumerable<User>>.Failure("No users found.");
        }


        public async Task<Result<int>> GetTotalUserCountAsync(UserQueryParameters parameters)
        {
            IEnumerable<User> query = await _userRepository.GetAllUsers();

            if (!string.IsNullOrEmpty(parameters.Username))
            {
                query = query.Where(u => u.Username.Contains(parameters.Username));
            }

            if (!string.IsNullOrEmpty(parameters.Email))
            {
                query = query.Where(u => u.Email.Contains(parameters.Email));
            }

            if (!string.IsNullOrEmpty(parameters.PhoneNumber))
            {
                query = query.Where(u => u.UserProfile.PhoneNumber.Contains(parameters.PhoneNumber));
            }

            int count =  query.Count();

            return count != 0
                ? Result<int>.Success(count)
                : Result<int>.Failure("No Users found.");
        }

        public async Task<Result<(decimal TotalAmount, CurrencyType Currency)>> GetTotalBalanceInMainWalletCurrencyAsync(int userId)
        {
            Result<User> userResult = await GetUserByIdAsync(userId);
            if (!userResult.IsSuccess)
            {
                return Result<(decimal TotalAmount, CurrencyType Currency)>.Failure("User not found.");
            }

            User user = userResult.Value;

            if (user.MainWallet == null)
            {
                return Result<(decimal TotalAmount, CurrencyType Currency)>.Failure("User does not have a main wallet set.");
            }

            CurrencyType mainWalletCurrency = user.MainWallet.Currency;
            decimal totalAmount = 0m;

            foreach (var wallet in user.Wallets)
            {
                if (wallet.Currency == mainWalletCurrency)
                {
                    totalAmount += wallet.Balance;
                }
                else
                {
                    var conversionResult = await _currencyService.GetRatesForCurrencyAsync(wallet.Currency);
                    if (!conversionResult.IsSuccess)
                    {
                        return Result<(decimal TotalAmount, CurrencyType Currency)>.Failure(conversionResult.Error);
                    }

                    Dictionary<string,decimal> conversionRates = conversionResult.Value.Data;
                    if (!conversionRates.TryGetValue(mainWalletCurrency.ToString(), out var rate))
                    {
                        return Result<(decimal TotalAmount, CurrencyType Currency)>.Failure($"Conversion rate for {mainWalletCurrency} not found.");
                    }

                    totalAmount += wallet.Balance * rate;
                }
            }

            return Result<(decimal TotalAmount, CurrencyType Currency)>.Success((totalAmount, mainWalletCurrency));
        }


    }


}
