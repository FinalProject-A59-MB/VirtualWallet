using VirtualWallet.DATA.Models;

namespace VirtualWallet.DATA.Repositories.Contracts
{
    public interface ICardRepository
    {
        public IQueryable<Card> GetCardsByUserId(int userId);

        public Task<Card?> GetCardByIdAsync(int id);

        public Task AddCardAsync(Card card);

        public Task UpdateCardAsync(Card card);

        public Task RemoveCardAsync(int cardId);

        Task<ICollection<CardTransaction>> FilterByAsync(CardTransactionQueryParameters filterParameters, int? userId = null);


        Task<int> GetTotalCountAsync(CardTransactionQueryParameters filterParameters, int? userId = null);

        Task<Card?> GetCardByTokenAsync(string token);

    }
}
