using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Models.Enums;
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
                var cardTransactions = await _context.CardTransactions
                    .Where(ct => ct.CardId == cardId)
                    .ToListAsync();

                _context.CardTransactions.RemoveRange(cardTransactions);

                await _context.SaveChangesAsync();

                _context.Cards.Remove(card);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<ICollection<CardTransaction>> FilterByAsync(CardTransactionQueryParameters filterParameters, int? userId = null)
        {
            var transactions = _context.CardTransactions.AsQueryable();

            if (userId.HasValue)
            {
                transactions = transactions.Where(t => t.UserId == userId.Value);
            }

            if (filterParameters.CardId != 0)
            {
                transactions = transactions.Where(t => t.CardId == filterParameters.CardId);
            }
            if (filterParameters.Amount != 0)
            {
                transactions = transactions.Where(t => t.Amount == filterParameters.Amount);
            }
            if (filterParameters.CreatedAfter.HasValue)
            {
                transactions = transactions.Where(t => t.CreatedAt >= filterParameters.CreatedAfter.Value);
            }
            if (filterParameters.CreatedBefore.HasValue)
            {
                transactions = transactions.Where(t => t.CreatedAt <= filterParameters.CreatedBefore.Value);
            }
            if (!string.IsNullOrEmpty(filterParameters.TransactionType))
            {
                if (Enum.TryParse<TransactionType>(filterParameters.TransactionType, out var transactionTypeEnum))
                {
                    transactions = transactions.Where(t => t.TransactionType == transactionTypeEnum);
                }
            }

            var sortPropertyMapping = new Dictionary<string, Expression<Func<CardTransaction, object>>>()
    {
        { "CreatedAt", t => t.CreatedAt },
        { "Amount", t => t.Amount }
    };

            if (!string.IsNullOrEmpty(filterParameters.SortBy) && sortPropertyMapping.ContainsKey(filterParameters.SortBy))
            {
                transactions = filterParameters.SortOrder.ToLower() == "asc"
                    ? transactions.OrderBy(sortPropertyMapping[filterParameters.SortBy])
                    : transactions.OrderByDescending(sortPropertyMapping[filterParameters.SortBy]);
            }

            var skip = (filterParameters.PageNumber - 1) * filterParameters.PageSize;

            return await transactions.Skip(skip).Take(filterParameters.PageSize).ToListAsync();
        }
        public async Task<int> GetTotalCountAsync(CardTransactionQueryParameters filterParameters, int? userId = null)
        {
            var transactions = _context.CardTransactions.AsQueryable();

            if (userId.HasValue)
            {
                transactions = transactions.Where(t => t.UserId == userId.Value);
            }

            if (!string.IsNullOrEmpty(filterParameters.TransactionType))
            {
                if (Enum.TryParse<TransactionType>(filterParameters.TransactionType, out var transactionTypeEnum))
                {
                    transactions = transactions.Where(t => t.TransactionType == transactionTypeEnum);
                }
                else
                {
                    return 0;
                }
            }

            if (filterParameters.CreatedAfter.HasValue)
            {
                transactions = transactions.Where(t => t.CreatedAt >= filterParameters.CreatedAfter.Value);
            }
            if (filterParameters.CreatedBefore.HasValue)
            {
                transactions = transactions.Where(t => t.CreatedAt <= filterParameters.CreatedBefore.Value);
            }
            if (filterParameters.CardId != 0)
            {
                transactions = transactions.Where(t => t.CardId == filterParameters.CardId);
            }

            return await transactions.CountAsync();
        }

    }
}
