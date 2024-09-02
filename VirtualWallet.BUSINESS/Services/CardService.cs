using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using VirtualWallet.BUSINESS.Resources;
using VirtualWallet.BUSINESS.Results;
using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.DATA.Services.Contracts;

namespace VirtualWallet.BUSINESS.Services
{
    public class CardService : ICardService
    {
        private readonly ICardRepository _cardRepository;
        private readonly ICardTransactionRepository _cardTransactionRepository;
        private readonly IWalletService _walletService;

        public CardService(
            ICardRepository cardRepository,
            ICardTransactionRepository cardTransactionRepository,
            IWalletService walletService)
        {
            _cardRepository = cardRepository;
            _cardTransactionRepository = cardTransactionRepository;
            _walletService = walletService;
        }

        public async Task<Result<Card>> GetCardByIdAsync(int cardId)
        {
            Card card = await _cardRepository.GetCardByIdAsync(cardId);
            return card != null
                ? Result<Card>.Success(card)
                : Result<Card>.Failure(ErrorMessages.CardNotFound);
        }



        public async Task<Result<IEnumerable<Card>>> GetUserCardsAsync(int userId)
        {
            List<Card> cards = await _cardRepository.GetCardsByUserId(userId).ToListAsync();
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

            var cardResult = _cardRepository.GetCardByTokenAsync(card.PaymentProcessorToken);
            if (cardResult == null)
            {
                return Result.Failure("This card is already added to our system.");
            }

            if (!user.MainWalletId.HasValue)
            {
                Wallet wallet = new Wallet
                {
                    Name = "Main Wallet",
                    WalletType = WalletType.Main,
                    UserId = user.Id,
                    Currency = card.Currency
                };

                Result<int> walletResult = await _walletService.AddWalletAsync(wallet);
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
            Result<Card> cardResult = await GetCardByIdAsync(cardId);
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

            Result<Card> cardResult = await GetCardByIdAsync(card.Id);
            if (!cardResult.IsSuccess)
            {
                return Result.Failure(ErrorMessages.CardNotFound);
            }

            await _cardRepository.UpdateCardAsync(card);
            return Result.Success();
        }

        public async Task<Result<IEnumerable<CardTransaction>>> GetCardTransactionsByUserIdAsync(int userId)
        {
            List<CardTransaction> transactions = await _cardTransactionRepository.GetTransactionsByUserId(userId).ToListAsync();
            return transactions.Any()
                ? Result<IEnumerable<CardTransaction>>.Success(transactions)
                : Result<IEnumerable<CardTransaction>>.Failure("No Transactions found");
        }

        public async Task<Result<IEnumerable<CardTransaction>>> FilterByAsync(CardTransactionQueryParameters filterParameters, int? userid = null)
        {
            ICollection<CardTransaction> result = await _cardRepository.FilterByAsync(filterParameters, userid);
            return result.Any()
                ? Result<IEnumerable<CardTransaction>>.Success(result)
                : Result<IEnumerable<CardTransaction>>.Failure("No Transactions found");
        }

        public async Task<Result<int>> GetTotalCountAsync(CardTransactionQueryParameters filterParameters, int? userId = null)
        {
            int result = await _cardRepository.GetTotalCountAsync(filterParameters, userId);
            return result != 0
                ? Result<int>.Success(result)
                : Result<int>.Failure("No Transactions found");
        }

    }
}
