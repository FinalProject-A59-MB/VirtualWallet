using VirtualWallet.DATA.Models;

namespace VirtualWallet.DATA.Repositories.Contracts
{
    public interface IWalletTransactionRepository
    {
        Task<IEnumerable<WalletTransaction>> GetTransactionsBySenderIdAsync(int senderId);
        Task<IEnumerable<WalletTransaction>> GetTransactionsByRecipientIdAsync(int recipientId);
        Task<WalletTransaction?> GetTransactionByIdAsync(int id);
        Task AddWalletTransactionAsync(WalletTransaction walletTransaction);
    }
}
