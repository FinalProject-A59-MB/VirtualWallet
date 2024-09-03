using Microsoft.EntityFrameworkCore;
using VirtualWallet.BUSINESS.Helpers;
using VirtualWallet.BUSINESS.Resources;
using VirtualWallet.BUSINESS.Results;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.DATA.Repositories.Contracts;
using VirtualWallet.DATA.Services.Contracts;

namespace VirtualWallet.DATA.Services
{
    public class WalletTransactionService : IWalletTransactionService
    {
        private readonly IWalletTransactionRepository _walletTransactionRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly ITransactionHandlingService _transactionHandlingService;
        private readonly ICurrencyService _currencyService;

        public WalletTransactionService(
            IWalletTransactionRepository walletTransactionRepository,
            IWalletRepository walletRepository,
            ITransactionHandlingService transactionHandlingService,
            ICurrencyService currencyService)
        {
            _walletTransactionRepository = walletTransactionRepository;
            _walletRepository = walletRepository;
            _transactionHandlingService = transactionHandlingService;
            _currencyService = currencyService;
        }

        public async Task<Result<WalletTransaction>> VerifySendAmountAsync(int senderWalletId, User recepient, decimal amount)
        {
            var senderWallet = await _walletRepository.GetWalletByIdAsync(senderWalletId);

            if (senderWallet == null)
                return Result<WalletTransaction>.Failure(ErrorMessages.WalletNotFound);
            if (senderWallet.Balance < amount)
            {
                return Result<WalletTransaction>.Failure("Not enough funds in the wallet to complete the transaction.");
            }

            var recipientWallets = await _walletRepository.GetWalletsByUserIdAsync(recepient.Id);

            if (recipientWallets.Count()==0)
                return Result<WalletTransaction>.Failure("User has no active wallets and cannot receive funds.");

            var recipientWallet = recipientWallets.Where(w => w.Currency == senderWallet.Currency).FirstOrDefault();

            if (recipientWallet == null)
            {
                if (recepient.MainWallet == null)
                {
                    recipientWallet = recipientWallets.FirstOrDefault();
                }
                else
                {
                    recipientWallet = recepient.MainWallet;
                }

                
            }
            recipientWallet.User = recepient;

            var sentAmount = await _currencyService.ConvertCurrencyAsync(amount, senderWallet.Currency, recipientWallet.Currency);
            if (!sentAmount.IsSuccess)
            {
                return Result<WalletTransaction>.Failure(sentAmount.Error);
            }

            var verificationCode = VerificationCode.Generate();

            WalletTransaction transaction = new WalletTransaction
            {
                AmountSent = sentAmount.Value,
                AmountReceived = amount,
                Recipient = recipientWallet,
                Sender = senderWallet,
                VerificationCode = verificationCode,
                CreatedAt = DateTime.UtcNow,
                Status = TransactionStatus.Pending,
                CurrencySent = senderWallet.Currency,
                CurrencyReceived = recipientWallet.Currency,
            };

            await _walletTransactionRepository.AddWalletTransactionAsync(transaction);
            //send verification code
            //var emailResult = await _emailService.SendPaymentVerificationEmailAsync(senderWallet.User,transaction.VerificationCode);
            //if (!emailResult.IsSuccess)
            //{
            //    return Result<WalletTransaction>.Failure(emailResult.Error);
            //}

            return Result<WalletTransaction>.Success(transaction);

        }

        public async Task<Result<WalletTransaction>> ProcessSendAmountAsync(WalletTransaction transaction)
        {
            var sender = await _walletRepository.GetWalletByIdAsync(transaction.SenderId);
            var receiver = await _walletRepository.GetWalletByIdAsync(transaction.RecipientId);
            transaction.Sender = sender;
            transaction.Recipient = receiver;
            var completedTransacctions = await _transactionHandlingService.ProcessWalletToWalletTransactionAsync(transaction);
            if (!completedTransacctions.IsSuccess)
            {
                return Result<WalletTransaction>.Failure(completedTransacctions.Error);
            }

            //set status completed

            // send notification to recipient (optional)

            return Result<WalletTransaction>.Success(completedTransacctions.Value);
        }

