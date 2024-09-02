using Microsoft.EntityFrameworkCore;
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


        public async Task<int> AddWalletAsync(Wallet wallet)
        {
            var newWallet = await _dbContext.Wallets.AddAsync(wallet);
            await _dbContext.SaveChangesAsync();

            return newWallet.Entity.Id;
        }

        public async Task<Wallet> GetWalletByIdAsync(int id)
        {
            var wallet = await _dbContext.Wallets
                .Include(x => x.User)
                .Include(x => x.UserWallets)
                .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(w => w.Id == id);

            var listOfSentTransactions = await _dbContext.WalletTransactions
                .Include(x => x.Sender)
                .Include(x => x.Recipient)
                .Where(x => x.SenderId == id)
                .ToListAsync();

            var listOfRecievedTransactions = await _dbContext.WalletTransactions
                .Include(x => x.Sender)
                .Include(x => x.Recipient)
                .Where(x => x.RecipientId == id && x.Status == TransactionStatus.Completed)
                .ToListAsync();

            if (wallet != null)
            {
                var allTransactions = listOfSentTransactions;
                allTransactions.AddRange(listOfRecievedTransactions);

                allTransactions.OrderByDescending(x => x.CreatedAt);

                wallet.WalletTransactions = allTransactions;
            }

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
            var walletToUpdate = await GetWalletByIdAsync(wallet.Id);

            walletToUpdate.Name = wallet.Name;
            await _dbContext.SaveChangesAsync();
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
    }
}
