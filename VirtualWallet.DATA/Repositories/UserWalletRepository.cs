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

        public IEnumerable<UserWallet> GetUserWalletsByUserId(int userId)
        {
            return _dbContext.UserWallets.Where(w => w.UserId == userId);
        }

        public IEnumerable<UserWallet> GetUserWalletByWalletId(int walletId)
        {
            return _dbContext.UserWallets.Where(w => w.WalletId == walletId);
        }

        public void RemoveUserWallet(int walletId)
        {
            //Remove user from joint wallet

            var wallet = _dbContext.UserWallets.FirstOrDefault(w => w.Id == walletId);

            if (wallet == null)
            {
                throw new Exception();
            }

            //TODO Does the money stay in the wallet when user is removed?

            _dbContext.UserWallets.Remove(wallet);
            _dbContext.SaveChanges();
        }
    }
}
