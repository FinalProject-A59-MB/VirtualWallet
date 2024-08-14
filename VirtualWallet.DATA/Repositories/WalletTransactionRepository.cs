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

        public void AddWalletTransaction(WalletTransaction walletTransaction)
        {
            _dbContext.WalletTransactions.Add(walletTransaction);
            _dbContext.SaveChanges();
        }

        public WalletTransaction GetTransactionById(int id)
        {
            return _dbContext.WalletTransactions.FirstOrDefault(t => t.Id == id);
        }

        public IEnumerable<WalletTransaction> GetTransactionsByRecipientId(int recipientId)
        {
            return _dbContext.WalletTransactions.Where(t => t.RecipientId == recipientId);
        }

        public IEnumerable<WalletTransaction> GetTransactionsBySenderId(int senderId)
        {
            return _dbContext.WalletTransactions.Where(t => t.SenderId == senderId);
        }
    }
}
