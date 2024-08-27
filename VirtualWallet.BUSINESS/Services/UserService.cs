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

        public async Task<Result<IQueryable<User>>> GetUsers()
        {
            var users = _userRepository.GetAllUsers();
            return Result<IQueryable<User>>.Success(users);
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
                return Result.Failure("Friend request already sent or you are already friends.");
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
            // Fetch the original friend request
            var friendRequest = await _userRepository.GetUserContactAsync(contactId, userId);

            if (friendRequest == null || friendRequest.Status != FriendRequestStatus.Pending)
            {
                return Result.Failure("Friend request not found.");
            }

            // Mark the original friend request as accepted
            friendRequest.Status = FriendRequestStatus.Accepted;
            await _userRepository.UpdateContactAsync(friendRequest);

            // Check if the reciprocal friend request already exists
            var reciprocalFriendRequest = await _userRepository.GetUserContactAsync(userId, contactId);

            if (reciprocalFriendRequest == null)
            {
                // If it doesn't exist, create a new reciprocal friend request
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
                // If it does exist, update its status to accepted
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




    }
}
