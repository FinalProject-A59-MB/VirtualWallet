using Microsoft.EntityFrameworkCore;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Repositories.Contracts;

namespace VirtualWallet.DATA.Repositories
{
    public class UserWalletRepository : IUserWalletRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly WalletRepository _walletRepository;

        public UserWalletRepository(ApplicationDbContext dbContext, WalletRepository walletRepository)
        {
            _dbContext = dbContext;
            _walletRepository = walletRepository;
        }


        public void AddUserWallet(UserWallet userWallet)
        {
            _dbContext.UserWallets.Add(userWallet);
            _dbContext.SaveChanges();
        }

        public async Task<IEnumerable<UserWallet>> GetUserWalletsByUserIdAsync(int userId)
        {
            return await _dbContext.UserWallets.Where(w => w.UserId == userId).ToListAsync();
        }

        public async Task<IEnumerable<UserWallet>> GetUserWalletByWalletIdAsync(int walletId)
        {
            return await _dbContext.UserWallets.Where(w => w.WalletId == walletId).ToListAsync();
        }

        public async Task RemoveUserWalletAsync(int walletId)
        {
            //Remove user from joint wallet

            var wallet = await _dbContext.UserWallets.FirstOrDefaultAsync(w => w.Id == walletId);

            if (wallet == null)
            {
                throw new Exception();
            }

            //TODO Does the money stay in the wallet when user is removed?

            _dbContext.UserWallets.Remove(wallet);
            await _dbContext.SaveChangesAsync();
        }
    }
}
