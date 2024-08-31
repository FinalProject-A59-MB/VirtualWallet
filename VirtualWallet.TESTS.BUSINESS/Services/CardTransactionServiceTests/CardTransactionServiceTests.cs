using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualWallet.BUSINESS.Results;
using VirtualWallet.BUSINESS.Services;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.DATA.Repositories.Contracts;

namespace VirtualWallet.TESTS.BUSINESS.Services.CardTransactionServiceTests
{
    [TestClass]
    public class CardTransactionServiceTests
    {
        private Mock<ICardRepository> _cardRepositoryMock;
        private Mock<ICardTransactionRepository> _cardTransactionRepositoryMock;
        private Mock<IWalletRepository> _walletRepositoryMock;
        private Mock<ITransactionHandlingService> _transactionHandlingServiceMock;
        private Mock<ICurrencyService> _currencyServiceMock;
        private CardTransactionService _cardTransactionService;

        [TestInitialize]
        public void SetUp()
        {
            _cardRepositoryMock = new Mock<ICardRepository>();
            _cardTransactionRepositoryMock = new Mock<ICardTransactionRepository>();
            _walletRepositoryMock = new Mock<IWalletRepository>();
            _transactionHandlingServiceMock = new Mock<ITransactionHandlingService>();
            _currencyServiceMock = new Mock<ICurrencyService>();

            _cardTransactionService = new CardTransactionService(
                _cardRepositoryMock.Object,
                _cardTransactionRepositoryMock.Object,
                _walletRepositoryMock.Object,
                _transactionHandlingServiceMock.Object,
                _currencyServiceMock.Object
            );
        }

        [TestMethod]
        public async Task DepositAsync_ShouldReturnFailure_WhenCardNotFound()
        {
            // Arrange
            var cardId = 1;
            var walletId = 1;
            var amount = 100m;

            _cardRepositoryMock.Setup(repo => repo.GetCardByIdAsync(cardId))
                               .ReturnsAsync((Card)null);

            // Act
            var result = await _cardTransactionService.DepositAsync(cardId, walletId, amount);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Card not found.", result.Error);
        }

        [TestMethod]
        public async Task DepositAsync_ShouldReturnSuccess_WhenValid()
        {
            // Arrange
            var card = TestHelper.GetTestCard();
            var wallet = TestHelper.GetTestWallet();
            var amount = 100m;

            _cardRepositoryMock.Setup(repo => repo.GetCardByIdAsync(card.Id))
                               .ReturnsAsync(card);

            _walletRepositoryMock.Setup(repo => repo.GetWalletByIdAsync(wallet.Id))
                                 .ReturnsAsync(wallet);

            _transactionHandlingServiceMock.Setup(service => service.ProcessCardToWalletTransactionAsync(card, wallet, amount))
                                           .ReturnsAsync(Result<CardTransaction>.Success(new CardTransaction()));

            // Act
            var result = await _cardTransactionService.DepositAsync(card.Id, wallet.Id, amount);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        public async Task WithdrawAsync_ShouldReturnFailure_WhenInsufficientFunds()
        {
            // Arrange
            var card = TestHelper.GetTestCard();
            var wallet = TestHelper.GetTestWallet();
            wallet.Balance = 50m; // Insufficient balance for withdrawal
            var amount = 100m;

            _walletRepositoryMock.Setup(repo => repo.GetWalletByIdAsync(wallet.Id))
                                 .ReturnsAsync(wallet);

            _cardRepositoryMock.Setup(repo => repo.GetCardByIdAsync(card.Id))
                               .ReturnsAsync(card);

            // Act
            var result = await _cardTransactionService.WithdrawAsync(wallet.Id, card.Id, amount);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Insufficient funds in the wallet.", result.Error);
        }

        [TestMethod]
        public async Task WithdrawAsync_ShouldReturnFailure_WhenTransactionProcessingFails()
        {
            // Arrange
            var card = TestHelper.GetTestCard();
            var wallet = TestHelper.GetTestWallet();
            wallet.Currency = CurrencyType.USD;
            card.Currency = CurrencyType.USD;
            var amount = 100m;

            _walletRepositoryMock.Setup(repo => repo.GetWalletByIdAsync(wallet.Id))
                                 .ReturnsAsync(wallet);

            _cardRepositoryMock.Setup(repo => repo.GetCardByIdAsync(card.Id))
                               .ReturnsAsync(card);

            _transactionHandlingServiceMock.Setup(service => service.ProcessWalletToCardTransactionAsync(wallet, card, amount, 0))
                                           .ReturnsAsync(Result<CardTransaction>.Failure("Transaction processing failed."));

            // Act
            var result = await _cardTransactionService.WithdrawAsync(wallet.Id, card.Id, amount);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Transaction processing failed.", result.Error);
        }

        [TestMethod]
        public async Task GetCardTransactionTotalCountAsync_ShouldReturnFailure_WhenNoTransactionsFound()
        {
            // Arrange
            var filterParameters = new CardTransactionQueryParameters();
            var emptyTransactions = Enumerable.Empty<CardTransaction>().AsQueryable();

            _cardTransactionRepositoryMock.Setup(repo => repo.GetAllCardTransactionsAsync())
                                          .ReturnsAsync(emptyTransactions);

            // Act
            var result = await _cardTransactionService.GetCardTransactionTotalCountAsync(filterParameters);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("No transactions found.", result.Error);
        }

        [TestMethod]
        public async Task GetCardTransactionTotalCountAsync_ShouldReturnSuccess_WhenTransactionsAreFound()
        {
            // Arrange
            var filterParameters = new CardTransactionQueryParameters();
            var sampleTransactions = new List<CardTransaction> { TestHelper.GetTestCardTransaction() }.AsQueryable();

            _cardTransactionRepositoryMock.Setup(repo => repo.GetAllCardTransactionsAsync())
                                          .ReturnsAsync(sampleTransactions);

            // Act
            var result = await _cardTransactionService.GetCardTransactionTotalCountAsync(filterParameters);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, result.Value);
        }

