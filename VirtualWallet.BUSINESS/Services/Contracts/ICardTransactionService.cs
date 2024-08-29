using VirtualWallet.BUSINESS.Results;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Models.Enums;

namespace VirtualWallet.BUSINESS.Services.Contracts
{
    public interface ICardTransactionService
    {
        public Task<Result<CardTransaction>> DepositAsync(int cardId, int walletId, decimal amount);
        public Task<Result<CardTransaction>> WithdrawAsync(int walletId, int cardId, decimal amount);
        public Task<Result<IEnumerable<CardTransaction>>> FilterCardTransactionsAsync(CardTransactionQueryParameters parameters);

        public Task<Result<int>> GetCardTransactionTotalCountAsync(CardTransactionQueryParameters filterParameters);

        public Task<Result<Dictionary<string, decimal>>> CalculateFeeAsync(decimal amount, CurrencyType fromCurrency, CurrencyType toCurrency);
    }
}