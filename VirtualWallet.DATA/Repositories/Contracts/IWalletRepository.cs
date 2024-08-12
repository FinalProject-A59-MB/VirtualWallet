using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualWallet.DATA.Models;

namespace VirtualWallet.DATA.Repositories.Contracts
{
    public interface IWalletRepository
    {
        Wallet GetWalletById(int id);
        IEnumerable<Wallet> GetWalletsByUserId(int userId);
        Wallet GetWalletByName(string walletName);
        void AddWallet(Wallet wallet);
        void UpdateWallet(Wallet wallet);
        void RemoveWallet(int walletId);
    }
}