        [TestMethod]
        public async Task GetCardTransactionTotalCountAsync_ShouldApplyCardIdFilter()
        {
            // Arrange
            var filterParameters = new CardTransactionQueryParameters { CardId = 1  };
            var sampleTransactions = new List<CardTransaction>
                {
                    TestHelper.GetTestCardTransaction(),
                    TestHelper.GetTestCardTransaction2()
                }.AsQueryable();

            _cardTransactionRepositoryMock.Setup(repo => repo.GetAllCardTransactionsAsync())
                                          .ReturnsAsync(sampleTransactions);

            // Act
            var result = await _cardTransactionService.GetCardTransactionTotalCountAsync(filterParameters);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, result.Value);
        }

        [TestMethod]
        public async Task GetCardTransactionTotalCountAsync_ShouldApplyCardNumberFilter()
        {
            // Arrange
            var filterParameters = new CardTransactionQueryParameters { CardNumber = "1234567812345678" };
            var sampleTransactions = new List<CardTransaction>
                {
                    TestHelper.GetTestCardTransaction(),
                    TestHelper.GetTestCardTransaction2()
                }.AsQueryable();

            _cardTransactionRepositoryMock.Setup(repo => repo.GetAllCardTransactionsAsync())
                                          .ReturnsAsync(sampleTransactions);

            // Act
            var result = await _cardTransactionService.GetCardTransactionTotalCountAsync(filterParameters);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, result.Value);
        }

        [TestMethod]
        public async Task GetCardTransactionTotalCountAsync_ShouldApplyCardWalletNameFilter()
        {
            // Arrange
            var filterParameters = new CardTransactionQueryParameters { Wallet = "Main Wallet" };
            var sampleTransactions = new List<CardTransaction>
                {
                    TestHelper.GetTestCardTransaction(),
                    TestHelper.GetTestCardTransaction2()
                }.AsQueryable();

            _cardTransactionRepositoryMock.Setup(repo => repo.GetAllCardTransactionsAsync())
                                          .ReturnsAsync(sampleTransactions);

            // Act
            var result = await _cardTransactionService.GetCardTransactionTotalCountAsync(filterParameters);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, result.Value);
        }

        [TestMethod]
        public async Task GetCardTransactionTotalCountAsync_ShouldApplyCardWalletAmountFilter()
        {
            // Arrange
            var filterParameters = new CardTransactionQueryParameters { Amount = 100 };
            var sampleTransactions = new List<CardTransaction>
                {
                    TestHelper.GetTestCardTransaction(),
                    TestHelper.GetTestCardTransaction2()
                }.AsQueryable();

            _cardTransactionRepositoryMock.Setup(repo => repo.GetAllCardTransactionsAsync())
                                          .ReturnsAsync(sampleTransactions);

            // Act
            var result = await _cardTransactionService.GetCardTransactionTotalCountAsync(filterParameters);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, result.Value);
        }

        [TestMethod]
        public async Task GetCardTransactionTotalCountAsync_ShouldApplyDateRangeFilters()
        {
            // Arrange
            var filterParameters = new CardTransactionQueryParameters
            {
                CreatedAfter = DateTime.UtcNow.AddDays(-10),
                CreatedBefore = DateTime.UtcNow.AddDays(-1)
            };
            var sampleTransactions = new List<CardTransaction>
    {
        new CardTransaction { CreatedAt = DateTime.UtcNow.AddDays(-5) },
        new CardTransaction { CreatedAt = DateTime.UtcNow.AddDays(-15) }
    }.AsQueryable();

            _cardTransactionRepositoryMock.Setup(repo => repo.GetAllCardTransactionsAsync())
                                          .ReturnsAsync(sampleTransactions);

            // Act
            var result = await _cardTransactionService.GetCardTransactionTotalCountAsync(filterParameters);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, result.Value);
        }



    }
}
