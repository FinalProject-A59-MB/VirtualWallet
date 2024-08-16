using Microsoft.EntityFrameworkCore;
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
        public IQueryable<CardTransaction> GetTransactionsByUserId(int userId);

        public IQueryable<CardTransaction> GetTransactionsByCardId(int cardId);

        public Task<CardTransaction?> GetTransactionByIdAsync(int id);

        public Task AddCardTransactionAsync(CardTransaction cardTransaction);
    }
}
