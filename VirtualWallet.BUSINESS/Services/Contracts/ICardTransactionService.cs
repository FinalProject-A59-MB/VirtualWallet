using VirtualWallet.DATA.Models;

namespace VirtualWallet.BUSINESS.Services.Contracts
{
    public interface ICardTransactionService
    {
        Task<CardTransaction> DepositAsync(int cardId, int walletId, decimal amount);
        Task<CardTransaction> WithdrawAsync(int walletId, int cardId, decimal amount);
    }
}