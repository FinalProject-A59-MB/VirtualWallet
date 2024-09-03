using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtualWallet.BUSINESS.Results;
using VirtualWallet.BUSINESS.Services;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Repositories.Contracts;
using VirtualWallet.DATA.Services;

namespace VirtualWallet.TESTS.BUSINESS.Services.WalletTransactionServiceTests
{
    [TestClass]
    public class WalletTransactionServiceTests
    {
        private Mock<IWalletTransactionRepository> _walletTransactionRepositoryMock;
        private Mock<IWalletRepository> _walletRepositoryMock;
        private Mock<ITransactionHandlingService> _transactionHandlingServiceMock;
        private Mock<ICurrencyService> _currencyService;    
        private WalletTransactionService _walletTransactionService;

        [TestInitialize]
        public void SetUp()
        {
            _walletTransactionRepositoryMock = new Mock<IWalletTransactionRepository>();
            _walletRepositoryMock = new Mock<IWalletRepository>();
            _transactionHandlingServiceMock = new Mock<ITransactionHandlingService>();
            _currencyService = new Mock<ICurrencyService>();
            _walletTransactionService = new WalletTransactionService(
                _walletTransactionRepositoryMock.Object,
                _walletRepositoryMock.Object,
                _transactionHandlingServiceMock.Object,
                _currencyService.Object);
        }

        [TestMethod]
        public async Task VerifySendAmountAsync_Should_ReturnFailure_When_SenderWalletNotFound()
        {
            // Arrange
            _walletRepositoryMock.Setup(repo => repo.GetWalletByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Wallet)null);

            // Act
            var result = await _walletTransactionService.VerifySendAmountAsync(1, new User(), 100);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Wallet not found.", result.Error);
        }

