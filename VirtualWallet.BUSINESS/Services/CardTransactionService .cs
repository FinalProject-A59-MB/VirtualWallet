using ForumProject.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.DATA.Repositories.Contracts;

namespace VirtualWallet.BUSINESS.Services
{
    public class CardTransactionService : ICardTransactionService
    {
        private readonly ICardTransactionRepository _cardTransactionRepository;
        private readonly ICardRepository _cardRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly ITransactionHandlingService _transactionService;

        public CardTransactionService(
            ICardTransactionRepository cardTransactionRepository,
            ICardRepository cardRepository,
            IWalletRepository walletRepository,
            ITransactionHandlingService transactionService)
        {
            _cardTransactionRepository = cardTransactionRepository;
            _cardRepository = cardRepository;
            _walletRepository = walletRepository;
            _transactionService = transactionService;
        }

        public async Task<CardTransaction> DepositAsync(int cardId, int walletId, decimal amount)
        {
            var card = await _cardRepository.GetCardByIdAsync(cardId);
            var wallet = await _walletRepository.GetUserByIdAsync(walletId);

            if (card == null)
                throw new EntityNotFoundException($"Card with ID {cardId} not found.");

            if (wallet == null)
                throw new EntityNotFoundException($"Wallet with ID {walletId} not found.");

            if (card.UserId != wallet.OwnerId)
                throw new UnauthorizedAccessException("The card and wallet must belong to the same user.");

            if (amount <= 0)
                throw new BadRequestException("The deposit amount must be greater than zero.");

            return await _transactionService.ProcessCardToWalletTransactionAsync(card, wallet, amount);
        }

        public async Task<CardTransaction> WithdrawAsync(int walletId, int cardId, decimal amount)
        {
            var wallet = await _walletRepository.GetWalletByIdAsync(walletId);
            var card = await _cardRepository.GetCardByIdAsync(cardId);

            if (wallet == null)
                throw new EntityNotFoundException($"Wallet with ID {walletId} not found.");

            if (card == null)
                throw new EntityNotFoundException($"Card with ID {cardId} not found.");

            if (wallet.OwnerId != card.UserId)
                throw new UnauthorizedAccessException("The card and wallet must belong to the same user.");

            if (amount <= 0)
                throw new BadRequestException("The withdrawal amount must be greater than zero.");

            if (wallet.Balance < amount)
                throw new BadRequestException("Insufficient funds in the wallet.");

            return await _transactionService.ProcessWalletToCardTransactionAsync(wallet, card, amount);
        }
    }
}
