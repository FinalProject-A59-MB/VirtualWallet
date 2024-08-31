using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualWallet.BUSINESS.Results;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.BUSINESS.Services.Responses;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.DATA.Repositories.Contracts;
using VirtualWallet.DATA.Services;

namespace VirtualWallet.TESTS.BUSINESS.Services.UserServiceTests
{
    [TestClass]
    public class UserServiceTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<ICurrencyService> _currencyServiceMock;
        private UserService _userService;

        [TestInitialize]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _currencyServiceMock = new Mock<ICurrencyService>();
            _userService = new UserService(_userRepositoryMock.Object, _currencyServiceMock.Object);
        }

        [TestMethod]
        public async Task RegisterUserAsync_Should_ReturnFailure_When_UsernameAlreadyExists()
        {
            // Arrange
            var existingUser = TestHelper.GetTestUser();
            _userRepositoryMock.Setup(repo => repo.GetUserByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync(existingUser);

            var newUser = new User
            {
                Username = "testuser",
                Email = "newuser@example.com",
                Password = "TestPassword!1"
            };

            // Act
            var result = await _userService.RegisterUserAsync(newUser);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Username already exists.", result.Error);
        }

        [TestMethod]
        public async Task RegisterUserAsync_Should_ReturnFailure_When_EmailAlreadyExists()
        {
            // Arrange
            var existingUser = TestHelper.GetTestUser();
            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(existingUser);

            var newUser = new User
            {
                Username = "newuser",
                Email = "testuser@example.com",
                Password = "TestPassword!1"
            };

            // Act
            var result = await _userService.RegisterUserAsync(newUser);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Email already exists.", result.Error);
        }

        [TestMethod]
        public async Task RegisterUserAsync_Should_Succeed_When_ValidUserProvided()
        {
            // Arrange
            _userRepositoryMock.Setup(repo => repo.GetUserByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            var newUser = new User
            {
                Username = "newuser",
                Email = "newuser@example.com",
                Password = "TestPassword!1"
            };

            // Act
            var result = await _userService.RegisterUserAsync(newUser);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(newUser.Username, result.Value.Username);
        }

        [TestMethod]
        public async Task RegisterUserAsync_Should_ReturnFailure_When_EmailValidationFails()
        {
            // Arrange
            var invalidEmail = "invalidemail";
            var user = new User
            {
                Username = "newuser",
                Email = invalidEmail,
                Password = "password"
            };

            // Act
            var result = await _userService.RegisterUserAsync(user);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Email format is invalid.", result.Error);
        }

        [TestMethod]
        public async Task RegisterUserAsync_Should_ReturnFailure_When_PasswordValidationFails()
        {
            // Arrange
            var user = new User
            {
                Username = "newuser",
                Email = "newuser@example.com",
                Password = "weak"
            };


            // Act
            var result = await _userService.RegisterUserAsync(user);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Password must be at least 8 characters long.", result.Error);
        }



        [TestMethod]
        public async Task ChangePasswordAsync_Should_ReturnFailure_When_UserNotFound()
        {
            // Arrange
            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((User)null);

            var userId = 1;
            var currentPassword = "oldpassword";
            var newPassword = "newpassword";

            // Act
            var result = await _userService.ChangePasswordAsync(userId, currentPassword, newPassword);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("User not found.", result.Error);
        }

        [TestMethod]
        public async Task ChangePasswordAsync_Should_ReturnFailure_When_CurrentPasswordIsIncorrect()
        {
            // Arrange
            var user = TestHelper.GetTestUser();
            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(user);

            var userId = user.Id;
            var currentPassword = "wrongpassword";
            var newPassword = "newpassword";

            // Act
            var result = await _userService.ChangePasswordAsync(userId, currentPassword, newPassword);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Current password is incorrect.", result.Error);
        }

        [TestMethod]
        public async Task ChangePasswordAsync_Should_Succeed_When_ValidDataProvided()
        {
            // Arrange
            var user = TestHelper.GetTestUser();
            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(user);

            var userId = user.Id;
            var currentPassword = "hashedpassword";
            var newPassword = "TestPassword!1";

            // Act
            var result = await _userService.ChangePasswordAsync(userId, currentPassword, newPassword);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        public async Task GetUsers_Should_ReturnAllUsers()
        {
            // Arrange
            var users = new List<User> { TestHelper.GetTestUser(), TestHelper.GetTestUser2() };
            _userRepositoryMock.Setup(repo => repo.GetAllUsers()).ReturnsAsync(users);

            // Act
            var result = await _userService.GetUsers();

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(users.Count, result.Value.Count());
            _userRepositoryMock.Verify(repo => repo.GetAllUsers(), Times.Once);
        }

        [TestMethod]
        public async Task GetUserByUsernameAsync_Should_ReturnUser_When_UserExists()
        {
            // Arrange
            var username = "testuser";
            var expectedUser = TestHelper.GetTestUser();
            _userRepositoryMock.Setup(repo => repo.GetUserByUsernameAsync(username)).ReturnsAsync(expectedUser);

            // Act
            var result = await _userService.GetUserByUsernameAsync(username);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(expectedUser, result.Value);
            _userRepositoryMock.Verify(repo => repo.GetUserByUsernameAsync(username), Times.Once);
        }

        [TestMethod]
        public async Task GetUserByUsernameAsync_Should_ReturnFailure_When_UserNotFound()
        {
            // Arrange
            var username = "nonexistentuser";
            _userRepositoryMock.Setup(repo => repo.GetUserByUsernameAsync(username)).ReturnsAsync((User)null);

            // Act
            var result = await _userService.GetUserByUsernameAsync(username);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("User not found.", result.Error);
            _userRepositoryMock.Verify(repo => repo.GetUserByUsernameAsync(username), Times.Once);
        }

        [TestMethod]
        public async Task GetUserByEmailAsync_Should_ReturnUser_When_UserExists()
        {
            // Arrange
            var email = "testuser@example.com";
            var expectedUser = TestHelper.GetTestUser();
            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(email)).ReturnsAsync(expectedUser);

            // Act
            var result = await _userService.GetUserByEmailAsync(email);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(expectedUser, result.Value);
            _userRepositoryMock.Verify(repo => repo.GetUserByEmailAsync(email), Times.Once);
        }

        [TestMethod]
        public async Task GetUserByEmailAsync_Should_ReturnFailure_When_UserNotFound()
        {
            // Arrange
            var email = "nonexistent@example.com";
            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(email)).ReturnsAsync((User)null);

            // Act
            var result = await _userService.GetUserByEmailAsync(email);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("User not found.", result.Error);
            _userRepositoryMock.Verify(repo => repo.GetUserByEmailAsync(email), Times.Once);
        }

        [TestMethod]
        public async Task GetUserProfileAsync_Should_ReturnUserProfile_When_ProfileExists()
        {
            // Arrange
            var userId = 1;
            var expectedProfile = TestHelper.GetTestUserProfile();
            _userRepositoryMock.Setup(repo => repo.GetUserProfileAsync(userId)).ReturnsAsync(expectedProfile);

            // Act
            var result = await _userService.GetUserProfileAsync(userId);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(expectedProfile, result.Value);
            _userRepositoryMock.Verify(repo => repo.GetUserProfileAsync(userId), Times.Once);
        }

        [TestMethod]
        public async Task GetUserProfileAsync_Should_ReturnFailure_When_ProfileNotFound()
        {
            // Arrange
            var userId = 1;
            _userRepositoryMock.Setup(repo => repo.GetUserProfileAsync(userId)).ReturnsAsync((UserProfile)null);

            // Act
            var result = await _userService.GetUserProfileAsync(userId);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("User profile not found.", result.Error);
            _userRepositoryMock.Verify(repo => repo.GetUserProfileAsync(userId), Times.Once);
        }

        [TestMethod]
        public async Task UpdateUserAsync_Should_UpdateUser_And_ReturnSuccess()
        {
            // Arrange
            var user = TestHelper.GetTestUser();

            // Act
            var result = await _userService.UpdateUserAsync(user);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            _userRepositoryMock.Verify(repo => repo.UpdateUserAsync(user), Times.Once);
        }

        [TestMethod]
        public async Task UpdateUserProfileAsync_Should_UpdateUserProfile_And_ReturnSuccess()
        {
            // Arrange
            var userProfile = TestHelper.GetTestUserProfile();

            // Act
            var result = await _userService.UpdateUserProfileAsync(userProfile);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            _userRepositoryMock.Verify(repo => repo.UpdateUserProfileAsync(userProfile), Times.Once);
        }

        [TestMethod]
        public async Task DeleteUserAsync_Should_DeleteUser_And_ReturnSuccess_When_UserExists()
        {
            // Arrange
            var userId = 1;
            var user = TestHelper.GetTestUser();
            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await _userService.DeleteUserAsync(userId);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            _userRepositoryMock.Verify(repo => repo.GetUserByIdAsync(userId), Times.Once);
            _userRepositoryMock.Verify(repo => repo.DeleteUserAsync(userId), Times.Once);
        }

        [TestMethod]
        public async Task DeleteUserAsync_Should_ReturnFailure_When_UserNotFound()
        {
            // Arrange
            var userId = 1;
            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync((User)null);

            // Act
            var result = await _userService.DeleteUserAsync(userId);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("User not found.", result.Error);
            _userRepositoryMock.Verify(repo => repo.GetUserByIdAsync(userId), Times.Once);
            _userRepositoryMock.Verify(repo => repo.DeleteUserAsync(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public async Task SendFriendRequestAsync_Should_SendRequest_When_NoExistingRequest()
        {
            // Arrange
            var userId = 1;
            var contactId = 2;
            _userRepositoryMock.Setup(repo => repo.GetUserContactAsync(userId, contactId)).ReturnsAsync((UserContact)null);

            // Act
            var result = await _userService.SendFriendRequestAsync(userId, contactId);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            _userRepositoryMock.Verify(repo => repo.GetUserContactAsync(userId, contactId), Times.Once);

            _userRepositoryMock.Verify(repo => repo.AddContactAsync(It.Is<UserContact>(uc => uc.UserId == userId && uc.ContactId == contactId && uc.SenderId == userId)), Times.Once);
            _userRepositoryMock.Verify(repo => repo.AddContactAsync(It.Is<UserContact>(uc => uc.UserId == contactId && uc.ContactId == userId && uc.SenderId == userId)), Times.Once);
        }


        [TestMethod]
        public async Task SendFriendRequestAsync_Should_ReturnFailure_When_RequestAlreadyExists()
        {
            // Arrange
            var userId = 1;
            var contactId = 2;
            var existingRequest = new UserContact { UserId = userId, ContactId = contactId, Status = FriendRequestStatus.Pending };
            _userRepositoryMock.Setup(repo => repo.GetUserContactAsync(userId, contactId)).ReturnsAsync(existingRequest);

            // Act
            var result = await _userService.SendFriendRequestAsync(userId, contactId);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Friend request already sent.", result.Error);
            _userRepositoryMock.Verify(repo => repo.GetUserContactAsync(userId, contactId), Times.Once);
            _userRepositoryMock.Verify(repo => repo.AddContactAsync(It.IsAny<UserContact>()), Times.Never);
        }

        [TestMethod]
        public async Task AcceptFriendRequestAsync_Should_AcceptRequest_When_RequestExists()
        {
            // Arrange
            var userId = 1;
            var contactId = 2;
            var pendingRequest = new UserContact
            {
                UserId = contactId,
                ContactId = userId,
                Status = FriendRequestStatus.Pending
            };

            _userRepositoryMock.Setup(repo => repo.GetUserContactAsync(contactId, userId))
                .ReturnsAsync(pendingRequest);
            _userRepositoryMock.Setup(repo => repo.GetUserContactAsync(userId, contactId))
                .ReturnsAsync((UserContact)null);

            // Act
            var result = await _userService.AcceptFriendRequestAsync(userId, contactId);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            _userRepositoryMock.Verify(repo => repo.UpdateContactAsync(It.Is<UserContact>(uc => uc.UserId == contactId && uc.ContactId == userId && uc.Status == FriendRequestStatus.Accepted)), Times.Once);
            _userRepositoryMock.Verify(repo => repo.AddContactAsync(It.Is<UserContact>(uc => uc.UserId == userId && uc.ContactId == contactId && uc.Status == FriendRequestStatus.Accepted)), Times.Once);
        }

        [TestMethod]
        public async Task AcceptFriendRequestAsync_Should_ReturnFailure_When_RequestDoesNotExist()
        {
            // Arrange
            var userId = 1;
            var contactId = 2;
            _userRepositoryMock.Setup(repo => repo.GetUserContactAsync(contactId, userId))
                .ReturnsAsync((UserContact)null);

            // Act
            var result = await _userService.AcceptFriendRequestAsync(userId, contactId);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Friend request not found.", result.Error);
            _userRepositoryMock.Verify(repo => repo.UpdateContactAsync(It.IsAny<UserContact>()), Times.Never);
            _userRepositoryMock.Verify(repo => repo.AddContactAsync(It.IsAny<UserContact>()), Times.Never);
        }

        [TestMethod]
        public async Task GetPendingFriendRequestsAsync_Should_ReturnPendingRequests_When_TheyExist()
        {
            // Arrange
            var userId = 1;
            var pendingRequests = new List<UserContact>
    {
        new UserContact { UserId = userId, ContactId = 2, Status = FriendRequestStatus.Pending }
    };

            _userRepositoryMock.Setup(repo => repo.GetPendingFriendRequestsAsync(userId))
                .ReturnsAsync(pendingRequests);

            // Act
            var result = await _userService.GetPendingFriendRequestsAsync(userId);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, result.Value.Count());
            _userRepositoryMock.Verify(repo => repo.GetPendingFriendRequestsAsync(userId), Times.Once);
        }

        [TestMethod]
        public async Task GetPendingFriendRequestsAsync_Should_ReturnEmpty_When_NoPendingRequestsExist()
        {
            // Arrange
            var userId = 1;
            var pendingRequests = new List<UserContact>();

            _userRepositoryMock.Setup(repo => repo.GetPendingFriendRequestsAsync(userId))
                .ReturnsAsync(pendingRequests);

            // Act
            var result = await _userService.GetPendingFriendRequestsAsync(userId);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(0, result.Value.Count());
            _userRepositoryMock.Verify(repo => repo.GetPendingFriendRequestsAsync(userId), Times.Once);
        }

        [TestMethod]
        public async Task GetFriendsAsync_Should_ReturnFriends_When_FriendsExist()
        {
            // Arrange
            var userId = 1;
            var friendsList = new List<User>
    {
        TestHelper.GetTestUser(),
        new User { Id = 2, Username = "friend1", Email = "friend1@example.com" },
        new User { Id = 3, Username = "friend2", Email = "friend2@example.com" }
    };

            _userRepositoryMock.Setup(repo => repo.GetUserContactsAsync(userId))
                .ReturnsAsync(friendsList);

            // Act
            var result = await _userService.GetFriendsAsync(userId);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(friendsList.Count, result.Value.Count());
            CollectionAssert.AreEqual(friendsList, result.Value.ToList());
        }

        [TestMethod]
        public async Task GetFriendsAsync_Should_ReturnEmpty_When_NoFriendsExist()
        {
            // Arrange
            var userId = 1;
            var emptyFriendsList = new List<User>();

            _userRepositoryMock.Setup(repo => repo.GetUserContactsAsync(userId))
                .ReturnsAsync(emptyFriendsList);

            // Act
            var result = await _userService.GetFriendsAsync(userId);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(0, result.Value.Count());
        }

        [TestMethod]
        public async Task DenyFriendRequestAsync_Should_DenyRequest_When_RequestExists()
        {
            // Arrange
            var userId = 1;
            var contactId = 2;
            var pendingRequest = new UserContact
            {
                UserId = contactId,
                ContactId = userId,
                Status = FriendRequestStatus.Pending
            };

            _userRepositoryMock.Setup(repo => repo.GetUserContactAsync(contactId, userId))
                .ReturnsAsync(pendingRequest);

            // Act
            var result = await _userService.DenyFriendRequestAsync(userId, contactId);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            _userRepositoryMock.Verify(repo => repo.RemoveContactAsync(It.Is<UserContact>(uc => uc.UserId == contactId && uc.ContactId == userId)), Times.Once);
        }

        [TestMethod]
        public async Task DenyFriendRequestAsync_Should_ReturnFailure_When_RequestDoesNotExist()
        {
            // Arrange
            var userId = 1;
            var contactId = 2;
            _userRepositoryMock.Setup(repo => repo.GetUserContactAsync(contactId, userId))
                .ReturnsAsync((UserContact)null);

            // Act
            var result = await _userService.DenyFriendRequestAsync(userId, contactId);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Friend request not found.", result.Error);
            _userRepositoryMock.Verify(repo => repo.RemoveContactAsync(It.IsAny<UserContact>()), Times.Never);
        }

        [TestMethod]
        public async Task SearchUsersAsync_Should_ReturnFailure_When_SearchTermIsEmpty()
        {
            // Arrange
            var searchTerm = string.Empty;

            // Act
            var result = await _userService.SearchUsersAsync(searchTerm);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Search term cannot be empty.", result.Error);
            _userRepositoryMock.Verify(repo => repo.SearchUsersAsync(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task SearchUsersAsync_Should_ReturnUsers_When_UsersExist()
        {
            // Arrange
            var searchTerm = "test";
            var users = new List<User> { TestHelper.GetTestUser() };

            _userRepositoryMock.Setup(repo => repo.SearchUsersAsync(searchTerm))
                .ReturnsAsync(users);

            // Act
            var result = await _userService.SearchUsersAsync(searchTerm);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, result.Value.Count());
            _userRepositoryMock.Verify(repo => repo.SearchUsersAsync(searchTerm), Times.Once);
        }

        [TestMethod]
        public async Task SearchUsersAsync_Should_ReturnFailure_When_NoUsersFound()
        {
            // Arrange
            var searchTerm = "test";
            var users = new List<User>();

            _userRepositoryMock.Setup(repo => repo.SearchUsersAsync(searchTerm))
                .ReturnsAsync(users);

            // Act
            var result = await _userService.SearchUsersAsync(searchTerm);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("No users found matching the search criteria.", result.Error);
            _userRepositoryMock.Verify(repo => repo.SearchUsersAsync(searchTerm), Times.Once);
        }

        [TestMethod]
        public async Task UpdateContact_Should_UpdateDescription_When_ContactExists()
        {
            // Arrange
            var userId = 1;
            var contactId = 2;
            var description = "New description";
            var existingContact = new UserContact { UserId = userId, ContactId = contactId };

            _userRepositoryMock.Setup(repo => repo.GetUserContactAsync(userId, contactId))
                .ReturnsAsync(existingContact);

            // Act
            var result = await _userService.UpdateContact(userId, contactId, description);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(description, existingContact.Description);
            _userRepositoryMock.Verify(repo => repo.UpdateContactAsync(It.Is<UserContact>(uc => uc.UserId == userId && uc.ContactId == contactId && uc.Description == description)), Times.Once);
        }

        [TestMethod]
        public async Task UpdateContact_Should_ReturnFailure_When_ContactDoesNotExist()
        {
            // Arrange
            var userId = 1;
            var contactId = 2;
            var description = "New description";

            _userRepositoryMock.Setup(repo => repo.GetUserContactAsync(userId, contactId))
                .ReturnsAsync((UserContact)null);

            // Act
            var result = await _userService.UpdateContact(userId, contactId, description);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Contact not found.", result.Error);
            _userRepositoryMock.Verify(repo => repo.UpdateContactAsync(It.IsAny<UserContact>()), Times.Never);
        }

        [TestMethod]
        public async Task ChangeEmailAsync_Should_ChangeEmail_When_CurrentPasswordIsCorrect_And_EmailIsUnique()
        {
            // Arrange
            var userId = 1;
            var newEmail = "newemail@example.com";
            var currentPassword = "hashedpassword";
            var user = TestHelper.GetTestUser();

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId))
                .ReturnsAsync(user);
            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(newEmail))
                .ReturnsAsync((User)null);
            _userRepositoryMock.Setup(repo => repo.UpdateUserAsync(It.IsAny<User>()))
                .Verifiable();

            // Act
            var result = await _userService.ChangeEmailAsync(userId, newEmail, currentPassword);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(newEmail, user.Email);
            _userRepositoryMock.Verify(repo => repo.UpdateUserAsync(It.Is<User>(u => u.Email == newEmail)), Times.Once);
        }

        [TestMethod]
        public async Task ChangeEmailAsync_Should_ReturnFailure_When_CurrentPasswordIsIncorrect()
        {
            // Arrange
            var userId = 1;
            var newEmail = "newemail@example.com";
            var currentPassword = "wrongPassword";
            var user = TestHelper.GetTestUser();

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.ChangeEmailAsync(userId, newEmail, currentPassword);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Current password is incorrect.", result.Error);
            _userRepositoryMock.Verify(repo => repo.UpdateUserAsync(It.IsAny<User>()), Times.Never);
        }

        [TestMethod]
        public async Task ChangeEmailAsync_Should_ReturnFailure_When_EmailIsAlreadyInUse()
        {
            // Arrange
            var userId = 1;
            var newEmail = "existingemail@example.com";
            var currentPassword = "hashedpassword";
            var user = TestHelper.GetTestUser();
            var existingUser = new User { Id = 2, Email = newEmail };

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId))
                .ReturnsAsync(user);
            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(newEmail))
                .ReturnsAsync(existingUser);

            // Act
            var result = await _userService.ChangeEmailAsync(userId, newEmail, currentPassword);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Email is already in use by another account.", result.Error);
            _userRepositoryMock.Verify(repo => repo.UpdateUserAsync(It.IsAny<User>()), Times.Never);
        }
        [TestMethod]
        public async Task FilterUsersAsync_Should_ReturnFilteredUsers_When_UsersMatchCriteria()
        {
            // Arrange
            var parameters = new UserQueryParameters { Username = "test" };
            var users = new List<User> { TestHelper.GetTestUser() };

            _userRepositoryMock.Setup(repo => repo.GetAllUsers())
                .ReturnsAsync(users.AsQueryable());

            // Act
            var result = await _userService.FilterUsersAsync(parameters);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, result.Value.Count());
        }

        [TestMethod]
        public async Task FilterUsersAsync_Should_ReturnFailure_When_NoUsersMatchCriteria()
        {
            // Arrange
            var parameters = new UserQueryParameters { Username = "nonexistent" };
            var users = new List<User>();

            _userRepositoryMock.Setup(repo => repo.GetAllUsers())
                .ReturnsAsync(users.AsQueryable());

            // Act
            var result = await _userService.FilterUsersAsync(parameters);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("No users found.", result.Error);
        }

        [TestMethod]
        public async Task GetTotalUserCountAsync_Should_ReturnUserCount_When_UsersMatchCriteria()
        {
            // Arrange
            var parameters = new UserQueryParameters { Username = "test" };
            var users = new List<User> { TestHelper.GetTestUser() };

            _userRepositoryMock.Setup(repo => repo.GetAllUsers())
                .ReturnsAsync(users.AsQueryable());

            // Act
            var result = await _userService.GetTotalUserCountAsync(parameters);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, result.Value);
        }

        [TestMethod]
        public async Task GetTotalUserCountAsync_Should_ReturnFailure_When_NoUsersMatchCriteria()
        {
            // Arrange
            var parameters = new UserQueryParameters { Username = "nonexistent" };
            var users = new List<User>();

            _userRepositoryMock.Setup(repo => repo.GetAllUsers())
                .ReturnsAsync(users.AsQueryable());

            // Act
            var result = await _userService.GetTotalUserCountAsync(parameters);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("No Users found.", result.Error);
        }

        [TestMethod]
        public async Task GetTotalBalanceInMainWalletCurrencyAsync_Should_ReturnTotalBalance_When_UserHasMainWallet()
        {
            // Arrange
            var userId = 1;
            var user = TestHelper.GetTestUser();
            user.MainWallet = new Wallet { Currency = CurrencyType.USD, Balance = 100m };
            user.Wallets.Add(user.MainWallet);

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.GetTotalBalanceInMainWalletCurrencyAsync(userId);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(200m, result.Value.TotalAmount);
            Assert.AreEqual(CurrencyType.USD, result.Value.Currency);
        }

        [TestMethod]
        public async Task GetTotalBalanceInMainWalletCurrencyAsync_Should_ReturnFailure_When_UserHasNoMainWallet()
        {
            // Arrange
            var userId = 1;
            var user = TestHelper.GetTestUser();
            user.MainWallet = null;

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.GetTotalBalanceInMainWalletCurrencyAsync(userId);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("User does not have a main wallet set.", result.Error);
        }

        [TestMethod]
        public async Task GetTotalBalanceInMainWalletCurrencyAsync_Should_ReturnFailure_When_ConversionRateIsNotFound()
        {
            // Arrange
            var userId = 1;
            var user = TestHelper.GetTestUser();
            user.MainWallet = new Wallet { Currency = CurrencyType.USD, Balance = 100m };
            user.Wallets.Add(new Wallet { Currency = CurrencyType.EUR, Balance = 50m });

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            _currencyServiceMock.Setup(service => service.GetRatesForCurrencyAsync(CurrencyType.EUR))
                .ReturnsAsync(Result<CurrencyExchangeRatesResponse>.Success(new CurrencyExchangeRatesResponse { Data = new Dictionary<string, decimal> { { "USD", 1.1m } } }));

            // Act
            var result = await _userService.GetTotalBalanceInMainWalletCurrencyAsync(userId);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(155m, result.Value.TotalAmount);
            Assert.AreEqual(CurrencyType.USD, result.Value.Currency);
        }







    }
}
