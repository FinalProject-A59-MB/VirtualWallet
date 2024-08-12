using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualWallet.DATA.Models;

namespace VirtualWallet.DATA.Repositories.Contracts
{
    public interface IWalletTransactionRepository
    {
        IEnumerable<WalletTransaction> GetTransactionsBySenderId(int senderId);
        IEnumerable<WalletTransaction> GetTransactionsByRecipientId(int recipientId);
        WalletTransaction GetTransactionById(int id);
        void AddWalletTransaction(WalletTransaction walletTransaction)
    }
}
