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

        private IQueryable<WalletTransaction> GetWalletTransactionsWithDetails()
        {
            return _dbContext.WalletTransactions.
                Include(t => t.Recipient).
                ThenInclude(u => u.User).
                Include(t => t.Sender).
                ThenInclude(u => u.User);
        }


        public async Task<IEnumerable<WalletTransaction>> GetTransactionsByRecipientIdAsync(int recipientId)
        {
            return await GetWalletTransactionsWithDetails().Where(t => t.RecipientId == recipientId).ToListAsync();
        }

        public async Task<IEnumerable<WalletTransaction>> GetTransactionsBySenderIdAsync(int senderId)
        {
            return await GetWalletTransactionsWithDetails().Where(t => t.SenderId == senderId).ToListAsync();
        }

        public async Task<WalletTransaction?> GetTransactionByIdAsync(int id)
        {
            return await GetWalletTransactionsWithDetails().FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task AddWalletTransactionAsync(WalletTransaction walletTransaction)
        {
            _dbContext.WalletTransactions.Add(walletTransaction);
            await _dbContext.SaveChangesAsync();
        }
        public async Task UpdateWalletTransactionAsync(WalletTransaction walletTransaction)
        {
            var existingTransaction = await _dbContext.WalletTransactions.FindAsync(walletTransaction.Id);

            if (existingTransaction != null)
            {
                _dbContext.Entry(existingTransaction).CurrentValues.SetValues(walletTransaction);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                _dbContext.WalletTransactions.Attach(walletTransaction);
                _dbContext.Entry(walletTransaction).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<WalletTransaction>> GetAllWalletTransactionsAsync()
        {
            return await GetWalletTransactionsWithDetails().ToListAsync();
        }

        public IQueryable<WalletTransaction> FilterWalletTransactions(TransactionQueryParameters parameters)
        {
            var query = GetWalletTransactionsWithDetails();

            if (!string.IsNullOrEmpty(parameters.Sender?.Username))
            {
                query = query.Where(t => t.Sender.Name.Contains(parameters.Sender.Username));
            }

            if (!string.IsNullOrEmpty(parameters.Recipient?.Username))
            {
                query = query.Where(t => t.Recipient.Name.Contains(parameters.Recipient.Username));
            }

            if (parameters.StartDate.HasValue)
            {
                query = query.Where(t => t.CreatedAt >= parameters.StartDate.Value);
            }

            if (parameters.EndDate.HasValue)
            {
                query = query.Where(t => t.CreatedAt <= parameters.EndDate.Value);
            }

            if (parameters.Direction == "Incoming" && parameters.Recipient != null)
            {
                query = query.Where(t => t.RecipientId == parameters.Recipient.Id);
            }
            else if (parameters.Direction == "Outgoing" && parameters.Sender != null)
            {
                query = query.Where(t => t.SenderId == parameters.Sender.Id);
            }

            if (!string.IsNullOrEmpty(parameters.SortBy))
            {
                var sortOrder = parameters.SortOrder?.ToLower() == "asc";

                switch (parameters.SortBy)
                {
                    case "Amount":
                        query = sortOrder ? query.OrderBy(t => t.AmountSent) : query.OrderByDescending(t => t.AmountSent);
                        break;
                    case "CreatedAt":
                    default:
                        query = sortOrder ? query.OrderBy(t => t.CreatedAt) : query.OrderByDescending(t => t.CreatedAt);
                        break;
                }
            }

            var skip = (parameters.PageNumber - 1) * parameters.PageSize;
            query = query.Skip(skip).Take(parameters.PageSize);

            return query;
        }


    }
}
