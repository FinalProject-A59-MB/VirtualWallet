using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Repositories.Contracts;

namespace VirtualWallet.TESTS.BUSINESS.Services.MockRepositories
{
    using Moq;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    namespace VirtualWallet.TESTS.BUSINESS.Services.MockRepositories
    {
        public class MockRealCardRepository
        {
            public Mock<IRealCardRepository> GetMockRepository()
            {
                var mockRepository = new Mock<IRealCardRepository>();

                // Sample data for testing
                var sampleRealCards = new List<RealCard>
            {
                new RealCard
                {
                    CardNumber = "1234567812345678",
                    CardHolderName = "John Doe",
                    Cvv = "123",
                    ExpirationDate = new DateTime(2025, 12, 31),
                    PaymentProcessorToken = "test-token",
                    Currency = CurrencyType.USD,
                    Balance = 1000m
                }
            };

                // Setup for GetByPaymentProcessorTokenAsync
                mockRepository.Setup(x => x.GetByPaymentProcessorTokenAsync(It.IsAny<string>()))
                    .ReturnsAsync((string token) => sampleRealCards.FirstOrDefault(rc => rc.PaymentProcessorToken == token));

                // Setup for GetByCardNumberAsync
                mockRepository.Setup(x => x.GetByCardNumberAsync(It.IsAny<string>()))
                    .ReturnsAsync((string cardNumber) => sampleRealCards.FirstOrDefault(rc => rc.CardNumber == cardNumber));

                // Setup for UpdateRealCardAsync
                mockRepository.Setup(x => x.UpdateRealCardAsync(It.IsAny<RealCard>()))
                    .Callback((RealCard realCard) =>
                    {
                        var existingCard = sampleRealCards.FirstOrDefault(rc => rc.PaymentProcessorToken == realCard.PaymentProcessorToken);
                        if (existingCard != null)
                        {
                            existingCard.Balance = realCard.Balance;
                        }
                    })
                    .Returns(Task.CompletedTask);

                return mockRepository;
            }
        }
    }



}
