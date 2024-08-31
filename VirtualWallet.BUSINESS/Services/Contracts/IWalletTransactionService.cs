using VirtualWallet.BUSINESS.Results;
using VirtualWallet.DATA.Models;

namespace VirtualWallet.DATA.Services.Contracts
{
    public interface IWalletTransactionService
    {
        Task<Result<IEnumerable<WalletTransaction>>> GetTransactionsBySenderIdAsync(int senderId);
        Task<Result<IEnumerable<WalletTransaction>>> GetTransactionsByRecipientIdAsync(int recipientId);
        Task<Result<WalletTransaction>> GetTransactionByIdAsync(int id);
        Task<Result<int>> DepositStep1Async(int senderWalletId, int recipientWalletId, decimal amount);
        Task<Result<int>> DepositStep2Async(int senderWalletId, int recipientWalletId, int transactionId);
        Task<Result<IEnumerable<WalletTransaction>>> FilterWalletTransactionsAsync(TransactionQueryParameters parameters);
        Task<Result<int>> GetTotalCountAsync(TransactionQueryParameters filterParameters);
    }
}
