using VirtualWallet.BUSINESS.Services.Responses;
using VirtualWallet.DATA.Models.Enums;

namespace VirtualWallet.BUSINESS.Services.Contracts
{
    public interface ICurrencyService
    {
        Task<CurrencyExchangeRatesResponse> GetRatesForCurrencyAsync(CurrencyType baseCurrency);
    }
}
