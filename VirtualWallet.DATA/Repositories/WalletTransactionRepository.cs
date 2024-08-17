using Microsoft.EntityFrameworkCore;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Repositories.Contracts;

namespace VirtualWallet.DATA.Repositories
{
    public class WalletTransactionRepository : IWalletTransactionRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public WalletTransactionRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<IEnumerable<WalletTransaction>> GetTransactionsByRecipientIdAsync(int recipientId)
        {
            return await _dbContext.WalletTransactions.Where(t => t.RecipientId == recipientId).ToListAsync();
        }

        public async Task<IEnumerable<WalletTransaction>> GetTransactionsBySenderIdAsync(int senderId)
        {
            return await _dbContext.WalletTransactions.Where(t => t.SenderId == senderId).ToListAsync();
        }

        public async Task<WalletTransaction?> GetTransactionByIdAsync(int id)
        {
            return await _dbContext.WalletTransactions.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task AddWalletTransactionAsync(WalletTransaction walletTransaction)
        {
            _dbContext.WalletTransactions.Add(walletTransaction);
            await _dbContext.SaveChangesAsync();
        }
    }
}
