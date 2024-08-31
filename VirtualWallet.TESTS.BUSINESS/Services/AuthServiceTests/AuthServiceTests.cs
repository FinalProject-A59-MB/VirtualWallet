using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualWallet.BUSINESS.Results;
using VirtualWallet.BUSINESS.Services;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Repositories.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VirtualWallet.DATA.Helpers;

namespace VirtualWallet.TESTS.BUSINESS.Services.AuthServiceTests
{
    [TestClass]
    public class AuthServiceTests
    {
        private Mock<IConfiguration> _configurationMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private AuthService _authService;

        [TestInitialize]
        public void SetUp()
        {
            _configurationMock = new Mock<IConfiguration>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _authService = new AuthService(_configurationMock.Object, _userRepositoryMock.Object);
        }

        [TestMethod]
        public async Task AuthenticateAsync_ShouldReturnUser_WhenCredentialsAreValid()
        {
            // Arrange
            var username = "testuser";
            var password = "password";
            var user = TestHelper.GetTestUser();
            user.Password = PasswordHasher.HashPassword(password);

            _userRepositoryMock.Setup(repo => repo.GetUserByUsernameAsync(username)).ReturnsAsync(user);

            // Act
            var result = await _authService.AuthenticateAsync(username, password);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(user, result.Value);
        }

        [TestMethod]
        public async Task AuthenticateAsync_ShouldReturnFailure_WhenUserNotFound()
        {
            // Arrange
            var username = "nonexistentuser";
            var password = "password";

            _userRepositoryMock.Setup(repo => repo.GetUserByUsernameAsync(username)).ReturnsAsync((User)null);

            // Act
            var result = await _authService.AuthenticateAsync(username, password);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Credentials are invalid.", result.Error);
        }

        [TestMethod]
        public async Task AuthenticateAsync_ShouldReturnFailure_WhenPasswordIsInvalid()
        {
            // Arrange
            var username = "testuser";
            var password = "invalidpassword";
            var user = TestHelper.GetTestUser();
            user.Password = PasswordHasher.HashPassword("correctpassword");

            _userRepositoryMock.Setup(repo => repo.GetUserByUsernameAsync(username)).ReturnsAsync(user);

            // Act
            var result = await _authService.AuthenticateAsync(username, password);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Credentials are invalid.", result.Error);
        }

        [TestMethod]
        public async Task ResetPasswordAsync_ShouldUpdatePassword_WhenTokenIsValid()
        {
            // Arrange
            var email = "testuser@example.com";
            var token = "validtoken";
            var newPassword = "TestPassword!1";
            var user = TestHelper.GetTestUser();

            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(email)).ReturnsAsync(user);

            var jwtSectionMock = new Mock<IConfigurationSection>();
            jwtSectionMock.Setup(x => x["Key"]).Returns("your_long_secret_key_here_which_is_32_chars_or_more");
            jwtSectionMock.Setup(x => x["Issuer"]).Returns("issuer");
            jwtSectionMock.Setup(x => x["Audience"]).Returns("audience");
            jwtSectionMock.Setup(x => x["ExpireMinutes"]).Returns("60");

            _configurationMock.Setup(config => config.GetSection("Jwt")).Returns(jwtSectionMock.Object);
            token = _authService.GenerateToken(user);
            // Act
            var result = await _authService.ResetPasswordAsync(email, token, newPassword);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            _userRepositoryMock.Verify(repo => repo.UpdateUserAsync(It.Is<User>(u => PasswordHasher.VerifyPassword(newPassword, u.Password))), Times.Once);
        }


        [TestMethod]
        public async Task ResetPasswordAsync_ShouldReturnFailure_WhenTokenIsInvalid()
        {
            // Arrange
            var email = "testuser@example.com";
            var token = "invalidtoken";
            var newPassword = "newpassword";

            var jwtSectionMock = new Mock<IConfigurationSection>();
            jwtSectionMock.Setup(x => x["Key"]).Returns("your_long_secret_key_here_which_is_32_chars_or_more");
            jwtSectionMock.Setup(x => x["Issuer"]).Returns("issuer");
            jwtSectionMock.Setup(x => x["Audience"]).Returns("audience");
            jwtSectionMock.Setup(x => x["ExpireMinutes"]).Returns("60");

            _configurationMock.Setup(config => config.GetSection("Jwt")).Returns(jwtSectionMock.Object);

            // Act
            var result = await _authService.ResetPasswordAsync(email, token, newPassword);

            // Assert
            Assert.IsFalse(result.IsSuccess);
        }


        [TestMethod]
        public async Task ResetPasswordAsync_ShouldReturnFailure_WhenEmailNotFound()
        {
            // Arrange
            var email = "nonexistent@example.com";
            var token = "validtoken";
            var newPassword = "newpassword";

            var jwtSectionMock = new Mock<IConfigurationSection>();
            jwtSectionMock.Setup(x => x["Key"]).Returns("your_long_secret_key_here_which_is_32_chars_or_more");
            jwtSectionMock.Setup(x => x["Issuer"]).Returns("issuer");
            jwtSectionMock.Setup(x => x["Audience"]).Returns("audience");
            jwtSectionMock.Setup(x => x["ExpireMinutes"]).Returns("60");

            _configurationMock.Setup(config => config.GetSection("Jwt")).Returns(jwtSectionMock.Object);

            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(email)).ReturnsAsync((User)null);

