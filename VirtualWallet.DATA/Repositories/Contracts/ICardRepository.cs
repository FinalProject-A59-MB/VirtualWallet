using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualWallet.DATA.Models;

namespace VirtualWallet.DATA.Repositories.Contracts
{
    public interface ICardRepository
    {
        public IQueryable<Card> GetCardsByUserId(int userId);

        public Task<Card?> GetCardByIdAsync(int id);

        public Task AddCardAsync(Card card);

        public Task UpdateCardAsync(Card card);

        public Task RemoveCardAsync(int cardId);
    }
}
