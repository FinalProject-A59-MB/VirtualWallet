using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using VirtualWallet.BUSINESS.Resources;
using VirtualWallet.BUSINESS.Results;
using VirtualWallet.DATA.Repositories;
using Microsoft.AspNetCore.Http;
using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.DATA.Services.Contracts;

namespace VirtualWallet.BUSINESS.Services
{
    public class CardService : ICardService
    {
        private readonly ICardRepository _cardRepository;
        private readonly IPaymentProcessorService _paymentProcessorService;
        private readonly ICardTransactionRepository _cardTransactionRepository;
        private readonly IWalletService _walletService;

        public CardService(
            ICardRepository cardRepository, 
            IPaymentProcessorService paymentProcessorService, 
            ICardTransactionRepository cardTransactionRepository,
            IWalletService walletService)
        {
            _cardRepository = cardRepository;
            _paymentProcessorService = paymentProcessorService;
            _cardTransactionRepository = cardTransactionRepository;
            _walletService = walletService;
        }

        public async Task<Result<Card>> GetCardByIdAsync(int cardId)
        {
            var card = await _cardRepository.GetCardByIdAsync(cardId);
            return card != null
                ? Result<Card>.Success(card)
                : Result<Card>.Failure(ErrorMessages.CardNotFound);
        }

        public async Task<Result<IEnumerable<Card>>> GetUserCardsAsync(int userId)
        {
            var cards = await _cardRepository.GetCardsByUserId(userId).ToListAsync();
            return cards.Any()
                ? Result<IEnumerable<Card>>.Success(cards)
                : Result<IEnumerable<Card>>.Failure(ErrorMessages.NoCardsFound);
        }

        public async Task<Result> AddCardAsync(User user, Card card)
        {
            if (user == null || card == null)
            {
                return Result.Failure("Invalid user or card information.");
            }

            if (!user.MainWalletId.HasValue)
            {
                var wallet = new Wallet
                {
                    Name = "Main Wallet",
                    WalletType = WalletType.Main,
                    UserId = user.Id,
                    Currency = card.Currency
                };

                var walletResult = await _walletService.AddWalletAsync(wallet);
                if (!walletResult.IsSuccess)
                {
                    return Result.Failure("Unable to add main Wallet.");
                }

                user.MainWalletId = wallet.Id;
            }

            user.Cards.Add(card);
            await _cardRepository.AddCardAsync(card);
            return Result.Success();
        }

        public async Task<Result> DeleteCardAsync(int cardId)
        {
            var cardResult = await GetCardByIdAsync(cardId);
            if (!cardResult.IsSuccess)
            {
                return Result.Failure(ErrorMessages.CardNotFound);
            }

            await _cardRepository.RemoveCardAsync(cardId);
            return Result.Success();
        }

        public async Task<Result> UpdateCardAsync(Card card)
        {
            if (card == null)
            {
                return Result.Failure("Invalid card information.");
            }

            var cardResult = await GetCardByIdAsync(card.Id);
            if (!cardResult.IsSuccess)
            {
                return Result.Failure(ErrorMessages.CardNotFound);
            }

            await _cardRepository.UpdateCardAsync(card);
            return Result.Success();
        }

        public async Task<Result<IEnumerable<CardTransaction>>> GetCardTransactionsByUserIdAsync(int userId)
        {
            var transactions = await _cardTransactionRepository.GetTransactionsByUserId(userId).ToListAsync();
            return transactions.Any()
                ? Result<IEnumerable<CardTransaction>>.Success(transactions)
                : Result<IEnumerable<CardTransaction>>.Failure("No Transactions found");
        }

        public async Task<Result<IEnumerable<CardTransaction>>> FilterByAsync(CardTransactionQueryParameters filterParameters,int? userid=null)
        {   var result = await _cardRepository.FilterByAsync(filterParameters,userid);

            return result.Any()
                ? Result<IEnumerable<CardTransaction>>.Success(result)
                : Result<IEnumerable<CardTransaction>>.Failure("No Transactions found");
        }

        public async Task<Result<int>> GetTotalCountAsync(CardTransactionQueryParameters filterParameters, int? userId = null)
        {   var result = await _cardRepository.GetTotalCountAsync(filterParameters,userId);
            return result!=0
                ? Result<int>.Success(result)
                : Result<int>.Failure("No Transactions found");
        }

    }
}
