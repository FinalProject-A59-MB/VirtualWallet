using VirtualWallet.BUSINESS.Exceptions;
using System.Threading.Tasks;
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

        public async Task<User> RegisterUserAsync(User userToRegister)
        {
            var username = userToRegister.Username;
            var email = userToRegister.Email;
            var password = userToRegister.Password;

            var existingUser = await _userRepository.GetUserByUsernameAsync(username);
            if (existingUser != null)
            {
                throw new DuplicateEntityException("Username already exists.");
            }

            existingUser = await _userRepository.GetUserByEmailAsync(email);
            if (existingUser != null)
            {
                throw new DuplicateEntityException("Email already exists.");
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

            return user;
        }


        public async Task<IQueryable<User>> GetUsers()
        {
            var users = _userRepository.GetAllUsers();
            return users;
        }


        public async Task<User> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId) ??
                throw new EntityNotFoundException("User not found");
            return user;
        }

        public async Task<User> GetUserByUsernameAsync(string userName)
        {
            var user = await _userRepository.GetUserByUsernameAsync(userName) ??
                throw new EntityNotFoundException("User not found"); ;
            return user;
        }

        public async Task<UserProfile> GetUserProfileAsync(int userId)
        {
            var userProfile = await _userRepository.GetUserProfileAsync(userId) ??
                throw new EntityNotFoundException("User profile not found");
            return userProfile;
        }

        public async Task UpdateUserAsync(User user)
        {
            await _userRepository.UpdateUserAsync(user);
        }

        public async Task UpdateUserProfileAsync(UserProfile userProfile)
        {
            await _userRepository.UpdateUserProfileAsync(userProfile);
        }

        public async Task DeleteUserAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId)??
                throw new EntityNotFoundException("User not found");
            await _userRepository.DeleteUserAsync(userId);
        }
    }
}
