using VirtualWallet.BUSINESS.Results;
using VirtualWallet.DATA.Models;

namespace VirtualWallet.DATA.Services.Contracts
{
    public interface IWalletTransactionService
    {
        Task<Result<IEnumerable<WalletTransaction>>> GetTransactionsBySenderIdAsync(int senderId);
        Task<Result<IEnumerable<WalletTransaction>>> GetTransactionsByRecipientIdAsync(int recipientId);
        Task<Result<WalletTransaction>> GetTransactionByIdAsync(int id);
        Task<Result<WalletTransaction>> DepositAsync(int senderWalletId, int recipientWalletId, decimal amount);
    }
}
