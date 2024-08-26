using VirtualWallet.BUSINESS.Results;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.BUSINESS.Services.Responses;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.DATA.Repositories.Contracts;

namespace VirtualWallet.BUSINESS.Services
{
    public class TransactionHandlingService : ITransactionHandlingService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICardRepository _cardRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly ICardTransactionRepository _cardTransactionRepository;
        private readonly IWalletTransactionRepository _walletTransactionRepository;
        private readonly IPaymentProcessorService _paymentProcessorService;
        private readonly ICurrencyService _currencyService;

        public TransactionHandlingService(
            ApplicationDbContext context,
            ICardRepository cardRepository,
            IWalletRepository walletRepository,
            ICardTransactionRepository cardTransactionRepository,
            IWalletTransactionRepository walletTransactionRepository,
            IPaymentProcessorService paymentProcessorService, 
            ICurrencyService currencyService)
        {
            _context = context;
            _cardRepository = cardRepository;
            _walletRepository = walletRepository;
            _cardTransactionRepository = cardTransactionRepository;
            _walletTransactionRepository = walletTransactionRepository;
            _paymentProcessorService = paymentProcessorService;
            _currencyService = currencyService;
        }

        public async Task<Result<CardTransaction>> ProcessCardToWalletTransactionAsync(Card card, Wallet wallet, decimal amount)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var paymentResult = await _paymentProcessorService.WithdrawFromRealCardAsync(card.PaymentProcessorToken, amount);
                if (!paymentResult.IsSuccess)
                {
                    return Result<CardTransaction>.Failure("Failed to withdraw funds from the real card.");
                }

                wallet.Balance += amount;

                var cardTransaction = new CardTransaction
                {
                    CardId = card.Id,
                    WalletId = wallet.Id,
                    Amount = amount,
                    CreatedAt = DateTime.UtcNow,
                    TransactionType = TransactionType.Deposit,
                    Status = TransactionStatus.Completed
                };

                await _cardTransactionRepository.AddCardTransactionAsync(cardTransaction);
                await _walletRepository.UpdateWalletAsync(wallet);

                await transaction.CommitAsync();

                return Result<CardTransaction>.Success(cardTransaction);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                // Refund the amount to the real card if transaction fails
                await _paymentProcessorService.DepositToRealCardAsync(card.PaymentProcessorToken, amount);

                return Result<CardTransaction>.Failure($"Transaction failed: {ex.Message}");
            }
        }

        public async Task<Result<CardTransaction>> ProcessWalletToCardTransactionAsync(Wallet wallet, Card card, decimal amount)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (wallet.Balance < amount)
                {
                    return Result<CardTransaction>.Failure("Insufficient funds in the wallet.");
                }

                wallet.Balance -= amount;

                var paymentResult = await _paymentProcessorService.DepositToRealCardAsync(card.PaymentProcessorToken, amount);
                if (!paymentResult.IsSuccess)
                {
                    return Result<CardTransaction>.Failure("Failed to deposit funds to the real card.");
                }

                var cardTransaction = new CardTransaction
                {
                    WalletId = wallet.Id,
                    CardId = card.Id,
                    Amount = amount,
                    CreatedAt = DateTime.UtcNow,
                    TransactionType = TransactionType.Withdrawal,
                    Status = TransactionStatus.Completed
                };

                await _cardTransactionRepository.AddCardTransactionAsync(cardTransaction);
                await _walletRepository.UpdateWalletAsync(wallet);

                await transaction.CommitAsync();

                return Result<CardTransaction>.Success(cardTransaction);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                // Refund the amount to the wallet if transaction fails
                await _paymentProcessorService.WithdrawFromRealCardAsync(card.PaymentProcessorToken, amount);

                return Result<CardTransaction>.Failure($"Transaction failed: {ex.Message}");
            }
        }

        public async Task<Result<WalletTransaction>> ProcessWalletToWalletTransactionAsync(Wallet senderWallet, Wallet recipientWallet, decimal amount)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                senderWallet.Balance -= amount;

                decimal recipientAmount = amount;

                if(recipientWallet.Currency != senderWallet.Currency)
                {
                    CurrencyExchangeRatesResponse rates = await _currencyService.GetRatesForCurrencyAsync(senderWallet.Currency);

                    var rate = GetRate(rates, recipientWallet.Currency);

                    recipientAmount = amount * rate;
                }

                recipientWallet.Balance += recipientAmount;

                var walletTransaction = new WalletTransaction
                {
                    SenderId = senderWallet.UserId,
                    RecipientId = recipientWallet.UserId,
                    CreatedAt = DateTime.UtcNow,
                    Status = TransactionStatus.Completed,
                    Currency = recipientWallet.Currency,
                    Amount = recipientAmount,
                };

                await _walletTransactionRepository.AddWalletTransactionAsync(walletTransaction);
                await _walletRepository.UpdateWalletAsync(senderWallet);
                await _walletRepository.UpdateWalletAsync(recipientWallet);

                await transaction.CommitAsync();

                return Result<WalletTransaction>.Success(walletTransaction);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return Result<WalletTransaction>.Failure($"Transaction failed: {ex.Message}");
            }
        }

        private decimal GetRate(CurrencyExchangeRatesResponse response, CurrencyType currencyType)
        {
            string currencyKey = currencyType.ToString(); 

            if (response.Data.TryGetValue(currencyKey, out decimal rate))
            {
                return rate;
            }

            throw new Exception("Currency not found!");
        }
    }
}
