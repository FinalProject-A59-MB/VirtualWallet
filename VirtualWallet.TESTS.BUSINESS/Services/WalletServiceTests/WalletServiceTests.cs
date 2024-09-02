using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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

namespace VirtualWallet.TESTS.BUSINESS.Services.WalletServiceTests
{
    [TestClass]
    public class WalletServiceTests
    {
        private Mock<IWalletRepository> _walletRepositoryMock;
        private Mock<IUserService> _userServiceMock;
        private Mock<IEmailService> _emailServiceMock;
        private WalletService _walletService;

        [TestInitialize]
        public void SetUp()
        {
            _walletRepositoryMock = new Mock<IWalletRepository>();
            _userServiceMock = new Mock<IUserService>();
            _emailServiceMock = new Mock<IEmailService>();
            _walletService = new WalletService(_walletRepositoryMock.Object, _userServiceMock.Object, _emailServiceMock.Object);
        }

        [TestMethod]
        public async Task AddWalletAsync_Should_ReturnFailure_When_WalletIsNull()
        {
            // Act
            var result = await _walletService.AddWalletAsync(null);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Invalid wallet information.", result.Error);
        }

        [TestMethod]
        public async Task AddWalletAsync_Should_ReturnSuccess_When_WalletIsValid()
        {
            // Arrange
            var wallet = TestHelper.GetTestWallet();
            _walletRepositoryMock.Setup(repo => repo.AddWalletAsync(wallet))
                .ReturnsAsync(1);

            // Act
            var result = await _walletService.AddWalletAsync(wallet);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, result.Value);
        }

        [TestMethod]
        public async Task GetWalletByIdAsync_Should_ReturnFailure_When_WalletNotFound()
        {
            // Arrange
            _walletRepositoryMock.Setup(repo => repo.GetWalletByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Wallet)null);

