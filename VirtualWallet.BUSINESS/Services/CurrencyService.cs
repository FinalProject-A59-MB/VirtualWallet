using System.Net.Http.Json;
using VirtualWallet.BUSINESS.Results;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.BUSINESS.Services.Responses;
using VirtualWallet.DATA.Models.Enums;

namespace VirtualWallet.BUSINESS.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly string _apiKey = "fca_live_XozJdfMfGEsu2eYxFNc6MvIFPyogfTFSsWiynTQU";
        private readonly string _baseUrl = "https://api.freecurrencyapi.com/v1/";
        private readonly HttpClient _httpClient;

        public CurrencyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Result<CurrencyExchangeRatesResponse>> GetRatesForCurrencyAsync(CurrencyType baseCurrency)
        {
            string endpoint = $"{_baseUrl}latest?apikey={_apiKey}&base_currency={baseCurrency}";

            switch (baseCurrency)
            {
                case CurrencyType.BGN:
                    endpoint += "&currencies=EUR,USD";
                    break;
                case CurrencyType.EUR:
                    endpoint += "&currencies=BGN,USD";
                    break;
                case CurrencyType.USD:
                    endpoint += "&currencies=EUR,BGN";
                    break;
                default:
                    return Result<CurrencyExchangeRatesResponse>.Failure("Unsupported currency type.");
            }

            try
            {
                var response = await _httpClient.GetFromJsonAsync<CurrencyExchangeRatesResponse>(endpoint);

                if (response == null)
                {
                    return Result<CurrencyExchangeRatesResponse>.Failure("Failed to retrieve currency rates.");
                }

                return Result<CurrencyExchangeRatesResponse>.Success(response);
            }
            catch (Exception ex)
            {
                return Result<CurrencyExchangeRatesResponse>.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}
