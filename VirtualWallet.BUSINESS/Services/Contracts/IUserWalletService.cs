using VirtualWallet.BUSINESS.Results;
using VirtualWallet.DATA.Models;

namespace VirtualWallet.DATA.Services.Contracts
{
    public interface IUserWalletService
    {
        Task<Result<IEnumerable<UserWallet>>> GetUserWalletsByUserIdAsync(int userId);
        Task<Result<IEnumerable<UserWallet>>> GetUserWalletByWalletIdAsync(int walletId);
        Task<Result> AddUserWalletAsync(int walletId, int userId);
        Task<Result> RemoveUserWalletAsync(int walletId, int userId);
    }
}
