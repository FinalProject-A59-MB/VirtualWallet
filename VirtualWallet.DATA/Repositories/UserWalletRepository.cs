using Microsoft.EntityFrameworkCore;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.DATA.Repositories.Contracts;

namespace VirtualWallet.DATA.Repositories
{
    public class UserWalletRepository : IUserWalletRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UserWalletRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<int> AddUserWalletAsync(int walletId, int userId, UserWalletRole role)
        {
            var walletToAdd = new UserWallet() 
            {
                WalletId = walletId,
                UserId = userId, 
                Role = role ,
            };

            var userWalletToAdd = _dbContext.UserWallets.Add(walletToAdd);
            await _dbContext.SaveChangesAsync();

            return userWalletToAdd.Entity.Id;
        }

        public async Task<IEnumerable<UserWallet>> GetUserWalletsByUserIdAsync(int userId)
        {
            return await _dbContext.UserWallets.Where(w => w.UserId == userId).ToListAsync();
        }

        public async Task<IEnumerable<UserWallet>> GetUserWalletByWalletIdAsync(int walletId)
        {
            return await _dbContext.UserWallets.Where(w => w.WalletId == walletId).ToListAsync();
        }

        public async Task RemoveUserWalletAsync(int walletId, int userId)
        {
            //Remove user from joint wallet

            var userWallet = await _dbContext.UserWallets.FirstOrDefaultAsync(uw => uw.WalletId == walletId && uw.UserId == userId );

            if (userWallet == null)
            {
                throw new Exception();
            }

            //TODO Does the money stay in the wallet when user is removed?

            _dbContext.UserWallets.Remove(userWallet);
            await _dbContext.SaveChangesAsync();
        }
    }
}
