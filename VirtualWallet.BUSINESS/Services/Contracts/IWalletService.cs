using VirtualWallet.DATA.Models;

namespace VirtualWallet.DATA.Services.Contracts
{
    public interface IWalletService
    {
        Task<Wallet> GetWalletByIdAsync(int id);
        Task<IEnumerable<Wallet>> GetWalletsByUserIdAsync(int userId);
        Task<Wallet> GetWalletByNameAsync(string walletName);
        Task AddWalletAsync(Wallet wallet);
        Task UpdateWalletAsync(int walletId, Wallet wallet);
        Task RemoveWalletAsync(int walletId);
    }
}
