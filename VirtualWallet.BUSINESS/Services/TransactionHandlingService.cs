using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Repositories.Contracts;
using VirtualWallet.BUSINESS.Services.Contracts;

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

        public TransactionHandlingService(
            ApplicationDbContext context,
            ICardRepository cardRepository,
            IWalletRepository walletRepository,
            ICardTransactionRepository cardTransactionRepository,
            IWalletTransactionRepository walletTransactionRepository,
            IPaymentProcessorService paymentProcessorService)
        {
            _context = context;
            _cardRepository = cardRepository;
            _walletRepository = walletRepository;
            _cardTransactionRepository = cardTransactionRepository;
            _walletTransactionRepository = walletTransactionRepository;
            _paymentProcessorService = paymentProcessorService;
        }

        public async Task<CardTransaction> ProcessCardToWalletTransactionAsync(Card card, Wallet wallet, decimal amount)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Simulate contacting the payment processor to withdraw funds from the real card
                var paymentSuccessful = await _paymentProcessorService.WithdrawFromCardAsync(card.PaymentProcessorToken, amount);
                if (!paymentSuccessful)
                {
                    throw new Exception("Failed to withdraw funds from the real card.");
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


                await _cardTransactionRepository.AddAsync(cardTransaction);
                await _cardRepository.UpdateCardAsync(card);
                await _walletRepository.UpdateAsync(wallet);

                await transaction.CommitAsync();

                return cardTransaction;
            }
            catch (Exception ex)
            {
                // Rollback the transaction
                await transaction.RollbackAsync();

                // Rollback on the payment processor side if necessary
                await _paymentProcessorService.RefundToCardAsync(card.PaymentProcessorToken, amount);

                throw new Exception($"Transaction failed: {ex.Message}");
            }
        }

        public async Task<CardTransaction> ProcessWalletToCardTransactionAsync(Wallet wallet, Card card, decimal amount)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {

                if (wallet.Balance < amount)
                {
                    throw new Exception("Insufficient funds in the wallet.");
                }


                wallet.Balance -= amount;



                var cardTransaction = new CardTransaction
                {
                    WalletId = wallet.Id,
                    CardId = card.Id,
                    Amount = amount,
                    CreatedAt = DateTime.UtcNow,
                    TransactionType = TransactionType.Withdrawal,
                    Status = TransactionStatus.Completed
                };

                // Simulate contacting the payment processor to deposit funds to the real card
                var paymentSuccessful = await _paymentProcessorService.DepositToCardAsync(card.PaymentProcessorToken, amount);
                if (!paymentSuccessful)
                {
                    throw new Exception("Failed to deposit funds to the real card.");
                }

                await _cardTransactionRepository.AddAsync(cardTransaction);
                await _walletRepository.UpdateAsync(wallet);
                await _cardRepository.UpdateAsync(card);

                await transaction.CommitAsync();

                return cardTransaction;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                await _paymentProcessorService.WithdrawFromCardAsync(card.PaymentProcessorToken, amount);

                throw new Exception($"Transaction failed: {ex.Message}");
            }
        }

        public async Task<WalletTransaction> ProcessWalletToWalletTransactionAsync(Wallet senderWallet, Wallet recipientWallet, decimal amount)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (senderWallet.Balance < amount)
                {
                    throw new Exception("Insufficient funds in the sender's wallet.");
                }

                senderWallet.Balance -= amount;
                recipientWallet.Balance += amount;

                var walletTransaction = new WalletTransaction
                {
                    SenderId = senderWallet.OwnerId,
                    RecipientId = recipientWallet.OwnerId,
                    Amount = amount,
                    CreatedAt = DateTime.UtcNow,
                    Status = TransactionStatus.Completed
                };

                await _walletTransactionRepository.AddAsync(walletTransaction);
                await _walletRepository.UpdateAsync(senderWallet);
                await _walletRepository.UpdateAsync(recipientWallet);

                await transaction.CommitAsync();

                return walletTransaction;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                throw new Exception($"Transaction failed: {ex.Message}");
            }
        }
    }
}
