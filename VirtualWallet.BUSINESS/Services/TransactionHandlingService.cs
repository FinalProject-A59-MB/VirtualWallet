using VirtualWallet.BUSINESS.Results;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.BUSINESS.Services.Responses;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.DATA.Repositories.Contracts;
using VirtualWallet.DATA.Services.Contracts;

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
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        public TransactionHandlingService(
            ApplicationDbContext context,
            ICardRepository cardRepository,
            IWalletRepository walletRepository,
            ICardTransactionRepository cardTransactionRepository,
            IWalletTransactionRepository walletTransactionRepository,
            IPaymentProcessorService paymentProcessorService,
            ICurrencyService currencyService,
            IUserService userService,
            IEmailService emailService)
        {
            _context = context;
            _cardRepository = cardRepository;
            _walletRepository = walletRepository;
            _cardTransactionRepository = cardTransactionRepository;
            _walletTransactionRepository = walletTransactionRepository;
            _paymentProcessorService = paymentProcessorService;
            _currencyService = currencyService;
            _userService = userService;
            _emailService = emailService;
        }

        public async Task<Result<CardTransaction>> ProcessCardToWalletTransactionAsync(Card card, Wallet wallet, decimal amount)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                Result paymentResult = await _paymentProcessorService.WithdrawFromRealCardAsync(card.PaymentProcessorToken, amount);
                if (!paymentResult.IsSuccess)
                {
                    return Result<CardTransaction>.Failure("Failed to withdraw funds from the real card.");
                }

                wallet.Balance += amount;

                CardTransaction cardTransaction = new CardTransaction
                {
                    User = card.User,
                    UserId = card.UserId,
                    CardId = card.Id,
                    WalletId = wallet.Id,
                    Amount = amount,
                    CreatedAt = DateTime.UtcNow,
                    TransactionType = TransactionType.Deposit,
                    Status = TransactionStatus.Completed,
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

        public async Task<Result<CardTransaction>> ProcessWalletToCardTransactionAsync(Wallet wallet, Card card, decimal amount, decimal feeAmmount)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (wallet.Balance < amount + feeAmmount)
                {
                    return Result<CardTransaction>.Failure("Insufficient funds in the wallet.");
                }

                wallet.Balance -= amount + feeAmmount;

                Result paymentResult = await _paymentProcessorService.DepositToRealCardAsync(card.PaymentProcessorToken, amount);
                if (!paymentResult.IsSuccess)
                {
                    return Result<CardTransaction>.Failure("Failed to deposit funds to the real card.");
                }

                CardTransaction cardTransaction = new CardTransaction
                {
                    UserId = card.UserId,
                    User = card.User,
                    WalletId = wallet.Id,
                    CardId = card.Id,
                    Amount = amount,
                    CreatedAt = DateTime.UtcNow,
                    TransactionType = TransactionType.Withdrawal,
                    Status = TransactionStatus.Completed,
                    Fee = feeAmmount,

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

        public async Task<Result<int>> ProcessWalletToWalletTransactionStep1Async(Wallet senderWallet, Wallet recipientWallet, decimal amount)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                decimal recipientAmount = amount;

                if (recipientWallet.Currency != senderWallet.Currency)
                {
                    Result<CurrencyExchangeRatesResponse> result = await _currencyService.GetRatesForCurrencyAsync(senderWallet.Currency);
                    if (!result.IsSuccess)
                    {
                        return Result<int>.Failure(result.Error);
                    }
                    CurrencyExchangeRatesResponse rates = result.Value;

                    decimal rate = GetRate(rates, recipientWallet.Currency);

                    recipientAmount = amount * rate;
                }

                WalletTransaction walletTransaction = new WalletTransaction
                {
                    SenderId = senderWallet.Id,
                    RecipientId = recipientWallet.Id,
                    CreatedAt = DateTime.UtcNow,
                    Status = TransactionStatus.Pending,
                    Currency = recipientWallet.Currency,
                    WithdrownAmount = amount,
                    DepositedAmount = recipientAmount,
                    VerificationCode = GenerateVerificationCode()
                };

                int transactionId = await _walletTransactionRepository.AddWalletTransactionAsync(walletTransaction);

                _context.SaveChanges();

                await transaction.CommitAsync();

                Result<User> userResult = await _userService.GetUserByIdAsync(recipientWallet.UserId);

                if (!userResult.IsSuccess)
                {
                    throw new Exception(userResult.Error);
                }

                Result emailResult = await _emailService.SendPaymentVerificationEmailAsync(userResult.Value, walletTransaction.VerificationCode);

                if (!emailResult.IsSuccess)
                {
                    throw new Exception(emailResult.Error);
                }

                return Result<int>.Success(transactionId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return Result<int>.Failure($"Transaction failed: {ex.Message}");
            }
        }

        public async Task<Result<int>> ProcessWalletToWalletTransactionStep2Async(Wallet senderWallet, Wallet recipientWallet, WalletTransaction walletTransaction)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                senderWallet.Balance -= walletTransaction.WithdrownAmount;

                recipientWallet.Balance += walletTransaction.DepositedAmount;

                walletTransaction.Status = TransactionStatus.Completed;

                _context.SaveChanges();

                await transaction.CommitAsync();

                Result<User> userResult = await _userService.GetUserByIdAsync(recipientWallet.UserId);

                if (!userResult.IsSuccess)
                {
                    throw new Exception(userResult.Error);
                }

                Result emailResult = await _emailService.SendEmailAsync(
                    userResult.Value.Email,
                    "Money Deposit",
                    $"You have successfully received {walletTransaction.DepositedAmount} {walletTransaction.Currency} from {senderWallet.Name}");

                if (!emailResult.IsSuccess)
                {
                    throw new Exception(emailResult.Error);
                }

                return Result<int>.Success(walletTransaction.Id);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return Result<int>.Failure($"Transaction failed: {ex.Message}");
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

        private string GenerateVerificationCode()
        {
            Random random = new Random();
            int verificationCode = random.Next(1000, 10000); // Generates a random number between 1000 and 9999
            return verificationCode.ToString();
        }
    }
}
