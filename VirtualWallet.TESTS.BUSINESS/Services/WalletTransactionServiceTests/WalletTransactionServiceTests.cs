using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtualWallet.BUSINESS.Resources;
using VirtualWallet.BUSINESS.Results;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Repositories.Contracts;
using VirtualWallet.DATA.Services;
using VirtualWallet.DATA.Services.Contracts;

namespace VirtualWallet.TESTS.BUSINESS.Services.WalletTransactionServiceTests
{
    [TestClass]
    public class WalletTransactionServiceTests
    {
        private Mock<IWalletTransactionRepository> _walletTransactionRepositoryMock;
        private Mock<IWalletRepository> _walletRepositoryMock;
        private Mock<ITransactionHandlingService> _transactionHandlingServiceMock;
        private WalletTransactionService _walletTransactionService;

        [TestInitialize]
        public void SetUp()
        {
            _walletTransactionRepositoryMock = new Mock<IWalletTransactionRepository>();
            _walletRepositoryMock = new Mock<IWalletRepository>();
            _transactionHandlingServiceMock = new Mock<ITransactionHandlingService>();
            _walletTransactionService = new WalletTransactionService(
                _walletTransactionRepositoryMock.Object,
                _walletRepositoryMock.Object,
                _transactionHandlingServiceMock.Object);
        }

        [TestMethod]
        public async Task DepositStep1Async_Should_ReturnFailure_When_SenderWalletNotFound()
        {
            // Arrange
            _walletRepositoryMock.Setup(repo => repo.GetWalletByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Wallet)null);

            // Act
            var result = await _walletTransactionService.DepositStep1Async(1, 2, 100);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Wallet not found.", result.Error);
        }

        [TestMethod]
        public async Task DepositStep1Async_Should_ReturnFailure_When_RecipientWalletNotFound()
        {
            // Arrange
            var senderWallet = new Wallet { Id = 1 };
            _walletRepositoryMock.Setup(repo => repo.GetWalletByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(senderWallet);

            _walletRepositoryMock.Setup(repo => repo.GetWalletByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Wallet)null);

            // Act
            var result = await _walletTransactionService.DepositStep1Async(1, 2, 100);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Wallet not found.", result.Error);
        }

        [TestMethod]
        public async Task DepositStep1Async_Should_ReturnFailure_When_AmountIsInvalid()
        {
            // Arrange
            var senderWallet = new Wallet { Id = 1 };
            var recipientWallet = new Wallet { Id = 2 };
            _walletRepositoryMock.Setup(repo => repo.GetWalletByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(senderWallet);

            _walletRepositoryMock.Setup(repo => repo.GetWalletByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(recipientWallet);

            // Act
            var result = await _walletTransactionService.DepositStep1Async(1, 2, -100);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("The deposit amount must be greater than zero.", result.Error);
        }

        [TestMethod]
        public async Task DepositStep1Async_Should_ReturnSuccess_When_Valid()
        {
            // Arrange
            var senderWallet = new Wallet { Id = 1 };
            var recipientWallet = new Wallet { Id = 2 };
            _walletRepositoryMock.Setup(repo => repo.GetWalletByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(senderWallet);

            _walletRepositoryMock.Setup(repo => repo.GetWalletByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(recipientWallet);

            _transactionHandlingServiceMock.Setup(service => service.ProcessWalletToWalletTransactionStep1Async(
                    It.IsAny<Wallet>(), It.IsAny<Wallet>(), It.IsAny<decimal>()))
                .ReturnsAsync(Result<int>.Success(1));

            // Act
            var result = await _walletTransactionService.DepositStep1Async(1, 2, 100);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, result.Value);
        }

        [TestMethod]
        public async Task DepositStep2Async_Should_ReturnFailure_When_SenderWalletNotFound()
        {
            // Arrange
            _walletRepositoryMock.Setup(repo => repo.GetWalletByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Wallet)null);

            // Act
            var result = await _walletTransactionService.DepositStep2Async(1, 2, 1);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Wallet not found.", result.Error);
        }

        [TestMethod]
        public async Task DepositStep2Async_Should_ReturnFailure_When_RecipientWalletNotFound()
        {
            // Arrange
            var senderWallet = new Wallet { Id = 1 };
            _walletRepositoryMock.Setup(repo => repo.GetWalletByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(senderWallet);

            _walletRepositoryMock.Setup(repo => repo.GetWalletByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Wallet)null);

            // Act
            var result = await _walletTransactionService.DepositStep2Async(1, 2, 1);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Wallet not found.", result.Error);
        }


        [TestMethod]
        public async Task DepositStep2Async_Should_ReturnSuccess_When_Valid()
        {
            // Arrange
            var senderWallet = new Wallet { Id = 1 };
            var recipientWallet = new Wallet { Id = 2 };
            var transaction = new WalletTransaction { Id = 1 };
            _walletRepositoryMock.Setup(repo => repo.GetWalletByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(senderWallet);

            _walletRepositoryMock.Setup(repo => repo.GetWalletByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(recipientWallet);

            _walletTransactionRepositoryMock.Setup(repo => repo.GetTransactionByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(transaction);

            _transactionHandlingServiceMock.Setup(service => service.ProcessWalletToWalletTransactionStep2Async(
                    It.IsAny<Wallet>(), It.IsAny<Wallet>(), It.IsAny<WalletTransaction>()))
                .ReturnsAsync(Result<int>.Success(1));

            // Act
            var result = await _walletTransactionService.DepositStep2Async(1, 2, 1);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, result.Value);
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


