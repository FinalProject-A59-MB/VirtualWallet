using VirtualWallet.DATA.Models;

namespace VirtualWallet.DATA.Services.Contracts
{
    public interface IUserWalletService
    {
        IEnumerable<UserWallet> GetUserWalletsByUserId(int userId);
        IEnumerable<UserWallet> GetUserWalletByWalletId(int walletId);
        void AddUserWallet(UserWallet userWallet);
        void RemoveUserWallet(int walletId);
    }
}
