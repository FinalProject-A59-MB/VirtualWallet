using ForumProject.Exceptions;
using System.Threading.Tasks;
using VirtualWallet.DATA.Models;
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

        public async Task<User> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) throw new EntityNotFoundException("User not found");
            return user;
        }

        public async Task<UserProfile> GetUserProfileAsync(int userId)
        {
            var userProfile = await _userRepository.GetUserProfileAsync(userId);
            if (userProfile == null) throw new EntityNotFoundException("User profile not found");
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
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) throw new EntityNotFoundException("User not found");
            await _userRepository.DeleteUserAsync(userId);
        }
    }
}
