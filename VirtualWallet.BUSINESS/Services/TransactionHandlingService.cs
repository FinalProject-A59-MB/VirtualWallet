using Microsoft.AspNetCore.Cors.Infrastructure;
using VirtualWallet.BUSINESS.Results;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.BUSINESS.Services.Responses;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.DATA.Repositories.Contracts;
using VirtualWallet.DATA.Services;
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

            Result paymentResult = await _paymentProcessorService.WithdrawFromRealCardAsync(card.PaymentProcessorToken, amount);
            if (!paymentResult.IsSuccess)
            {
                return Result<CardTransaction>.Failure("Failed to withdraw funds from the real card.");
            }

            try
            {
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

                return Result<CardTransaction>.Failure($"Transaction failed: {ex.Message}");
            }
        }

        public async Task<Result<WalletTransaction>> ProcessWalletToWalletTransactionAsync(WalletTransaction transactionToProcess)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {

                transactionToProcess.Sender.Balance -= transactionToProcess.AmountSent;
                transactionToProcess.Recipient.Balance += transactionToProcess.AmountReceived;
                await _walletRepository.UpdateWalletAsync(transactionToProcess.Sender);
                await _walletRepository.UpdateWalletAsync(transactionToProcess.Recipient);

                transactionToProcess.Status = TransactionStatus.Completed;
                transactionToProcess.CreatedAt = DateTime.UtcNow;
                await _walletTransactionRepository.UpdateWalletTransactionAsync(transactionToProcess);

                await transaction.CommitAsync();

                return Result<WalletTransaction>.Success(transactionToProcess);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return Result<WalletTransaction>.Failure($"Transaction failed: {ex.Message}");
            }
        }
    }
}
