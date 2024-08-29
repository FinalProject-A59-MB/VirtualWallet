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
            return _dbContext.WalletTransactions
                .Include(t => t.Sender)
                .Include(t => t.Recipient)
                .Include(t => t.Sender.User)
                .ThenInclude(u => u.UserProfile)
                .Include(t => t.Recipient.User)
                .ThenInclude(u => u.UserProfile);
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
                        query = sortOrder ? query.OrderBy(t => t.Amount) : query.OrderByDescending(t => t.Amount);
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