            // Act
            var result = await _authService.ResetPasswordAsync(email, token, newPassword);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Token validation failed: IDX12741: JWT must have three segments (JWS) or five segments (JWE).", result.Error);
        }


        [TestMethod]
        public void GenerateToken_ShouldReturnValidJwtToken()
        {
            // Arrange
            var user = TestHelper.GetTestUser();

            var jwtSectionMock = new Mock<IConfigurationSection>();
            jwtSectionMock.Setup(x => x["Key"]).Returns("your_long_secret_key_here_which_is_32_chars_or_more");
            jwtSectionMock.Setup(x => x["Issuer"]).Returns("issuer");
            jwtSectionMock.Setup(x => x["Audience"]).Returns("audience");
            jwtSectionMock.Setup(x => x["ExpireMinutes"]).Returns("60");

            _configurationMock.Setup(config => config.GetSection("Jwt")).Returns(jwtSectionMock.Object);

            // Act
            var token = _authService.GenerateToken(user);

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(token));
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            Assert.IsNotNull(securityToken);
            Assert.AreEqual(user.Username, securityToken.Subject);
        }


        [TestMethod]
        public void ValidateToken_ShouldReturnSuccess_WhenTokenIsValid()
        {
            // Arrange
            var user = TestHelper.GetTestUser();

            // Mocking the Jwt section in the configuration
            var jwtSectionMock = new Mock<IConfigurationSection>();
            jwtSectionMock.Setup(x => x["Key"]).Returns("your_long_secret_key_here_which_is_32_chars_or_more");
            jwtSectionMock.Setup(x => x["Issuer"]).Returns("issuer");
            jwtSectionMock.Setup(x => x["Audience"]).Returns("audience");
            jwtSectionMock.Setup(x => x["ExpireMinutes"]).Returns("60");

            _configurationMock.Setup(config => config.GetSection("Jwt")).Returns(jwtSectionMock.Object);

            // Generate a token using the mocked configuration
            var token = _authService.GenerateToken(user);

            // Act
            var result = _authService.ValidateToken(token);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value);
        }


        [TestMethod]
        public void ValidateToken_ShouldReturnFailure_WhenTokenIsInvalid()
        {
            // Arrange
            var invalidToken = "invalidtoken";

            // Act
            var jwtSectionMock = new Mock<IConfigurationSection>();
            jwtSectionMock.Setup(x => x["Key"]).Returns("your_long_secret_key_here_which_is_32_chars_or_more");
            jwtSectionMock.Setup(x => x["Issuer"]).Returns("issuer");
            jwtSectionMock.Setup(x => x["Audience"]).Returns("audience");
            jwtSectionMock.Setup(x => x["ExpireMinutes"]).Returns("60");

            _configurationMock.Setup(config => config.GetSection("Jwt")).Returns(jwtSectionMock.Object);

            var result = _authService.ValidateToken(invalidToken);

            // Assert
            Assert.IsFalse(result.IsSuccess);
        }

        [TestMethod]
        public void GetUserIdFromToken_ShouldReturnUserId_WhenTokenIsValid()
        {
            // Arrange
            var user = TestHelper.GetTestUser();

            var jwtSectionMock = new Mock<IConfigurationSection>();
            jwtSectionMock.Setup(x => x["Key"]).Returns("your_long_secret_key_here_which_is_32_chars_or_more");
            jwtSectionMock.Setup(x => x["Issuer"]).Returns("issuer");
            jwtSectionMock.Setup(x => x["Audience"]).Returns("audience");
            jwtSectionMock.Setup(x => x["ExpireMinutes"]).Returns("60");

            _configurationMock.Setup(config => config.GetSection("Jwt")).Returns(jwtSectionMock.Object);

            var token = _authService.GenerateToken(user);

            // Act
            var result = _authService.GetUserIdFromToken(token);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(user.Id, result.Value);
        }


        [TestMethod]
        public void GetUserIdFromToken_ShouldReturnFailure_WhenTokenIsInvalid()
        {
            // Arrange
            var invalidToken = "invalidtoken";

            var jwtSectionMock = new Mock<IConfigurationSection>();
            jwtSectionMock.Setup(x => x["Key"]).Returns("your_long_secret_key_here_which_is_32_chars_or_more");
            jwtSectionMock.Setup(x => x["Issuer"]).Returns("issuer");
            jwtSectionMock.Setup(x => x["Audience"]).Returns("audience");
            jwtSectionMock.Setup(x => x["ExpireMinutes"]).Returns("60");

            _configurationMock.Setup(config => config.GetSection("Jwt")).Returns(jwtSectionMock.Object);

            // Act
            var result = _authService.GetUserIdFromToken(invalidToken);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Failed to extract user ID from token: IDX12741: JWT must have three segments (JWS) or five segments (JWE).", result.Error);
        }




    }
}
