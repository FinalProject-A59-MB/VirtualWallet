using VirtualWallet.DATA.Models;
using VirtualWallet.BUSINESS.Results;
using VirtualWallet.DATA.Models.Enums;

namespace VirtualWallet.BUSINESS.Services.Contracts
{
    public interface IPaymentProcessorService
    {
        public Task<Result<string>> VerifyAndRetrieveTokenAsync(Card card);
        public Task<Result> WithdrawFromRealCardAsync(string paymentProcessorToken, decimal amount);
        public Task<Result> DepositToRealCardAsync(string paymentProcessorToken, decimal amount);
        public Task<Result<CurrencyType>> GetCardCurrency(string paymentProcessorToken);
    }

}
