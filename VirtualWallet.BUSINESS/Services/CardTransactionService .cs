using VirtualWallet.BUSINESS.Results;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Repositories.Contracts;
using VirtualWallet.BUSINESS.Resources;
using VirtualWallet.DATA.Repositories;
using System.Transactions;
using VirtualWallet.DATA.Models.Enums;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace VirtualWallet.BUSINESS.Services
{
    public class CardTransactionService : ICardTransactionService
    {
        private readonly ICardRepository _cardRepository;
        private readonly ICardTransactionRepository _cardTransactionRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly ITransactionHandlingService _transactionHandlingService;
        private readonly ICurrencyService _currencyService;

        public CardTransactionService(
            ICardRepository cardRepository,
            ICardTransactionRepository cardTransactionRepository,
            IWalletRepository walletRepository,
            ITransactionHandlingService transactionHandlingService,
            ICurrencyService currencyService)
        {
            _cardRepository = cardRepository;
            _walletRepository = walletRepository;
            _transactionHandlingService = transactionHandlingService;
            _cardTransactionRepository = cardTransactionRepository;
            _currencyService = currencyService;
        }
        public async Task<Result<CardTransaction>> DepositAsync(int cardId, int walletId, decimal amount)
        {
            var cardResult = await _cardRepository.GetCardByIdAsync(cardId);
            if (cardResult == null)
                return Result<CardTransaction>.Failure(ErrorMessages.CardNotFound);

            var walletResult = await _walletRepository.GetWalletByIdAsync(walletId);
            if (walletResult == null)
                return Result<CardTransaction>.Failure(ErrorMessages.WalletNotFound);

            if (amount <= 0)
                return Result<CardTransaction>.Failure(ErrorMessages.InvalidDepositAmount);

            var transactionResult = await _transactionHandlingService.ProcessCardToWalletTransactionAsync(cardResult, walletResult, amount);
            return transactionResult;
        }

        public async Task<Result<CardTransaction>> WithdrawAsync(int walletId, int cardId, decimal amount)
        {
            var walletResult = await _walletRepository.GetWalletByIdAsync(walletId);
            if (walletResult == null)
                return Result<CardTransaction>.Failure(ErrorMessages.WalletNotFound);

            var cardResult = await _cardRepository.GetCardByIdAsync(cardId);
            if (cardResult == null)
                return Result<CardTransaction>.Failure(ErrorMessages.CardNotFound);

            if (amount <= 0)
                return Result<CardTransaction>.Failure(ErrorMessages.InvalidWithdrawalAmount);
            decimal feeAmount = 0;
            decimal amountToWithdraw = amount;

            // Calculate the fee and the actual amount to withdraw
            if (cardResult.Currency != walletResult.Currency)
            {
                var feeCalculationResult = await CalculateFeeAsync(amount, walletResult.Currency, cardResult.Currency);
                if (!feeCalculationResult.IsSuccess)
                    return Result<CardTransaction>.Failure(feeCalculationResult.Error);
                var feeAndAmount = feeCalculationResult.Value;
                amountToWithdraw = feeAndAmount["amountToWithdraw"];
                feeAmount = feeAndAmount["feeAmount"];
            }

            var totalAmountToWithdraw = amountToWithdraw + feeAmount;

            if (walletResult.Balance < totalAmountToWithdraw)
                return Result<CardTransaction>.Failure(ErrorMessages.InsufficientWalletFunds);

            var transactionResult = await _transactionHandlingService.ProcessWalletToCardTransactionAsync(walletResult, cardResult, amountToWithdraw, feeAmount);
            if (!transactionResult.IsSuccess)
            {
                return Result<CardTransaction>.Failure(transactionResult.Error);
            }

            // Deduct the fee from the wallet's balance if applicable
            walletResult.Balance -= feeAmount;

            await _walletRepository.UpdateWalletAsync(walletResult);

            return transactionResult;
        }


        public async Task<Result<Dictionary<string, decimal>>> CalculateFeeAsync(decimal amount, CurrencyType fromCurrency, CurrencyType toCurrency)
        {
            var exchangeRateResult = await _currencyService.GetRatesForCurrencyAsync(fromCurrency);

            if (!exchangeRateResult.IsSuccess)
            {
                return Result<Dictionary<string, decimal>>.Failure(exchangeRateResult.Error);
            }
            decimal exchangeRate;
            // Get the exchange rate for the target currency
            try
            {
                exchangeRate = exchangeRateResult.Value.Data[toCurrency.ToString()];
            }catch (Exception ex)
            {
                return Result<Dictionary<string, decimal>>.Failure(ex.Message);
            }
            

            // Calculate the amount in the target currency
            var amountInTargetCurrency = amount * exchangeRate;

            // Calculate the fee based on the target currency
            decimal feePercentage = (int)toCurrency / 100m;
            var feeAmount = amountInTargetCurrency * feePercentage;

            var result = new Dictionary<string, decimal>
                {
                    { "amountToWithdraw", amountInTargetCurrency - feeAmount }, 
                    { "feeAmount", feeAmount }
                };

            return Result<Dictionary<string, decimal>>.Success(result);
        }




        public async Task<Result<IEnumerable<CardTransaction>>> FilterCardTransactionsAsync(CardTransactionQueryParameters parameters)
        {
            var query = await _cardTransactionRepository.GetAllCardTransactionsAsync();

            if (parameters.CardId > 0)
            {
                query = query.Where(t => t.Card.Id == parameters.CardId);
            }

            if (!string.IsNullOrEmpty(parameters.CardNumber))
            {
                query = query.Where(t => t.Card.CardNumber == parameters.CardNumber);
            }

            if (!string.IsNullOrEmpty(parameters.Wallet))
            {
                query = query.Where(t => t.Wallet.Name == parameters.Wallet);
            }

            if (parameters.Amount > 0)
            {
                query = query.Where(t => t.Amount == parameters.Amount);
            }

            if (parameters.CreatedAfter.HasValue)
            {
                query = query.Where(t => t.CreatedAt >= parameters.CreatedAfter.Value);
            }

            if (parameters.CreatedBefore.HasValue)
            {
                query = query.Where(t => t.CreatedAt <= parameters.CreatedBefore.Value);
            }

            if (!string.IsNullOrEmpty(parameters.SortBy))
            {
                var sortOrder = parameters.SortOrder?.ToLower() == "asc" ? true : false;

                switch (parameters.SortBy)
                {
                    case "Amount":
                        query = sortOrder ? query.OrderBy(t => t.Amount) : query.OrderByDescending(t => t.Amount);
                        break;
                    case "CreatedAt":
                    default:
                        query = sortOrder ? query.OrderBy(t => t.CreatedAt) : query.OrderByDescending(t => t.CreatedAt);
                        break;
                }
            }

            var skip = (parameters.PageNumber - 1) * parameters.PageSize;
            query = query.Skip(skip).Take(parameters.PageSize);

            var transactions = query;

            return transactions.Any()
                ? Result<IEnumerable<CardTransaction>>.Success(transactions)
                : Result<IEnumerable<CardTransaction>>.Failure("No transactions found.");
        }

        public async Task<Result<int>> GetCardTransactionTotalCountAsync(CardTransactionQueryParameters filterParameters)
        {
            var transactions = await _cardTransactionRepository.GetAllCardTransactionsAsync();

            if (filterParameters.CardId > 0)
            {
                transactions = transactions.Where(t => t.Card.Id == filterParameters.CardId);
            }

            if (!string.IsNullOrEmpty(filterParameters.CardNumber))
            {
                transactions = transactions.Where(t => t.Card.CardNumber == filterParameters.CardNumber);
            }

            if (!string.IsNullOrEmpty(filterParameters.Wallet))
            {
                transactions = transactions.Where(t => t.Wallet.Name == filterParameters.Wallet);
            }

            if (filterParameters.Amount > 0)
            {
                transactions = transactions.Where(t => t.Amount == filterParameters.Amount);
            }

            if (filterParameters.CreatedAfter.HasValue)
            {
                transactions = transactions.Where(t => t.CreatedAt >= filterParameters.CreatedAfter.Value);
            }

            if (filterParameters.CreatedBefore.HasValue)
            {
                transactions = transactions.Where(t => t.CreatedAt <= filterParameters.CreatedBefore.Value);
            }

            var count = transactions.Count();

            return count != 0
                ? Result<int>.Success(count)
                : Result<int>.Failure("No transactions found.");
        }

    }
}
