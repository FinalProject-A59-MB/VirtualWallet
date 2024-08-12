using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualWallet.DATA.Models;

namespace VirtualWallet.DATA.Repositories.Contracts
{
    public interface ICardTransactionRepository
    {
        IEnumerable<CardTransaction> GetTransactionsByUserId(int userId);
        IEnumerable<CardTransaction> GetTransactionsByCardId(int cardId);
        CardTransaction GetTransactionById(int id);
        void AddCardTransaction(CardTransaction cardTransaction);
    }
}
