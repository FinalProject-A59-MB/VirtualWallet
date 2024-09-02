using System.Threading.Tasks;
using VirtualWallet.BUSINESS.Results;
using VirtualWallet.DATA.Models;

namespace VirtualWallet.DATA.Services.Contracts
{
    public interface IWalletService
    {
        Task<Result<Wallet>> GetWalletByIdAsync(int id);
        Task<Result<IEnumerable<Wallet>>> GetWalletsByUserIdAsync(int userId);
        Task<Result<Wallet>> GetWalletByNameAsync(string walletName);
        Task<Result<int>> AddWalletAsync(Wallet wallet);
        Task<Result> UpdateWalletAsync(Wallet wallet);
        Task<Result> RemoveWalletAsync(int walletId);
        Task<Result<int>> GetWalletIdByUserDetailsAsync(string details);
        Task<Result> AddUserToJointWalletAsync(int walletId, string username);
        Task<Result> RemoveUserFromJointWalletAsync(int walletId, string username);
    }
}
