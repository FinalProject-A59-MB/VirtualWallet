using VirtualWallet.DATA.Models;

namespace VirtualWallet.DATA.Repositories.Contracts
{
    public interface ICardTransactionRepository
    {
        public IQueryable<CardTransaction> GetTransactionsByUserId(int userId);

        public IQueryable<CardTransaction> GetTransactionsByCardId(int cardId);

        public Task<CardTransaction?> GetTransactionByIdAsync(int id);

        public Task AddCardTransactionAsync(CardTransaction cardTransaction);
    }
}
