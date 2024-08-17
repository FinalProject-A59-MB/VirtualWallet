using VirtualWallet.BUSINESS.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualWallet.DATA.Models;

namespace VirtualWallet.BUSINESS.Services.Contracts
{
    public interface ICardService
    {
        public Task<Card> GetCardByIdAsync(int cardId);

        public Task<IEnumerable<Card>> GetUserCardsAsync(int userId);

        public Task AddCardAsync(Card card);

        public Task DeleteCardAsync(int cardId);

        public Task UpdateCardAsync(Card card);
    }
}
