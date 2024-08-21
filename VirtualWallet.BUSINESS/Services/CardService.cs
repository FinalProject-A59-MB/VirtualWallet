using System.Threading.Tasks;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Repositories.Contracts;
using System;
using VirtualWallet.BUSINESS.Exceptions;
using Microsoft.EntityFrameworkCore;
using VirtualWallet.BUSINESS.Resources;

namespace VirtualWallet.BUSINESS.Services
{
    public class CardService : ICardService
    {
        private readonly ICardRepository _cardRepository;
        private readonly IPaymentProcessorService _paymentProcessorService;

        public CardService(ICardRepository cardRepository, IPaymentProcessorService paymentProcessorService)
        {
            _cardRepository = cardRepository;
            _paymentProcessorService = paymentProcessorService;
        }

        public async Task<Card> GetCardByIdAsync(int cardId)
        {
            var card = await _cardRepository.GetCardByIdAsync(cardId);
            if (card == null) throw new EntityNotFoundException(ErrorMessages.CardNotFound);
            return card;
        }

        public async Task<IEnumerable<Card>> GetUserCardsAsync(int userId)
        {
            return await _cardRepository.GetCardsByUserId(userId).ToListAsync();
        }

        public async Task AddCardAsync(User user, Card card)
        {
            user.Cards.Add(card);
            await _cardRepository.AddCardAsync(card);
        }

        public async Task DeleteCardAsync(int cardId)
        {
            var card = await _cardRepository.GetCardByIdAsync(cardId);
            if (card == null) throw new EntityNotFoundException(ErrorMessages.CardNotFound);
            await _cardRepository.RemoveCardAsync(cardId);
        }

        public async Task UpdateCardAsync(Card card)
        {
            await _cardRepository.UpdateCardAsync(card);
        }
    }
}