            // Act
            var result = await _walletService.GetWalletByIdAsync(1);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Invalid wallet information.", result.Error);
        }

        [TestMethod]
        public async Task GetWalletByIdAsync_Should_ReturnSuccess_When_WalletFound()
        {
            // Arrange
            var wallet = TestHelper.GetTestWallet();
            _walletRepositoryMock.Setup(repo => repo.GetWalletByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(wallet);

            // Act
            var result = await _walletService.GetWalletByIdAsync(1);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(wallet, result.Value);
        }

        [TestMethod]
        public async Task GetWalletByNameAsync_Should_ReturnFailure_When_WalletNotFound()
        {
            // Arrange
            _walletRepositoryMock.Setup(repo => repo.GetWalletByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((Wallet)null);

            // Act
            var result = await _walletService.GetWalletByNameAsync("Test Wallet");

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Invalid wallet information.", result.Error);
        }

        [TestMethod]
        public async Task GetWalletByNameAsync_Should_ReturnSuccess_When_WalletFound()
        {
            // Arrange
            var wallet = TestHelper.GetTestWallet();
            _walletRepositoryMock.Setup(repo => repo.GetWalletByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(wallet);

            // Act
            var result = await _walletService.GetWalletByNameAsync("Test Wallet");

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(wallet, result.Value);
        }

        [TestMethod]
        public async Task GetWalletsByUserIdAsync_Should_ReturnSuccess_When_WalletsFound()
        {
            // Arrange
            var wallets = new List<Wallet> { TestHelper.GetTestWallet() };
            _walletRepositoryMock.Setup(repo => repo.GetWalletsByUserIdAsync(It.IsAny<int>()))
                .ReturnsAsync(wallets);

            // Act
            var result = await _walletService.GetWalletsByUserIdAsync(1);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(wallets.Count, result.Value.Count());
        }

        [TestMethod]
        public async Task GetWalletsByUserIdAsync_Should_ReturnFailure_When_WalletsNotFound()
        {
            // Arrange
            _walletRepositoryMock.Setup(repo => repo.GetWalletsByUserIdAsync(It.IsAny<int>()))
                .ReturnsAsync((IEnumerable<Wallet>)null);

            // Act
            var result = await _walletService.GetWalletsByUserIdAsync(1);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Invalid wallet information.", result.Error);
        }

        [TestMethod]
        public async Task RemoveWalletAsync_Should_ReturnFailure_When_WalletNotFound()
        {
            // Arrange
            _walletRepositoryMock.Setup(repo => repo.GetWalletByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Wallet)null);

            // Act
            var result = await _walletService.RemoveWalletAsync(1);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Wallet not found.", result.Error);
        }

        [TestMethod]
        public async Task RemoveWalletAsync_Should_ReturnFailure_When_WalletNotEmpty()
        {
            // Arrange
            var wallet = TestHelper.GetTestWallet();
            wallet.Balance = 100;
            _walletRepositoryMock.Setup(repo => repo.GetWalletByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(wallet);

            // Act
            var result = await _walletService.RemoveWalletAsync(1);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Wallet is not empty!", result.Error);
        }

        [TestMethod]
        public async Task RemoveWalletAsync_Should_ReturnSuccess_When_WalletIsEmptyAndExists()
        {
            // Arrange
            var wallet = TestHelper.GetTestWallet();
            wallet.Balance = 0;
            _walletRepositoryMock.Setup(repo => repo.GetWalletByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(wallet);

            _walletRepositoryMock.Setup(repo => repo.RemoveWalletAsync(It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _walletService.RemoveWalletAsync(1);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        public async Task UpdateWalletAsync_Should_ReturnFailure_When_WalletIsNull()
        {
            // Act
            var result = await _walletService.UpdateWalletAsync(null);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Invalid wallet information.", result.Error);
        }

        [TestMethod]
        public async Task UpdateWalletAsync_Should_ReturnFailure_When_WalletNotFound()
        {
            // Arrange
            _walletRepositoryMock.Setup(repo => repo.GetWalletByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Wallet)null);

            // Act
            var result = await _walletService.UpdateWalletAsync(new Wallet());

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Wallet not found.", result.Error);
        }

        [TestMethod]
        public async Task UpdateWalletAsync_Should_ReturnSuccess_When_WalletIsUpdated()
        {
            // Arrange
            var wallet = TestHelper.GetTestWallet();
            _walletRepositoryMock.Setup(repo => repo.GetWalletByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(wallet);

            _walletRepositoryMock.Setup(repo => repo.UpdateWalletAsync(wallet))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _walletService.UpdateWalletAsync(wallet);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        public async Task GetWalletIdByUserDetailsAsync_Should_ReturnFailure_When_WalletNotFound()
        {
            // Arrange
            _walletRepositoryMock.Setup(repo => repo.GetWalletByUserDetailsAsync(It.IsAny<string>()))
                .ReturnsAsync((Wallet)null);

            // Act
            var result = await _walletService.GetWalletIdByUserDetailsAsync("details");

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Wallet not found.", result.Error);
        }

        [TestMethod]
        public async Task GetWalletIdByUserDetailsAsync_Should_ReturnSuccess_When_WalletFound()
        {
            // Arrange
            var wallet = TestHelper.GetTestWallet();
            _walletRepositoryMock.Setup(repo => repo.GetWalletByUserDetailsAsync(It.IsAny<string>()))
                .ReturnsAsync(wallet);

            // Act
            var result = await _walletService.GetWalletIdByUserDetailsAsync("details");

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(wallet.Id, result.Value);
        }

        [TestMethod]
        public async Task AddUserToJointWalletAsync_Should_ReturnFailure_When_WalletNotFound()
        {
            // Arrange
            _walletRepositoryMock.Setup(repo => repo.GetWalletByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Wallet)null);

            // Act
            var result = await _walletService.AddUserToJointWalletAsync(1, "username");

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Wallet not found.", result.Error);
        }

        [TestMethod]
        public async Task AddUserToJointWalletAsync_Should_ReturnFailure_When_WalletIsNotJoint()
        {
            // Arrange
            var wallet = TestHelper.GetTestWallet();
            wallet.WalletType = DATA.Models.Enums.WalletType.Standart;
            _walletRepositoryMock.Setup(repo => repo.GetWalletByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(wallet);

            // Act
            var result = await _walletService.AddUserToJointWalletAsync(1, "username");

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Invalid wallet information.", result.Error);
        }

        [TestMethod]
        public async Task AddUserToJointWalletAsync_Should_ReturnFailure_When_UserNotFound()
        {
            // Arrange
            var wallet = TestHelper.GetTestWallet();
            wallet.WalletType = DATA.Models.Enums.WalletType.Joint;
            _walletRepositoryMock.Setup(repo => repo.GetWalletByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(wallet);

            _userServiceMock.Setup(us => us.GetUserByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync(Result<User>.Failure("User with id={0} doesn't exist."));

            // Act
            var result = await _walletService.AddUserToJointWalletAsync(1, "username");

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("User with id={0} doesn't exist.", result.Error);
        }

        [TestMethod]
        public async Task AddUserToJointWalletAsync_Should_ReturnSuccess_When_UserAdded()
        {
            // Arrange
            var wallet = TestHelper.GetTestWallet();
            wallet.WalletType = DATA.Models.Enums.WalletType.Joint;
            var user = TestHelper.GetTestUser();

            _walletRepositoryMock.Setup(repo => repo.GetWalletByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(wallet);

            _userServiceMock.Setup(us => us.GetUserByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync(Result<User>.Success(user));

            _walletRepositoryMock.Setup(repo => repo.AddUserToJointWalletAsync(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            //_emailServiceMock.Setup(es => es.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            //    .Returns(Result.Success());

            // Act
            var result = await _walletService.AddUserToJointWalletAsync(1, "username");

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        public async Task RemoveUserFromJointWalletAsync_Should_ReturnFailure_When_WalletNotFound()
        {
            // Arrange
            _walletRepositoryMock.Setup(repo => repo.GetWalletByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Wallet)null);

            // Act
            var result = await _walletService.RemoveUserFromJointWalletAsync(1, "username");

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Wallet not found.", result.Error);
        }

        [TestMethod]
        public async Task RemoveUserFromJointWalletAsync_Should_ReturnFailure_When_WalletIsNotJoint()
        {
            // Arrange
            var wallet = TestHelper.GetTestWallet();
            wallet.WalletType = DATA.Models.Enums.WalletType.Standart;
            _walletRepositoryMock.Setup(repo => repo.GetWalletByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(wallet);

            // Act
            var result = await _walletService.RemoveUserFromJointWalletAsync(1, "username");

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Invalid wallet information.", result.Error);
        }

        [TestMethod]
        public async Task RemoveUserFromJointWalletAsync_Should_ReturnFailure_When_UserNotFound()
        {
            // Arrange
            var wallet = TestHelper.GetTestWallet();
            wallet.WalletType = DATA.Models.Enums.WalletType.Joint;
            _walletRepositoryMock.Setup(repo => repo.GetWalletByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(wallet);

            _userServiceMock.Setup(us => us.GetUserByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync(Result<User>.Failure("User with id={0} doesn't exist."));

            // Act
            var result = await _walletService.RemoveUserFromJointWalletAsync(1, "username");

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("User with id={0} doesn't exist.", result.Error);
        }

        [TestMethod]
        public async Task RemoveUserFromJointWalletAsync_Should_ReturnSuccess_When_UserRemoved()
        {
            // Arrange
            var wallet = TestHelper.GetTestWallet();
            wallet.WalletType = DATA.Models.Enums.WalletType.Joint;
            var user = TestHelper.GetTestUser();

            _walletRepositoryMock.Setup(repo => repo.GetWalletByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(wallet);

            _userServiceMock.Setup(us => us.GetUserByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync(Result<User>.Success(user));

            _walletRepositoryMock.Setup(repo => repo.RemoveUserFromJointWalletAsync(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            //_emailServiceMock.Setup(es => es.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            //    .Returns(Task.CompletedTask);

            // Act
            var result = await _walletService.RemoveUserFromJointWalletAsync(1, "username");

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }
    }
}

