using System.Threading.Tasks;
using VirtualWallet.BUSINESS.Results;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Repositories.Contracts;
using VirtualWallet.BUSINESS.Resources;

namespace VirtualWallet.BUSINESS.Services
{
    public class CardTransactionService : ICardTransactionService
    {
        private readonly ICardRepository _cardRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly ITransactionHandlingService _transactionHandlingService;

        public CardTransactionService(
            ICardRepository cardRepository,
            IWalletRepository walletRepository,
            ITransactionHandlingService transactionHandlingService)
        {
            _cardRepository = cardRepository;
            _walletRepository = walletRepository;
            _transactionHandlingService = transactionHandlingService;
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

            if (walletResult.Balance < amount)
                return Result<CardTransaction>.Failure(ErrorMessages.InsufficientWalletFunds);

            var transactionResult = await _transactionHandlingService.ProcessWalletToCardTransactionAsync(walletResult, cardResult, amount);
            return transactionResult;
        }
    }
}
