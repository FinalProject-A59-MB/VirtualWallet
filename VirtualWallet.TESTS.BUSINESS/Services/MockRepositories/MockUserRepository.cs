using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Repositories.Contracts;

namespace VirtualWallet.TESTS.BUSINESS.Services.MockRepositories
{
    public class MockUserRepository
    {
        public Mock<IUserRepository> GetMockRepository()
        {
            var mockRepository = new Mock<IUserRepository>();

            var sampleUsers = new List<User> { TestHelper.GetTestUser() };

            mockRepository.Setup(x => x.GetAllUsers())
                          .ReturnsAsync(sampleUsers);

            mockRepository.Setup(x => x.GetUserByIdAsync(It.IsAny<int>()))
                          .ReturnsAsync((int id) => sampleUsers.FirstOrDefault(user => user.Id == id));

            mockRepository.Setup(x => x.GetUserByEmailAsync(It.IsAny<string>()))
                          .ReturnsAsync((string email) => sampleUsers.FirstOrDefault(user => user.Email == email));

            mockRepository.Setup(x => x.GetUserByUsernameAsync(It.IsAny<string>()))
                          .ReturnsAsync((string username) => sampleUsers.FirstOrDefault(user => user.Username == username));

            mockRepository.Setup(x => x.AddUserAsync(It.IsAny<User>()))
                          .Callback((User user) => sampleUsers.Add(user))
                          .Returns(Task.CompletedTask);

            mockRepository.Setup(x => x.AddUserProfileAsync(It.IsAny<UserProfile>()))
                          .Callback((UserProfile profile) =>
                          {
                              var user = sampleUsers.FirstOrDefault(u => u.Id == profile.UserId);
                              if (user != null)
                              {
                                  user.UserProfile = profile;
                              }
                          })
                          .Returns(Task.CompletedTask);

            mockRepository.Setup(x => x.UpdateUserAsync(It.IsAny<User>()))
                          .Callback((User user) =>
                          {
                              var existingUser = sampleUsers.FirstOrDefault(u => u.Id == user.Id);
                              if (existingUser != null)
                              {
                                  existingUser.Username = user.Username;
                                  existingUser.Email = user.Email;
                                  existingUser.Password = user.Password;
                                  existingUser.UserProfile = user.UserProfile;
                              }
                          })
                          .Returns(Task.CompletedTask);

            mockRepository.Setup(x => x.DeleteUserAsync(It.IsAny<int>()))
                          .Callback((int id) =>
                          {
                              var userToRemove = sampleUsers.FirstOrDefault(u => u.Id == id);
                              if (userToRemove != null)
                              {
                                  sampleUsers.Remove(userToRemove);
                              }
                          })
                          .Returns(Task.CompletedTask);

            mockRepository.Setup(x => x.SearchUsersAsync(It.IsAny<string>()))
                          .ReturnsAsync((string searchTerm) =>
                          {
                              return sampleUsers.Where(u => u.Username.Contains(searchTerm) || u.Email.Contains(searchTerm)).ToList();
                          });

            mockRepository.Setup(x => x.GetUserProfileAsync(It.IsAny<int>()))
                          .ReturnsAsync((int userId) =>
                          {
                              var user = sampleUsers.FirstOrDefault(u => u.Id == userId);
                              return user?.UserProfile;
                          });


            return mockRepository;
        }
    }
}
