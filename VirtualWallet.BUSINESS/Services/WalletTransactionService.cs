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


        public Task AddWalletTransactionAsync(WalletTransaction walletTransaction)
        {


            return _walletTransactionRepository.AddWalletTransactionAsync(walletTransaction);
        }

        public Task<WalletTransaction?> GetTransactionByIdAsync(int id)
        {
            return _walletTransactionRepository.GetTransactionByIdAsync(id);
        }

        public Task<IEnumerable<WalletTransaction>> GetTransactionsByRecipientIdAsync(int recipientId)
        {
            return _walletTransactionRepository.GetTransactionsByRecipientIdAsync(recipientId);
        }

        public Task<IEnumerable<WalletTransaction>> GetTransactionsBySenderIdAsync(int senderId)
        {
            return _walletTransactionRepository.GetTransactionsBySenderIdAsync(senderId);
        }
    }
}
