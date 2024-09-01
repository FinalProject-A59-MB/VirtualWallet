using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using VirtualWallet.BUSINESS.Services;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.DATA.Repositories.Contracts;

namespace VirtualWallet.TESTS.BUSINESS.Services.PaymentProcessorServiceTests
{
    [TestClass]
    public class PaymentProcessorServiceTests
    {
        private Mock<IRealCardRepository> _realCardRepositoryMock;
        private PaymentProcessorService _paymentProcessorService;

        [TestInitialize]
        public void SetUp()
        {
            _realCardRepositoryMock = new Mock<IRealCardRepository>();
            _paymentProcessorService = new PaymentProcessorService(_realCardRepositoryMock.Object);
        }

        [TestMethod]
        public async Task VerifyAndRetrieveTokenAsync_ShouldReturnSuccess_WhenCardIsValid()
        {
            // Arrange
            var card = new Card
            {
                CardNumber = "1234567812345678",
                CardHolderName = "John Doe",
                Cvv = "123",
                ExpirationDate = new DateTime(2025, 12, 31)
            };

            // Act
            var result = await _paymentProcessorService.VerifyAndRetrieveTokenAsync(card);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("test-token", result.Value);
        }

        [TestMethod]
        public async Task VerifyAndRetrieveTokenAsync_ShouldReturnFailure_WhenCardHolderNameMismatch()
        {
            // Arrange
            var card = new Card
            {
                CardNumber = "1234567812345678",
                CardHolderName = "Jane Doe", // Name mismatch
                Cvv = "123",
                ExpirationDate = new DateTime(2025, 12, 31)
            };

            // Act
            var result = await _paymentProcessorService.VerifyAndRetrieveTokenAsync(card);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Real card not found.", result.Error);
        }

        [TestMethod]
        public async Task GetCardCurrency_ShouldReturnSuccess_WhenTokenIsValid()
        {
            // Arrange
            var token = "test-token";

            // Act
            var result = await _paymentProcessorService.GetCardCurrency(token);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(CurrencyType.USD, result.Value);
        }

        [TestMethod]
        public async Task GetCardCurrency_ShouldReturnFailure_WhenTokenIsInvalid()
        {
            // Arrange
            var token = "invalid-token";

            // Act
            var result = await _paymentProcessorService.GetCardCurrency(token);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Real card not found.", result.Error);
        }

        [TestMethod]
        public async Task WithdrawFromRealCardAsync_ShouldReturnSuccess_WhenSufficientBalance()
        {
            // Arrange
            var token = "test-token";
            var amount = 100m;

            // Act
            var result = await _paymentProcessorService.WithdrawFromRealCardAsync(token, amount);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            _realCardRepositoryMock.Verify(repo => repo.UpdateRealCardAsync(It.Is<RealCard>(rc => rc.Balance == 900m)), Times.Once);
        }

        [TestMethod]
        public async Task WithdrawFromRealCardAsync_ShouldReturnFailure_WhenInsufficientBalance()
        {
            // Arrange
            var token = "test-token";
            var amount = 2000m;

            // Act
            var result = await _paymentProcessorService.WithdrawFromRealCardAsync(token, amount);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Real card associated with this payment processor token not found.", result.Error);
        }

        [TestMethod]
        public async Task DepositToRealCardAsync_ShouldReturnSuccess_WhenDepositIsSuccessful()
        {
            // Arrange
            var token = "test-token";
            var amount = 100m;

            // Act
            var result = await _paymentProcessorService.DepositToRealCardAsync(token, amount);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            _realCardRepositoryMock.Verify(repo => repo.UpdateRealCardAsync(It.Is<RealCard>(rc => rc.Balance == 1100m)), Times.Once);
        }

        [TestMethod]
        public async Task DepositToRealCardAsync_ShouldReturnFailure_WhenTokenIsInvalid()
        {
            // Arrange
            var token = "invalid-token";
            var amount = 100m;

            // Act
            var result = await _paymentProcessorService.DepositToRealCardAsync(token, amount);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Real card associated with this payment processor token not found.", result.Error);
        }



    }
}
