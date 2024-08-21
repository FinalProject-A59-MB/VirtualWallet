using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public Task AddWalletAsync(Wallet wallet)
        {
            return _walletRepository.AddWalletAsync(wallet);
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
