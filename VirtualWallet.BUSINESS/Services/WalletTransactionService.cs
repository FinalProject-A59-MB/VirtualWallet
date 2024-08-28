using VirtualWallet.BUSINESS.Resources;
using VirtualWallet.BUSINESS.Results;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Repositories;
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

        public async Task<Result<WalletTransaction>> DepositAsync(int senderWalletId, int recipientWalletId, decimal amount)
        {
            var senderWallet = await _walletRepository.GetWalletByIdAsync(senderWalletId);

            if (senderWallet == null)
                return Result<WalletTransaction>.Failure(ErrorMessages.WalletNotFound);

            var recipientWallet = await _walletRepository.GetWalletByIdAsync(recipientWalletId);

            if (senderWallet == null)
                return Result<WalletTransaction>.Failure(ErrorMessages.WalletNotFound);

            if (amount <= 0)
                return Result<WalletTransaction>.Failure(ErrorMessages.InvalidDepositAmount);

            var transaction = await _transactionHandlingService.ProcessWalletToWalletTransactionAsync(senderWallet, recipientWallet, amount);
            return Result<WalletTransaction>.Success(transaction.Value);
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
    }
}
