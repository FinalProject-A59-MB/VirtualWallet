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

        public async Task<Result> AddFriendAsync(int userId, int contactId)
        {
            if (await _userRepository.IsContactExistsAsync(userId, contactId))
            {
                return Result.Failure("This user is already in your friend list.");
            }

            var userContact = new UserContact
            {
                UserId = userId,
                ContactId = contactId,
                AddedDate = DateTime.UtcNow
            };

            await _userRepository.AddContactAsync(userContact);

            return Result.Success();
        }

        public async Task<List<User>> GetFriendsAsync(int userId)
        {
            return await _userRepository.GetUserContactsAsync(userId);
        }

        public async Task<Result> RemoveFriendAsync(int userId, int contactId)
        {
            var userContact = await _userRepository.GetUserContactAsync(userId, contactId);
            if (userContact == null)
            {
                return Result.Failure("Contact not found in your friend list.");
            }

            await _userRepository.RemoveContactAsync(userContact);

            return Result.Success();
        }


    }
}
