using ForumProject.Exceptions;
using System.Threading.Tasks;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Repositories.Contracts;

namespace VirtualWallet.BUSINESS.Services
{
    public class PaymentProcessorService : IPaymentProcessorService
    {
        private readonly IRealCardRepository _realCardRepository;
        private readonly ICardTransactionService _cardTransactionService;

        public PaymentProcessorService(
            IRealCardRepository realCardRepository,
            ICardTransactionService cardTransactionService)
        {
            _realCardRepository = realCardRepository;
            _cardTransactionService = cardTransactionService;
        }

        public async Task<string> VerifyAndRetrieveTokenAsync(Card card)
        {
            var realCard = await _realCardRepository.GetByCardNumberAsync(card.CardNumber);

            if (realCard == null)
                throw new EntityNotFoundException("Real card not found.");

            if (realCard.CardHolderName != card.CardHolderName)
                throw new InvalidOperationException("Cardholder name does not match.");

            if (realCard.CheckNumber != card.CheckNumber)
                throw new InvalidOperationException("CVV number does not match.");

            return realCard.PaymentProcessorToken;
        }

        public async Task<bool> ProcessDepositAsync(string paymentProcessorToken, int cardId, int walletId, decimal amount)
        {
            var realCard = await _realCardRepository.GetByPaymentProcessorTokenAsync(paymentProcessorToken);

            if (realCard == null)
                throw new EntityNotFoundException("`Real` card associated with this payment processor token not found.");

            if (realCard.Balance < amount)
                throw new BadRequestException("Insufficient funds in the `real` card.");

            var isSuccess = SimulateExternalDeposit(realCard, amount);

            if (!isSuccess)
                throw new Exception("External payment processor failed to process the deposit.");

            await _cardTransactionService.DepositAsync(cardId, walletId, amount);

            return true;
        }

        public async Task<bool> ProcessWithdrawalAsync(string paymentProcessorToken, int walletId, int cardId, decimal amount)
        {

            var realCard = await _realCardRepository.GetByPaymentProcessorTokenAsync(paymentProcessorToken);

            if (realCard == null)
                throw new EntityNotFoundException("`Real` card associated with this payment processor token not found.");

            var isSuccess = SimulateExternalWithdrawal(realCard, amount);

            if (!isSuccess)
                throw new Exception("External payment processor failed to process the withdrawal.");

            await _cardTransactionService.WithdrawAsync(walletId, cardId, amount);

            return true;
        }

        private bool SimulateExternalDeposit(RealCard realCard, decimal amount)
        {
            realCard.Balance -= amount;
            _realCardRepository.UpdateRealCardAsync(realCard);
            return true;
        }

        private bool SimulateExternalWithdrawal(RealCard realCard, decimal amount)
        {
            realCard.Balance += amount;
            _realCardRepository.UpdateRealCardAsync(realCard);
            return true;
        }
    }
}
