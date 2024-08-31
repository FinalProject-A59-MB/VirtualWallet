using Microsoft.EntityFrameworkCore;
using VirtualWallet.BUSINESS.Resources;
using VirtualWallet.BUSINESS.Results;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Repositories.Contracts;
using VirtualWallet.DATA.Services.Contracts;

namespace VirtualWallet.DATA.Services
{
    public class WalletTransactionService : IWalletTransactionService
    {
        private readonly IWalletTransactionRepository _walletTransactionRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly ITransactionHandlingService _transactionHandlingService;

        public WalletTransactionService(IWalletTransactionRepository walletTransactionRepository, 
            IWalletRepository walletRepository,
            ITransactionHandlingService transactionHandlingService)
        {
            _walletTransactionRepository = walletTransactionRepository;
            _walletRepository = walletRepository;
            _transactionHandlingService = transactionHandlingService;
        }

        //Step1
        public async Task<Result<int>> DepositStep1Async(int senderWalletId, int recipientWalletId, decimal amount)
        {
            var senderWallet = await _walletRepository.GetWalletByIdAsync(senderWalletId);

            if (senderWallet == null)
                return Result<int>.Failure(ErrorMessages.WalletNotFound);

            var recipientWallet = await _walletRepository.GetWalletByIdAsync(recipientWalletId);

            if (senderWallet == null)
                return Result<int>.Failure(ErrorMessages.WalletNotFound);

            if (amount <= 0)
                return Result<int>.Failure(ErrorMessages.InvalidDepositAmount);

            return await _transactionHandlingService.ProcessWalletToWalletTransactionStep1Async(senderWallet, recipientWallet, amount);
        }

        //Step2
        public async Task<Result<int>> DepositStep2Async(int senderWalletId, int recipientWalletId, int transactionId)
        {
            var senderWallet = await _walletRepository.GetWalletByIdAsync(senderWalletId);

            if (senderWallet == null)
                return Result<int>.Failure(ErrorMessages.WalletNotFound);

            var recipientWallet = await _walletRepository.GetWalletByIdAsync(recipientWalletId);

            if (senderWallet == null)
                return Result<int>.Failure(ErrorMessages.WalletNotFound);

            WalletTransaction? transactionResult = await _walletTransactionRepository.GetTransactionByIdAsync(transactionId);

            //TODO CHECK WALLETTRASCATION RESULT

            return await _transactionHandlingService.ProcessWalletToWalletTransactionStep2Async(senderWallet, recipientWallet, transactionResult);
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
