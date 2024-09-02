using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtualWallet.BUSINESS.Resources;
using VirtualWallet.BUSINESS.Results;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.DATA.Repositories.Contracts;
using VirtualWallet.DATA.Services;
using VirtualWallet.DATA.Services.Contracts;

namespace VirtualWallet.TESTS.BUSINESS.Services.UserWalletServiceTests
{
    [TestClass]
    public class UserWalletServiceTests
    {
        private Mock<IUserWalletRepository> _userWalletRepositoryMock;
        private Mock<IUserService> _userServiceMock;
        private Mock<IWalletService> _walletServiceMock;
        private UserWalletService _userWalletService;

        [TestInitialize]
        public void SetUp()
        {
            _userWalletRepositoryMock = new Mock<IUserWalletRepository>();
            _userServiceMock = new Mock<IUserService>();
            _walletServiceMock = new Mock<IWalletService>();
            _userWalletService = new UserWalletService(_userWalletRepositoryMock.Object, _walletServiceMock.Object, _userServiceMock.Object);
        }

        [TestMethod]
        public async Task AddUserWalletAsync_Should_ReturnFailure_When_UserNotFound()
        {
            // Arrange
            var wallet = TestHelper.GetTestWallet();
            _walletServiceMock.Setup(service => service.GetWalletByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(Result<Wallet>.Success(wallet));
            _userServiceMock.Setup(service => service.GetUserByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(Result<User>.Success(null));

            // Act
            var result = await _userWalletService.AddUserWalletAsync(1, 1);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Invalid user information.", result.Error);
        }

        [TestMethod]
        public async Task AddUserWalletAsync_Should_ReturnFailure_When_WalletIsNotJoint()
        {
            // Arrange
            var wallet = TestHelper.GetTestWallet();
            wallet.WalletType = WalletType.Standart;
            _walletServiceMock.Setup(service => service.GetWalletByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(Result<Wallet>.Success(wallet));
            _userServiceMock.Setup(service => service.GetUserByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(Result<User>.Success(TestHelper.GetTestUser()));

            // Act
            var result = await _userWalletService.AddUserWalletAsync(1, 1);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Invalid user information.", result.Error);
        }

        [TestMethod]
        public async Task AddUserWalletAsync_Should_ReturnFailure_When_UserIsAlreadyAddedToWallet()
        {
            // Arrange
            var wallet = TestHelper.GetTestWallet();
            _walletServiceMock.Setup(service => service.GetWalletByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(Result<Wallet>.Success(wallet));
            _userServiceMock.Setup(service => service.GetUserByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(Result<User>.Success(TestHelper.GetTestUser()));

            var userWallets = new List<UserWallet> { TestHelper.GetTestUserWallet() };
            _userWalletRepositoryMock.Setup(repo => repo.GetUserWalletByWalletIdAsync(It.IsAny<int>()))
                .ReturnsAsync(userWallets);

            // Act
            var result = await _userWalletService.AddUserWalletAsync(1, 1);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Invalid user information.", result.Error);
        }


        [TestMethod]
        public async Task GetUserWalletsByUserIdAsync_Should_ReturnUserWallets_When_UserWalletsExist()
        {
            // Arrange
            var userWallets = new List<UserWallet> { TestHelper.GetTestUserWallet() };
            _userWalletRepositoryMock.Setup(repo => repo.GetUserWalletsByUserIdAsync(It.IsAny<int>()))
                .ReturnsAsync(userWallets);

            // Act
            var result = await _userWalletService.GetUserWalletsByUserIdAsync(1);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(userWallets.Count, result.Value.Count());
        }

        [TestMethod]
        public async Task GetUserWalletByWalletIdAsync_Should_ReturnUserWallets_When_WalletExists()
        {
            // Arrange
            var userWallets = new List<UserWallet> { TestHelper.GetTestUserWallet() };
            _userWalletRepositoryMock.Setup(repo => repo.GetUserWalletByWalletIdAsync(It.IsAny<int>()))
                .ReturnsAsync(userWallets);

            // Act
            var result = await _userWalletService.GetUserWalletByWalletIdAsync(1);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(userWallets.Count, result.Value.Count());
        }

        [TestMethod]
        public async Task RemoveUserWalletAsync_Should_Succeed_When_ValidDataProvided()
        {
            // Arrange
            _walletServiceMock.Setup(service => service.GetWalletByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(Result<Wallet>.Success(TestHelper.GetTestWallet()));
            _userServiceMock.Setup(service => service.GetUserByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(Result<User>.Success(TestHelper.GetTestUser()));

            _userWalletRepositoryMock.Setup(repo => repo.RemoveUserWalletAsync(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _userWalletService.RemoveUserWalletAsync(1, 1);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }
    }
}

