using VirtualWallet.DATA.Models;

namespace VirtualWallet.DATA.Repositories.Contracts
{
    public interface IRealCardRepository
    {
        Task<RealCard?> GetByPaymentProcessorTokenAsync(string paymentProcessorToken);

        Task<RealCard?> GetByCardNumberAsync(string cardNumber);

        Task UpdateRealCardAsync(RealCard realCard);
    }
}
