using System.Threading.Tasks;
using VirtualWallet.BUSINESS.Results;
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

        public async Task<Result<string>> VerifyAndRetrieveTokenAsync(Card card)
        {
            var realCard = await _realCardRepository.GetByCardNumberAsync(card.CardNumber);

            if (realCard == null)
                return Result<string>.Failure(ErrorMessages.RealCardNotFound);

            if (realCard.CardHolderName != card.CardHolderName)
                return Result<string>.Failure(ErrorMessages.CardHolderNameMismatch);

            if (realCard.Cvv != card.Cvv)
                return Result<string>.Failure(ErrorMessages.CVVMismatch);

            return Result<string>.Success(realCard.PaymentProcessorToken);
        }

        public async Task<Result> WithdrawFromRealCardAsync(string paymentProcessorToken, decimal amount)
        {
            var realCard = await _realCardRepository.GetByPaymentProcessorTokenAsync(paymentProcessorToken);

            if (realCard == null)
                return Result.Failure(ErrorMessages.RealCardTokenNotFound);

            if (realCard.Balance < amount)
                return Result.Failure(ErrorMessages.InsufficientRealCardFunds);

            realCard.Balance -= amount;
            await _realCardRepository.UpdateRealCardAsync(realCard);

            return Result.Success();
        }

        public async Task<Result> DepositToRealCardAsync(string paymentProcessorToken, decimal amount)
        {
            var realCard = await _realCardRepository.GetByPaymentProcessorTokenAsync(paymentProcessorToken);

            if (realCard == null)
                return Result.Failure(ErrorMessages.RealCardTokenNotFound);

            realCard.Balance += amount;
            await _realCardRepository.UpdateRealCardAsync(realCard);

            return Result.Success();
        }
    }
}
