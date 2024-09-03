using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using System.Linq.Expressions;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.DATA.Repositories.Contracts;

namespace VirtualWallet.DATA.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IUserRepository _userRepository;

        public WalletRepository(ApplicationDbContext dbContext, IUserRepository userRepository)
        {
            _dbContext = dbContext;
            _userRepository = userRepository;
        }

        private IQueryable<Wallet> GetWalletsWithDetails()
        {
            return _dbContext.Wallets.
                Include(w => w.User).
                ThenInclude(u => u.UserProfile).
                Include(w => w.UserWallets).
                Include(w => w.WalletTransactions).
                Include(w => w.CardTransactions);
        }

        private IQueryable<WalletTransaction> GetWalletTransactionsWithDetails()
        {
            return _dbContext.WalletTransactions.
                Include(wt => wt.Recipient).
                ThenInclude(r => r.User).
                Include(wt => wt.Sender).
                ThenInclude(s => s.User);
        }

        public async Task<int> AddWalletAsync(Wallet wallet)
        {
            var newWallet = await _dbContext.Wallets.AddAsync(wallet);
            await _dbContext.SaveChangesAsync();

            return newWallet.Entity.Id;
        }


        public async Task<Wallet?> GetWalletByIdAsync(int id)
        {
            var wallet = await GetWalletsWithDetails().Include(w => w.User).
                Include(w => w.User).
                ThenInclude(u => u.UserProfile).
                Include(w => w.UserWallets).
                Include(w => w.WalletTransactions).
                Include(w => w.CardTransactions).FirstOrDefaultAsync(c => c.Id == id);

            return wallet;
        }

        public async Task<Wallet> GetWalletByNameAsync(string walletName)
        {
            var wallet = await _dbContext.Wallets.FirstOrDefaultAsync(w => w.Name == walletName);

            return wallet;
        }

        public async Task<IEnumerable<Wallet>> GetWalletsByUserIdAsync(int userId)
        {
            var wallets = await _dbContext.Wallets.Where(w => w.UserId == userId).ToListAsync();

            var jointWallets = await _dbContext.UserWallets.Include(w => w.Wallet).Where(x => x.UserId == userId).ToListAsync();

            wallets.AddRange(jointWallets.Select(x => x.Wallet));

            return wallets;
        }

        public async Task RemoveWalletAsync(int walletId)
        {
            var wallet = await GetWalletByIdAsync(walletId);

            if (wallet.WalletType != Models.Enums.WalletType.Joint)
            {
                var user = await _userRepository.GetUserByIdAsync(wallet.UserId);

                //TODO transfer the money from the wallet back to the user card
            }

            _dbContext.Wallets.Remove(wallet);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateWalletAsync(Wallet wallet)
        {
            var existingWallet = await _dbContext.Wallets.FindAsync(wallet.Id);

            if (existingWallet != null)
            {
                _dbContext.Entry(existingWallet).CurrentValues.SetValues(wallet);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                _dbContext.Wallets.Attach(wallet);
                _dbContext.Entry(wallet).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
            }
        }


        public async Task<Wallet> GetWalletByUserDetailsAsync(string details)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.Username == details || x.Email == details);

            if (user == null)
            {
                return null;
            }

            var wallet = await _dbContext.Wallets
                .FirstOrDefaultAsync(x => x.UserId == user.Id && x.WalletType == WalletType.Main);

            return wallet;
        }

        public async Task AddUserToJointWalletAsync(int walletId, int userId)
        {
            await _dbContext.UserWallets.AddAsync(new UserWallet()
            {
                WalletId = walletId,
                UserId = userId,
                Role = UserWalletRole.Member,
                JoinedDate = DateTime.UtcNow,
            });

            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveUserFromJointWalletAsync(int walletId, int userId)
        {
            var userWallet = _dbContext.UserWallets.FirstOrDefault(x => x.WalletId == walletId && x.UserId == userId);

            _dbContext.UserWallets.Remove(userWallet);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<ICollection<WalletTransaction>> FilterByAsync(WalletTransactionQueryParameters filterParameters, int? userId = null)
        {
            var transactions = GetWalletTransactionsWithDetails();
            if (userId.HasValue)
            {
                transactions = transactions.Where(t => t.SenderId == userId.Value || t.RecipientId == userId.Value);
            }

            if (filterParameters.WalletId.HasValue)
            {
                transactions = transactions.Where(t => t.SenderId >= filterParameters.WalletId);
            }

            if (filterParameters.WalletId.HasValue)
            {
                transactions = transactions.Where(t => t.RecipientId >= filterParameters.WalletId);
            }

            if (filterParameters.Amount.HasValue)
            {
                transactions = transactions.Where(t => t.AmountSent >= filterParameters.Amount.Value);
            }
            if (filterParameters.CreatedAfter.HasValue)
            {
                transactions = transactions.Where(t => t.CreatedAt >= filterParameters.CreatedAfter.Value);
            }
            if (filterParameters.CreatedBefore.HasValue)
            {
                transactions = transactions.Where(t => t.CreatedAt <= filterParameters.CreatedBefore.Value);
            }
            if (filterParameters.Status.HasValue)
            {
                transactions = transactions.Where(t => t.Status == filterParameters.Status.Value);
            }

            var sortPropertyMapping = new Dictionary<string, Expression<Func<WalletTransaction, object>>>()
        {
            { "CreatedAt", t => t.CreatedAt },
            { "Amount", t => t.AmountSent }
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

        public async Task<int> GetTotalCountAsync(WalletTransactionQueryParameters filterParameters, int? userId = null)
        {
            var transactions = GetWalletTransactionsWithDetails();

            if (userId.HasValue)
            {
                transactions = transactions.Where(t => t.SenderId == userId.Value || t.RecipientId == userId.Value);
            }

            if (filterParameters.SenderId.HasValue)
            {
                transactions = transactions.Where(t => t.SenderId >= filterParameters.SenderId);
            }

            if (filterParameters.RecipientId.HasValue)
            {
                transactions = transactions.Where(t => t.RecipientId >= filterParameters.RecipientId);
            }

            if (filterParameters.Amount.HasValue)
            {
                transactions = transactions.Where(t => t.AmountSent >= filterParameters.Amount.Value);
            }
            if (filterParameters.CreatedAfter.HasValue)
            {
                transactions = transactions.Where(t => t.CreatedAt >= filterParameters.CreatedAfter.Value);
            }
            if (filterParameters.CreatedBefore.HasValue)
            {
                transactions = transactions.Where(t => t.CreatedAt <= filterParameters.CreatedBefore.Value);
            }
            if (filterParameters.Status.HasValue)
            {
                transactions = transactions.Where(t => t.Status == filterParameters.Status.Value);
            }

            return await transactions.CountAsync();
        }

    }
}
