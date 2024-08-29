using VirtualWallet.BUSINESS.Results;
using VirtualWallet.BUSINESS.Services.Responses;
using VirtualWallet.DATA.Models.Enums;

namespace VirtualWallet.BUSINESS.Services.Contracts
{
    public interface ICurrencyService
    {
        public Task<Result<CurrencyExchangeRatesResponse>> GetRatesForCurrencyAsync(CurrencyType baseCurrency);
    }
}
