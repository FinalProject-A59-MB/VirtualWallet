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
        IEnumerable<Card> GetCardsByUserId(int userId);
        Card GetCardById(int id);
        void AddCard(Card card);
        void UpdateCard(Card card);
        void RemoveCard(int cardId);
    }
}
