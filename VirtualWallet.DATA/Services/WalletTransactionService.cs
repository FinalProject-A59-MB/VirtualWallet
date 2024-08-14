using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Repositories.Contracts;
using VirtualWallet.DATA.Services.Contracts;

namespace VirtualWallet.DATA.Services
{
    public class WalletTransactionService : IWalletTransactionService
    {
        private readonly IWalletTransactionRepository _walletTransactionRepository;

        public WalletTransactionService(IWalletTransactionRepository walletTransactionRepository)
        {
            _walletTransactionRepository = walletTransactionRepository;
        }
        public void AddWalletTransaction(WalletTransaction walletTransaction)
        {
            _walletTransactionRepository.AddWalletTransaction(walletTransaction);
        }

        public WalletTransaction GetTransactionById(int id)
        {
            return _walletTransactionRepository.GetTransactionById(id);
        }

        public IEnumerable<WalletTransaction> GetTransactionsByRecipientId(int recipientId)
        {
            return _walletTransactionRepository.GetTransactionsByRecipientId(recipientId);
        }

        public IEnumerable<WalletTransaction> GetTransactionsBySenderId(int senderId)
        {
            return _walletTransactionRepository.GetTransactionsBySenderId(senderId);
        }
    }
}
