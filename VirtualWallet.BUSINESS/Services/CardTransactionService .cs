using VirtualWallet.BUSINESS.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Repositories.Contracts;
using VirtualWallet.BUSINESS.Resources;

namespace VirtualWallet.BUSINESS.Services
{
    public class CardTransactionService : ICardTransactionService
    {
        private readonly ICardTransactionRepository _cardTransactionRepository;
        private readonly ICardRepository _cardRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly ITransactionHandlingService _transactionHandlingService;

        public CardTransactionService(
            ICardTransactionRepository cardTransactionRepository,
            ICardRepository cardRepository,
            IWalletRepository walletRepository,
            ITransactionHandlingService transactionHandlingService)
        {
            _cardTransactionRepository = cardTransactionRepository;
            _cardRepository = cardRepository;
            _walletRepository = walletRepository;
            _transactionHandlingService = transactionHandlingService;
        }

        public async Task<CardTransaction> DepositAsync(int cardId, int walletId, decimal amount)
        {
            var card = await _cardRepository.GetCardByIdAsync(cardId);
            var wallet = await _walletRepository.GetWalletByIdAsync(walletId);

            if (card == null)
                throw new EntityNotFoundException(ErrorMessages.CardNotFound);

            if (wallet == null)
                throw new EntityNotFoundException(ErrorMessages.WalletNotFound);

            if (amount <= 0)
                throw new BadRequestException(ErrorMessages.InvalidDepositAmount);

            return await _transactionHandlingService.ProcessCardToWalletTransactionAsync(card, wallet, amount);
        }

        public async Task<CardTransaction> WithdrawAsync(int walletId, int cardId, decimal amount)
        {
            var wallet = await _walletRepository.GetWalletByIdAsync(walletId);
            var card = await _cardRepository.GetCardByIdAsync(cardId);

            if (wallet == null)
                throw new EntityNotFoundException(ErrorMessages.WalletNotFound);

            if (card == null)
                throw new EntityNotFoundException(ErrorMessages.CardNotFound);

            if (amount <= 0)
                throw new BadRequestException(ErrorMessages.InvalidWithdrawalAmount);

            if (wallet.Balance < amount)
                throw new BadRequestException(ErrorMessages.InsufficientWalletFunds);

            return await _transactionHandlingService.ProcessWalletToCardTransactionAsync(wallet, card, amount);
        }
    }
}