        [TestMethod]
        public async Task VerifySendAmountAsync_Should_ReturnFailure_When_NotEnoughFunds()
        {
            // Arrange
            var senderWallet = new Wallet { Id = 1, Balance = 50 };
            _walletRepositoryMock.Setup(repo => repo.GetWalletByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(senderWallet);

            // Act
            var result = await _walletTransactionService.VerifySendAmountAsync(1, new User(), 100);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Not enough funds in the wallet to complete the transaction.", result.Error);
        }

        [TestMethod]
        public async Task VerifySendAmountAsync_Should_ReturnSuccess_When_Valid()
        {
            // Arrange
            var senderWallet = new Wallet
            {
                Id = 1,
                Balance = 200,
                Currency = DATA.Models.Enums.CurrencyType.USD
            };

            var recipientWallet = new Wallet
            {
                Id = 2,
                Currency = DATA.Models.Enums.CurrencyType.USD
            };

            var recipientUser = new User
            {
                Id = 2,
                MainWallet = recipientWallet
            };

            // Setup mocks
            _walletRepositoryMock.Setup(repo => repo.GetWalletByIdAsync(senderWallet.Id))
                .ReturnsAsync(senderWallet);

            _walletRepositoryMock.Setup(repo => repo.GetWalletsByUserIdAsync(recipientUser.Id))
                .ReturnsAsync(new List<Wallet> { recipientWallet });

            _walletTransactionRepositoryMock.Setup(repo => repo.AddWalletTransactionAsync(It.IsAny<WalletTransaction>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _walletTransactionService.VerifySendAmountAsync(senderWallet.Id, recipientUser, 100);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }





        [TestMethod]
        public async Task ProcessSendAmountAsync_Should_ReturnFailure_When_ProcessingFails()
        {
            // Arrange
            var transaction = new WalletTransaction { Id = 1, SenderId = 1, RecipientId = 2 };
            _transactionHandlingServiceMock.Setup(service => service.ProcessWalletToWalletTransactionAsync(It.IsAny<WalletTransaction>()))
                .ReturnsAsync(Result<WalletTransaction>.Failure("Processing failed."));

            // Act
            var result = await _walletTransactionService.ProcessSendAmountAsync(transaction);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Processing failed.", result.Error);
        }

        [TestMethod]
        public async Task ProcessSendAmountAsync_Should_ReturnSuccess_When_Valid()
        {
            // Arrange
            var transaction = new WalletTransaction { Id = 1, SenderId = 1, RecipientId = 2 };
            _transactionHandlingServiceMock.Setup(service => service.ProcessWalletToWalletTransactionAsync(It.IsAny<WalletTransaction>()))
                .ReturnsAsync(Result<WalletTransaction>.Success(transaction));

            // Act
            var result = await _walletTransactionService.ProcessSendAmountAsync(transaction);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(transaction, result.Value);
        }

        [TestMethod]
        public async Task GetTransactionByIdAsync_Should_ReturnFailure_When_TransactionNotFound()
        {
            // Arrange
            _walletTransactionRepositoryMock.Setup(repo => repo.GetTransactionByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((WalletTransaction)null);

            // Act
            var result = await _walletTransactionService.GetTransactionByIdAsync(1);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Invalid wallet information.", result.Error);
        }

        [TestMethod]
        public async Task GetTransactionByIdAsync_Should_ReturnSuccess_When_TransactionFound()
        {
            // Arrange
            var transaction = new WalletTransaction { Id = 1 };
            _walletTransactionRepositoryMock.Setup(repo => repo.GetTransactionByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(transaction);

            // Act
            var result = await _walletTransactionService.GetTransactionByIdAsync(1);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(transaction, result.Value);
        }

        [TestMethod]
        public async Task GetTransactionsByRecipientIdAsync_Should_ReturnFailure_When_NoTransactionsFound()
        {
            // Arrange
            _walletTransactionRepositoryMock.Setup(repo => repo.GetTransactionsByRecipientIdAsync(It.IsAny<int>()))
                .ReturnsAsync((IEnumerable<WalletTransaction>)null);

            // Act
            var result = await _walletTransactionService.GetTransactionsByRecipientIdAsync(1);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Invalid wallet information.", result.Error);
        }

        [TestMethod]
        public async Task GetTransactionsByRecipientIdAsync_Should_ReturnSuccess_When_TransactionsFound()
        {
            // Arrange
            var transactions = new List<WalletTransaction>
            {
                new WalletTransaction { Id = 1 },
                new WalletTransaction { Id = 2 }
            };
            _walletTransactionRepositoryMock.Setup(repo => repo.GetTransactionsByRecipientIdAsync(It.IsAny<int>()))
                .ReturnsAsync(transactions);

            // Act
            var result = await _walletTransactionService.GetTransactionsByRecipientIdAsync(1);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            CollectionAssert.AreEqual(transactions, result.Value.ToList());
        }

        [TestMethod]
        public async Task GetTransactionsBySenderIdAsync_Should_ReturnFailure_When_NoTransactionsFound()
        {
            // Arrange
            _walletTransactionRepositoryMock.Setup(repo => repo.GetTransactionsBySenderIdAsync(It.IsAny<int>()))
                .ReturnsAsync((IEnumerable<WalletTransaction>)null);

            // Act
            var result = await _walletTransactionService.GetTransactionsBySenderIdAsync(1);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Invalid wallet information.", result.Error);
        }

        [TestMethod]
        public async Task GetTransactionsBySenderIdAsync_Should_ReturnSuccess_When_TransactionsFound()
        {
            // Arrange
            var transactions = new List<WalletTransaction>
            {
                new WalletTransaction { Id = 1 },
                new WalletTransaction { Id = 2 }
            };
            _walletTransactionRepositoryMock.Setup(repo => repo.GetTransactionsBySenderIdAsync(It.IsAny<int>()))
                .ReturnsAsync(transactions);

            // Act
            var result = await _walletTransactionService.GetTransactionsBySenderIdAsync(1);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            CollectionAssert.AreEqual(transactions, result.Value.ToList());
        }

        [TestMethod]
        public async Task GetTotalCountAsync_Should_ReturnFailure_When_NoTransactionsFound()
        {
            // Arrange
            var filterParameters = new TransactionQueryParameters
            {
                Sender = new User { Username = "John" }
            };
            var transactions = new List<WalletTransaction>().AsQueryable();
            _walletTransactionRepositoryMock.Setup(repo => repo.GetAllWalletTransactionsAsync())
                .ReturnsAsync(transactions);

            // Act
            var result = await _walletTransactionService.GetTotalCountAsync(filterParameters);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("No transactions found.", result.Error);
        }
    }
}
