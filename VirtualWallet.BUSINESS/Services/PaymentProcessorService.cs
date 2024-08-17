using VirtualWallet.BUSINESS.Exceptions;
using System.Threading.Tasks;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Repositories.Contracts;
using VirtualWallet.BUSINESS.Resources;

namespace VirtualWallet.BUSINESS.Services
{
    public class PaymentProcessorService : IPaymentProcessorService
    {
        private readonly IRealCardRepository _realCardRepository;

        public PaymentProcessorService(IRealCardRepository realCardRepository)
        {
            _realCardRepository = realCardRepository;
        }

        public async Task<string> VerifyAndRetrieveTokenAsync(Card card)
        {
            var realCard = await _realCardRepository.GetByCardNumberAsync(card.CardNumber);

            if (realCard == null)
                throw new EntityNotFoundException(ErrorMessages.RealCardNotFound);

            if (realCard.CardHolderName != card.CardHolderName)
                throw new InvalidOperationException(ErrorMessages.CardHolderNameMismatch);

            if (realCard.CheckNumber != card.CheckNumber)
                throw new InvalidOperationException(ErrorMessages.CVVMismatch);

            return realCard.PaymentProcessorToken;
        }

        public async Task<bool> WithdrawFromRealCardAsync(string paymentProcessorToken, decimal amount)
        {
            var realCard = await _realCardRepository.GetByPaymentProcessorTokenAsync(paymentProcessorToken);

            if (realCard == null)
                throw new EntityNotFoundException(ErrorMessages.RealCardTokenNotFound);

            if (realCard.Balance < amount)
                throw new BadRequestException(ErrorMessages.InsufficientRealCardFunds);

            realCard.Balance -= amount;
            await _realCardRepository.UpdateRealCardAsync(realCard);

            return true;
        }

        public async Task<bool> DepositToRealCardAsync(string paymentProcessorToken, decimal amount)
        {
            var realCard = await _realCardRepository.GetByPaymentProcessorTokenAsync(paymentProcessorToken);

            if (realCard == null)
                throw new EntityNotFoundException(ErrorMessages.RealCardTokenNotFound);

            realCard.Balance += amount;
            await _realCardRepository.UpdateRealCardAsync(realCard);

            return true;
        }
    }
}
