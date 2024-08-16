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
        public void AddWallet(Wallet wallet)
        {
            _walletRepository.AddWallet(wallet);
        }

        public Wallet GetWalletById(int id)
        {
            return _walletRepository.GetWalletById(id);
        }

        public Wallet GetWalletByName(string walletName)
        {
            return _walletRepository.GetWalletByName(walletName);
        }

        public IEnumerable<Wallet> GetWalletsByUserId(int userId)
        {
            return _walletRepository.GetWalletsByUserId(userId);
        }

        public void RemoveWallet(int walletId)
        {
            _walletRepository.RemoveWallet(walletId);
        }

        public void UpdateWallet(Wallet wallet)
        {
            _walletRepository.UpdateWallet(wallet);
        }
    }
}
