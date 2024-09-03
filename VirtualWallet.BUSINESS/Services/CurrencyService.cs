using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
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

        public async Task<Result<decimal>> ConvertCurrencyAsync(decimal amount, CurrencyType senderCurrency, CurrencyType destinationCurrency)
        {
            if (senderCurrency == destinationCurrency)
            {
                return Result<decimal>.Success(amount);
            }

            var ratesResult = await GetRatesForCurrencyAsync(senderCurrency);

            if (!ratesResult.IsSuccess)
            {
                return Result<decimal>.Failure(ratesResult.Error);
            }

            var rates = ratesResult.Value;

            string destinationCurrencyCode = destinationCurrency.ToString();

            if (!rates.Data.TryGetValue(destinationCurrencyCode, out decimal exchangeRate))
            {
                return Result<decimal>.Failure("Unsupported or unavailable destination currency.");
            }

            decimal convertedAmount = amount * exchangeRate;

            return Result<decimal>.Success(convertedAmount);
        }
    }
}
