using VirtualWallet.DATA.Models;

namespace VirtualWallet.DATA.Repositories.Contracts
{
    public interface IUserWalletRepository
    {
        Task<IEnumerable<UserWallet>> GetUserWalletsByUserIdAsync(int userId);
        Task<IEnumerable<UserWallet>> GetUserWalletByWalletIdAsync(int walletId);
        Task AddUserWalletAsync(UserWallet userWallet);
        Task RemoveUserWalletAsync(int walletId);
    }
}
