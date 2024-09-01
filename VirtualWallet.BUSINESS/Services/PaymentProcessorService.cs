using VirtualWallet.BUSINESS.Results;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Repositories.Contracts;
using VirtualWallet.BUSINESS.Resources;
using VirtualWallet.DATA.Models.Enums;

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
            RealCard realCard = await _realCardRepository.GetByCardNumberAsync(card.CardNumber);

            if (realCard == null)
                return Result<string>.Failure(ErrorMessages.RealCardNotFound);

            if (realCard.CardHolderName != card.CardHolderName)
                return Result<string>.Failure(ErrorMessages.CardHolderNameMismatch);

            if (realCard.Cvv != card.Cvv)
                return Result<string>.Failure(ErrorMessages.CVVMismatch);

            if (realCard.ExpirationDate != card.ExpirationDate)
                return Result<string>.Failure("The expiration date does not match.");

            return Result<string>.Success(realCard.PaymentProcessorToken);
        }

        public async Task<Result<CurrencyType>> GetCardCurrency(string paymentProcessorToken)
        {
            RealCard realCard = await _realCardRepository.GetByPaymentProcessorTokenAsync(paymentProcessorToken);

            if (realCard == null)
                return Result<CurrencyType>.Failure(ErrorMessages.RealCardNotFound);

            return Result<CurrencyType>.Success(realCard.Currency);
        }

        public async Task<Result> WithdrawFromRealCardAsync(string paymentProcessorToken, decimal amount)
        {
            RealCard realCard = await _realCardRepository.GetByPaymentProcessorTokenAsync(paymentProcessorToken);

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
            RealCard realCard = await _realCardRepository.GetByPaymentProcessorTokenAsync(paymentProcessorToken);

            if (realCard == null)
                return Result.Failure(ErrorMessages.RealCardTokenNotFound);

            realCard.Balance += amount;
            await _realCardRepository.UpdateRealCardAsync(realCard);

            return Result.Success();
        }
    }
}
