using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualWallet.DATA.Models;

namespace VirtualWallet.DATA.Repositories.Contracts
{
    public interface IUserWalletRepository
    {
        IEnumerable<UserWallet> GetUserWalletsByUserId(int userId);
        IEnumerable<UserWallet> GetUserWalletsByWalletId(int walletId);
        void AddUserWallet(UserWallet userWallet);
        void RemoveUserWallet(int walletId);
    }
}
