using VirtualWallet.BUSINESS.Results;
using VirtualWallet.DATA.Models;

namespace VirtualWallet.DATA.Services.Contracts
{
    public interface IWalletTransactionService
    {
        Task<Result<IEnumerable<WalletTransaction>>> GetTransactionsBySenderIdAsync(int senderId);
        Task<Result<IEnumerable<WalletTransaction>>> GetTransactionsByRecipientIdAsync(int recipientId);
        Task<Result<WalletTransaction>> GetTransactionByIdAsync(int id);
        Task<Result<WalletTransaction>> VerifySendAmountAsync(int senderWalletId, User recepient, decimal amount);
        Task<Result<WalletTransaction>> ProcessSendAmountAsync(WalletTransaction transaction);

        Task<Result<WalletTransaction>> ProcessSendAmountInternalAsync(int senderWalletId, int recepientWalletId, decimal amount);
        Task<Result<IEnumerable<WalletTransaction>>> FilterWalletTransactionsAsync(TransactionQueryParameters parameters);
        Task<Result<int>> GetTotalCountAsync(TransactionQueryParameters filterParameters);

        Task<Result> UpdateTransaction(WalletTransaction transaction);
    }
}
