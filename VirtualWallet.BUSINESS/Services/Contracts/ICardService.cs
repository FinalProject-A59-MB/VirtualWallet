using VirtualWallet.BUSINESS.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualWallet.DATA.Models;
using VirtualWallet.BUSINESS.Resources;
using VirtualWallet.BUSINESS.Results;

namespace VirtualWallet.BUSINESS.Services.Contracts
{
    public interface ICardService
    {
        public Task<Result<Card>> GetCardByIdAsync(int cardId);

        public Task<Result<IEnumerable<Card>>> GetUserCardsAsync(int userId);

        public Task<Result> AddCardAsync(User user, Card card);

        public Task<Result> DeleteCardAsync(int cardId);

        public Task<Result> UpdateCardAsync(Card card);
    }
}
