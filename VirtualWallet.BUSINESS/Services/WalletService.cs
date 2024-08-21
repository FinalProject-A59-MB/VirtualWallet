using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Repositories.Contracts;
using VirtualWallet.DATA.Services.Contracts;

namespace VirtualWallet.DATA.Services
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;

        public WalletService(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository;
        }
        public async Task<int> AddWalletAsync(Wallet wallet)
        {
            var newWalletId = await _walletRepository.AddWalletAsync(wallet);

            return newWalletId;
        }

        public Task<Wallet> GetWalletByIdAsync(int id)
        {
            return _walletRepository.GetWalletByIdAsync(id);
        }

        public Task<Wallet> GetWalletByNameAsync(string walletName)
        {
            return _walletRepository.GetWalletByNameAsync(walletName);
        }

        public Task<IEnumerable<Wallet>> GetWalletsByUserIdAsync(int userId)
        {
            return _walletRepository.GetWalletsByUserIdAsync(userId);
        }

        public Task RemoveWalletAsync(int walletId)
        {
            return _walletRepository.RemoveWalletAsync(walletId);
        }

        public Task UpdateWalletAsync(Wallet wallet)
        {
            return _walletRepository.UpdateWalletAsync(wallet);
        }


    }
}
