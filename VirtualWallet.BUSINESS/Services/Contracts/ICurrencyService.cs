using VirtualWallet.BUSINESS.Results;
using VirtualWallet.BUSINESS.Services.Responses;
using VirtualWallet.DATA.Models.Enums;

namespace VirtualWallet.BUSINESS.Services.Contracts
{
    public interface ICurrencyService
    {
        Task<Result<CurrencyExchangeRatesResponse>> GetRatesForCurrencyAsync(CurrencyType baseCurrency);

        Task<Result<decimal>> ConvertCurrencyAsync(decimal amount, CurrencyType senderCurrency, CurrencyType destinationCurrency);
    }
}
