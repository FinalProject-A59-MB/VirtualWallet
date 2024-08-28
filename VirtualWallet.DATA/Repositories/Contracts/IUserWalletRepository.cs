using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Models.Enums;

namespace VirtualWallet.DATA.Repositories.Contracts
{
    public interface IUserWalletRepository
    {
        Task<IEnumerable<UserWallet>> GetUserWalletsByUserIdAsync(int userId);
        Task<IEnumerable<UserWallet>> GetUserWalletByWalletIdAsync(int walletId);
        Task<int> AddUserWalletAsync(int walletId, int userId, UserWalletRole role);
        Task RemoveUserWalletAsync(int walletId, int userId);
    }
}
