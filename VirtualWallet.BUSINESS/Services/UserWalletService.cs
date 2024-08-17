using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Repositories.Contracts;
using VirtualWallet.DATA.Services.Contracts;

namespace VirtualWallet.DATA.Services
{
    public class UserWalletService : IUserWalletService
    {
        private readonly IUserWalletRepository _userWalletRepository;

        public UserWalletService(IUserWalletRepository userWalletRepository)
        {
            _userWalletRepository = userWalletRepository;
        }


        public Task AddUserWalletAsync(UserWallet userWallet)
        {
             return _userWalletRepository.AddUserWalletAsync(userWallet);
        }

        public Task<IEnumerable<UserWallet>> GetUserWalletsByUserIdAsync(int userId)
        {
            return _userWalletRepository.GetUserWalletsByUserIdAsync(userId);
        }

        public Task<IEnumerable<UserWallet>> GetUserWalletByWalletIdAsync(int walletId)
        {
            return _userWalletRepository.GetUserWalletByWalletIdAsync(walletId);
        }

        public Task RemoveUserWalletAsync(int walletId)
        {
            return _userWalletRepository.RemoveUserWalletAsync(walletId);
        }
    }
}
