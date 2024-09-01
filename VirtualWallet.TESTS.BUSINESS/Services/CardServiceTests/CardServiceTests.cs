using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using VirtualWallet.BUSINESS.Results;
using VirtualWallet.BUSINESS.Services;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Repositories.Contracts;
using VirtualWallet.DATA.Services.Contracts;

namespace VirtualWallet.TESTS.BUSINESS.Services.CardServiceTests
{
    [TestClass]
    public class CardServiceTests
    {
        private Mock<ICardRepository> _cardRepositoryMock;
        private Mock<ICardTransactionRepository> _cardTransactionRepositoryMock;
        private Mock<IWalletService> _walletServiceMock;
        private CardService _cardService;

        [TestInitialize]
        public void SetUp()
        {
            _cardRepositoryMock = new Mock<ICardRepository>();
            _cardTransactionRepositoryMock = new Mock<ICardTransactionRepository>();
            _walletServiceMock = new Mock<IWalletService>();
            _cardService = new CardService(_cardRepositoryMock.Object, _cardTransactionRepositoryMock.Object, _walletServiceMock.Object);
        }

        [TestMethod]
        public async Task GetCardByIdAsync_ShouldReturnCard_WhenCardExists()
        {
            // Arrange
            var cardId = 1;
            var card = TestHelper.GetTestCard();
            _cardRepositoryMock.Setup(repo => repo.GetCardByIdAsync(cardId)).ReturnsAsync(card);

            // Act
            var result = await _cardService.GetCardByIdAsync(cardId);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(card, result.Value);
        }

        [TestMethod]
        public async Task GetCardByIdAsync_ShouldReturnFailure_WhenCardDoesNotExist()
        {
            // Arrange
            var cardId = 1;
            _cardRepositoryMock.Setup(repo => repo.GetCardByIdAsync(cardId)).ReturnsAsync((Card)null);

            // Act
            var result = await _cardService.GetCardByIdAsync(cardId);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Card not found.", result.Error);
        }





        [TestMethod]
        public async Task AddCardAsync_ShouldAddCard_WhenUserHasNoMainWallet()
        {
            // Arrange
            var user = TestHelper.GetTestUser();
            var card = TestHelper.GetTestCard();
            user.MainWalletId = null;

            _walletServiceMock
             .Setup(service => service.AddWalletAsync(It.IsAny<Wallet>()))
             .ReturnsAsync(Result<int>.Success(1));

            _cardRepositoryMock.Setup(repo => repo.AddCardAsync(card)).Returns(Task.CompletedTask);

            // Act
            var result = await _cardService.AddCardAsync(user, card);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            _cardRepositoryMock.Verify(repo => repo.AddCardAsync(card), Times.Once);
        }

        [TestMethod]
        public async Task AddCardAsync_ShouldReturnFailure_WhenInvalidUserOrCardProvided()
        {
            // Arrange
            User user = null;
            Card card = null;

            // Act
            var result = await _cardService.AddCardAsync(user, card);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Invalid user or card information.", result.Error);
        }

        [TestMethod]
        public async Task DeleteCardAsync_ShouldDeleteCard_WhenCardExists()
        {
            // Arrange
            var cardId = 1;
            _cardRepositoryMock.Setup(repo => repo.GetCardByIdAsync(cardId)).ReturnsAsync(TestHelper.GetTestCard());

            // Act
            var result = await _cardService.DeleteCardAsync(cardId);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            _cardRepositoryMock.Verify(repo => repo.RemoveCardAsync(cardId), Times.Once);
        }

        [TestMethod]
        public async Task DeleteCardAsync_ShouldReturnFailure_WhenCardDoesNotExist()
        {
            // Arrange
            var cardId = 1;
            _cardRepositoryMock.Setup(repo => repo.GetCardByIdAsync(cardId)).ReturnsAsync((Card)null);

            // Act
            var result = await _cardService.DeleteCardAsync(cardId);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Card not found.", result.Error);
        }

        [TestMethod]
        public async Task UpdateCardAsync_ShouldUpdateCard_WhenCardExists()
        {
            // Arrange
            var card = TestHelper.GetTestCard();
            _cardRepositoryMock.Setup(repo => repo.GetCardByIdAsync(card.Id)).ReturnsAsync(card);

            // Act
            var result = await _cardService.UpdateCardAsync(card);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            _cardRepositoryMock.Verify(repo => repo.UpdateCardAsync(card), Times.Once);
        }

        [TestMethod]
        public async Task UpdateCardAsync_ShouldReturnFailure_WhenCardDoesNotExist()
        {
            // Arrange
            var card = TestHelper.GetTestCard();
            _cardRepositoryMock.Setup(repo => repo.GetCardByIdAsync(card.Id)).ReturnsAsync((Card)null);

            // Act
            var result = await _cardService.UpdateCardAsync(card);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Card not found.", result.Error);
        }





        [TestMethod]
        public async Task FilterByAsync_ShouldReturnTransactions_WhenTheyMatchFilters()
        {
            // Arrange
            var filterParams = new CardTransactionQueryParameters();
            var transactions = new List<CardTransaction> { TestHelper.GetTestCardTransaction() };
            _cardRepositoryMock.Setup(repo => repo.FilterByAsync(filterParams, null)).ReturnsAsync(transactions);

            // Act
            var result = await _cardService.FilterByAsync(filterParams);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, result.Value.Count());
        }

        //[TestMethod]
        //public async Task FilterByAsync_ShouldReturnFailure_WhenNoTransactionsMatchFilters()
        //{
        //    // Arrange
        //    var filterParams = new CardTransactionQueryParameters();
        //    _cardRepositoryMock.Setup(repo => repo.FilterByAsync(filterParams, null)).Returns(Collection.Empty<CardTransaction>());

        //    // Act
        //    var result = await _cardService.FilterByAsync(filterParams);

        //    // Assert
        //    Assert.IsFalse(result.IsSuccess);
        //    Assert.AreEqual("No Transactions found", result.Error);
        //}





    }
}
