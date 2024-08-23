using System.Net.Http.Json;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.BUSINESS.Services.Responses;
using VirtualWallet.DATA.Models.Enums;

namespace VirtualWallet.BUSINESS.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly string apiKey = "fca_live_XozJdfMfGEsu2eYxFNc6MvIFPyogfTFSsWiynTQU";
        private readonly string baseUrl = "https://api.freecurrencyapi.com/v1/";

        private readonly HttpClient _httpClient;

        public CurrencyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CurrencyExchangeRatesResponse> GetRatesForCurrencyAsync(CurrencyType baseCurrency)
        {

            string endpoint = $"{baseUrl}latest?apikey={apiKey}&base_currency={baseCurrency}";

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
            }

            using HttpClient client = _httpClient;

            try
            {
                var response = await client.GetFromJsonAsync<CurrencyExchangeRatesResponse>(endpoint);

                if (response == null)
                {
                    throw new Exception();
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
    }
}
