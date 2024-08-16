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
        public void AddUserWallet(UserWallet userWallet)
        {
            _userWalletRepository.AddUserWallet(userWallet);
        }

        public IEnumerable<UserWallet> GetUserWalletsByUserId(int userId)
        {
            return _userWalletRepository.GetUserWalletsByUserId(userId);
        }

        public IEnumerable<UserWallet> GetUserWalletsByWalletId(int walletId)
        {
            return _userWalletRepository.GetUserWalletsByWalletId(walletId);
        }

        public void RemoveUserWallet(int walletId)
        {
            _userWalletRepository.RemoveUserWallet(walletId);
        }
    }
}
