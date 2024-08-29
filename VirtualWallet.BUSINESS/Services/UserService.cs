using Microsoft.EntityFrameworkCore;
using VirtualWallet.BUSINESS.Results;
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

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<User>> RegisterUserAsync(User userToRegister)
        {
            var username = userToRegister.Username;
            var email = userToRegister.Email;
            var password = userToRegister.Password;

            var existingUser = await _userRepository.GetUserByUsernameAsync(username);
            if (existingUser != null)
            {
                return Result<User>.Failure("Username already exists.");
            }

            existingUser = await _userRepository.GetUserByEmailAsync(email);
            if (existingUser != null)
            {
                return Result<User>.Failure("Email already exists.");
            }

            var hashedPassword = PasswordHasher.HashPassword(password);

            var user = new User
            {
                Username = username,
                Password = hashedPassword,
                Email = email,
                Role = UserRole.RegisteredUser,
                VerificationStatus = UserVerificationStatus.NotVerified
            };

            await _userRepository.AddUserAsync(user);

            var userProfile = new UserProfile
            {
                UserId = user.Id,
                FirstName = "",
                LastName = "",
            };

            await _userRepository.AddUserProfileAsync(userProfile);

            return Result<User>.Success(user);
        }

        public async Task<Result<IEnumerable<User>>> GetUsers()
        {
            var users = await _userRepository.GetAllUsers();
            return Result<IEnumerable<User>>.Success(users);
        }

        public async Task<Result<User>> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return Result<User>.Failure("User not found.");
            }
            return Result<User>.Success(user);
        }

        public async Task<Result<User>> GetUserByUsernameAsync(string userName)
        {
            var user = await _userRepository.GetUserByUsernameAsync(userName);
            if (user == null)
            {
                return Result<User>.Failure("User not found.");
            }
            return Result<User>.Success(user);
        }

        public async Task<Result<User>> GetUserByEmailAsync(string userName)
        {
            var user = await _userRepository.GetUserByEmailAsync(userName);
            if (user == null)
            {
                return Result<User>.Failure("User not found.");
            }
            return Result<User>.Success(user);
        }

        public async Task<Result<UserProfile>> GetUserProfileAsync(int userId)
        {
            var userProfile = await _userRepository.GetUserProfileAsync(userId);
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
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return Result.Failure("User not found.");
            }

            await _userRepository.DeleteUserAsync(userId);
            return Result.Success();
        }

        public async Task<Result> SendFriendRequestAsync(int userId, int contactId)
        {
            var existingRequest = await _userRepository.GetUserContactAsync(userId, contactId);

            if (existingRequest != null)
            {
                return Result.Failure("Friend request already sent.");
            }

            
            var senderContact = new UserContact
            {
                UserId = userId,
                ContactId = contactId,
                SenderId = userId,
                AddedDate = DateTime.UtcNow,
                Status = FriendRequestStatus.Pending
            };

            
            var receiverContact = new UserContact
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
            var friendRequest = await _userRepository.GetUserContactAsync(contactId, userId);

            if (friendRequest == null || friendRequest.Status != FriendRequestStatus.Pending)
            {
                return Result.Failure("Friend request not found.");
            }

            friendRequest.Status = FriendRequestStatus.Accepted;
            await _userRepository.UpdateContactAsync(friendRequest);

            var reciprocalFriendRequest = await _userRepository.GetUserContactAsync(userId, contactId);

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
            var requests = await _userRepository.GetPendingFriendRequestsAsync(userId);
            return Result<IEnumerable<UserContact>>.Success(requests);
        }

        public async Task<Result<IEnumerable<User>>> GetFriendsAsync(int userId)
        {
            var friends = await _userRepository.GetUserContactsAsync(userId);
            return Result<IEnumerable<User>>.Success(friends);
        }

        public async Task<Result> DenyFriendRequestAsync(int userId, int contactId)
        {
            var friendRequest = await _userRepository.GetUserContactAsync(contactId, userId);

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

            var users = await _userRepository.SearchUsersAsync(searchTerm);

            if (users == null || !users.Any())
            {
                return Result<IEnumerable<User>>.Failure("No users found matching the search criteria.");
            }

            return Result<IEnumerable<User>>.Success(users);
        }


        public async Task<Result> UpdateContact(int userId, int contactId, string description)
        {
            var contact = await _userRepository.GetUserContactAsync(userId, contactId);

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
            var userResult = await GetUserByIdAsync(userId);
            if (!userResult.IsSuccess)
            {
                return Result.Failure("User not found.");
            }

            var user = userResult.Value;

            if (!PasswordHasher.VerifyPassword(currentPassword, user.Password))
            {
                return Result.Failure("Current password is incorrect.");
            }

            var hashedNewPassword = PasswordHasher.HashPassword(newPassword);

            user.Password = hashedNewPassword;
            await _userRepository.UpdateUserAsync(user);

            return Result.Success();
        }


        public async Task<Result> ChangeEmailAsync(int userId, string newEmail, string currentPassword)
        {
            var userResult = await GetUserByIdAsync(userId);
            if (!userResult.IsSuccess)
            {
                return Result.Failure("User not found.");
            }

            var user = userResult.Value;

            if (!PasswordHasher.VerifyPassword(currentPassword, user.Password))
            {
                return Result.Failure("Current password is incorrect.");
            }

            var existingUser = await _userRepository.GetUserByEmailAsync(newEmail);
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
            var query = await _userRepository.GetAllUsers();

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

            var skip = (parameters.PageNumber - 1) * parameters.PageSize;
            query = query.Skip(skip).Take(parameters.PageSize);

            var users = query;

            return users.Any()
                ? Result<IEnumerable<User>>.Success(users)
                : Result<IEnumerable<User>>.Failure("No users found.");
        }


        public async Task<Result<int>> GetTotalUserCountAsync(UserQueryParameters parameters)
        {
            var query = await _userRepository.GetAllUsers();

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

            var count =  query.Count();

            return count != 0
                ? Result<int>.Success(count)
                : Result<int>.Failure("No Users found.");
        }
    }


}
