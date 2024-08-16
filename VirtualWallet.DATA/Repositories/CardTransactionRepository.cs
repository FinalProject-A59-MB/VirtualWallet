using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Repositories.Contracts;

namespace VirtualWallet.DATA.Repositories
{
    public class CardTransactionRepository : ICardTransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public CardTransactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        private IQueryable<CardTransaction> GetCardTransactionsWithDetails()
        {
            return _context.CardTransactions
                .Include(ct => ct.User)
                .Include(ct => ct.Card)
                .Include(ct => ct.Wallet);
        }

        public IQueryable<CardTransaction> GetTransactionsByUserId(int userId)
        {
            return GetCardTransactionsWithDetails()
                .Where(ct => ct.UserId == userId);
        }

        public IQueryable<CardTransaction> GetTransactionsByCardId(int cardId)
        {
            return GetCardTransactionsWithDetails()
                .Where(ct => ct.CardId == cardId);
        }

        public async Task<CardTransaction?> GetTransactionByIdAsync(int id)
        {
            return await GetCardTransactionsWithDetails()
                .FirstOrDefaultAsync(ct => ct.Id == id);
        }

        public async Task AddCardTransactionAsync(CardTransaction cardTransaction)
        {
            await _context.CardTransactions.AddAsync(cardTransaction);
            await _context.SaveChangesAsync();
        }
    }
}