        public async Task<Result<WalletTransaction>> ProcessSendAmountInternalAsync(int senderWalletId, int recepientWalletId, decimal amount)
        {
            var senderWallet = await _walletRepository.GetWalletByIdAsync(senderWalletId);
            var receiverWallet = await _walletRepository.GetWalletByIdAsync(recepientWalletId);

            var sentAmount = await _currencyService.ConvertCurrencyAsync(amount, senderWallet.Currency, receiverWallet.Currency);
            if (!sentAmount.IsSuccess)
            {
                return Result<WalletTransaction>.Failure(sentAmount.Error);
            }

            var verificationCode = VerificationCode.Generate();

            WalletTransaction transaction = new WalletTransaction
            {
                AmountSent = sentAmount.Value,
                AmountReceived = amount,
                Recipient = receiverWallet,
                Sender = senderWallet,
                VerificationCode = verificationCode,
                CreatedAt = DateTime.UtcNow,
                Status = TransactionStatus.Completed,
                CurrencySent = receiverWallet.Currency,
                CurrencyReceived = senderWallet.Currency,
            };
            await _walletTransactionRepository.AddWalletTransactionAsync(transaction);
            //var completedTransacctions = await _transactionHandlingService.ProcessWalletToWalletTransactionAsync(transaction);
            //if (!completedTransacctions.IsSuccess)
            //{
            //    return Result<WalletTransaction>.Failure(completedTransacctions.Error);
            //}
            //add tranasaction to repository


            return Result<WalletTransaction>.Success(transaction);

        }

        public async Task<Result> UpdateTransaction(WalletTransaction transaction)
        {
            await _walletTransactionRepository.UpdateWalletTransactionAsync(transaction);
            return Result<WalletTransaction>.Success(transaction);
        }

        public async Task<Result<WalletTransaction>> GetTransactionByIdAsync(int id)
        {
            var transaction = await _walletTransactionRepository.GetTransactionByIdAsync(id);

            return transaction != null
                ? Result<WalletTransaction>.Success(transaction)
                : Result<WalletTransaction>.Failure(ErrorMessages.InvalidWalletInformation);
        }

        public async Task<Result<IEnumerable<WalletTransaction>>> GetTransactionsByRecipientIdAsync(int recipientId)
        {
            var transactions = await _walletTransactionRepository.GetTransactionsByRecipientIdAsync(recipientId);

            return transactions != null
                ? Result<IEnumerable<WalletTransaction>>.Success(transactions)
                : Result<IEnumerable<WalletTransaction>>.Failure(ErrorMessages.InvalidWalletInformation);
        }

        public async Task<Result<IEnumerable<WalletTransaction>>> GetTransactionsBySenderIdAsync(int senderId)
        {
            var transactions = await _walletTransactionRepository.GetTransactionsBySenderIdAsync(senderId);

            return transactions != null
                ? Result<IEnumerable<WalletTransaction>>.Success(transactions)
                : Result<IEnumerable<WalletTransaction>>.Failure(ErrorMessages.InvalidWalletInformation);
        }

        public async Task<Result<IEnumerable<WalletTransaction>>> FilterWalletTransactionsAsync(TransactionQueryParameters parameters)
        {
            var query = _walletTransactionRepository.FilterWalletTransactions(parameters);

            var skip = (parameters.PageNumber - 1) * parameters.PageSize;
            query = query.Skip(skip).Take(parameters.PageSize);

            var transactions = await query.ToListAsync();

            return transactions.Any()
                ? Result<IEnumerable<WalletTransaction>>.Success(transactions)
                : Result<IEnumerable<WalletTransaction>>.Failure("No transactions found.");
        }


        public async Task<Result<int>> GetTotalCountAsync(TransactionQueryParameters filterParameters)
        {
            var transactions = await _walletTransactionRepository.GetAllWalletTransactionsAsync();

            if (!string.IsNullOrEmpty(filterParameters.Sender?.Username))
            {
                transactions = transactions.Where(t => t.Sender.Name.Contains(filterParameters.Sender.Username));
            }

            if (!string.IsNullOrEmpty(filterParameters.Recipient?.Username))
            {
                transactions = transactions.Where(t => t.Recipient.Name.Contains(filterParameters.Recipient.Username));
            }

            if (filterParameters.StartDate.HasValue)
            {
                transactions = transactions.Where(t => t.CreatedAt >= filterParameters.StartDate.Value);
            }

            if (filterParameters.EndDate.HasValue)
            {
                transactions = transactions.Where(t => t.CreatedAt <= filterParameters.EndDate.Value);
            }

            if (filterParameters.Direction == "Incoming" && filterParameters.Recipient != null)
            {
                transactions = transactions.Where(t => t.RecipientId == filterParameters.Recipient.Id);
            }
            else if (filterParameters.Direction == "Outgoing" && filterParameters.Sender != null)
            {
                transactions = transactions.Where(t => t.SenderId == filterParameters.Sender.Id);
            }

            var count = transactions.Count();

            return count != 0
                ? Result<int>.Success(count)
                : Result<int>.Failure("No transactions found.");
        }


    }
}
