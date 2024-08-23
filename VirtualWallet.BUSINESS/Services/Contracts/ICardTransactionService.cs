using VirtualWallet.BUSINESS.Results;
using VirtualWallet.DATA.Models;

namespace VirtualWallet.BUSINESS.Services.Contracts
{
    public interface ICardTransactionService
    {
        public Task<Result<CardTransaction>> DepositAsync(int cardId, int walletId, decimal amount);
        public Task<Result<CardTransaction>> WithdrawAsync(int walletId, int cardId, decimal amount);
    }
}