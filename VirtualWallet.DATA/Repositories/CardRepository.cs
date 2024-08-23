using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Repositories.Contracts;

namespace VirtualWallet.DATA.Repositories
{
    public class CardRepository : ICardRepository
    {
        private readonly ApplicationDbContext _context;

        public CardRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        private IQueryable<Card> GetCardsWithDetails()
        {
            return _context.Cards
                .Include(c => c.User);
        }

        public IQueryable<Card> GetCardsByUserId(int userId)
        {
            return GetCardsWithDetails()
                .Where(c => c.UserId == userId);
        }

        public async Task<Card?> GetCardByIdAsync(int id)
        {
            return await GetCardsWithDetails()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddCardAsync(Card card)
        {
            card.CardNumber = ObfuscateCardNumber(card.CardNumber);
            await _context.Cards.AddAsync(card);
            await _context.SaveChangesAsync();
        }

        private string ObfuscateCardNumber(string cardNumber)
        {
            var lastFourDigits = cardNumber[^4..];
            return new string('*', cardNumber.Length - 4) + lastFourDigits;
        }

        public async Task UpdateCardAsync(Card card)
        {
            _context.Cards.Update(card);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveCardAsync(int cardId)
        {
            var card = await GetCardByIdAsync(cardId);
            if (card != null)
            {
                _context.Cards.Remove(card);
                await _context.SaveChangesAsync();
            }
        }
    }
}
