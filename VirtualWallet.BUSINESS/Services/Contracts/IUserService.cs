﻿using VirtualWallet.DATA.Models;
using VirtualWallet.BUSINESS.Results;

namespace VirtualWallet.DATA.Services.Contracts
{
    public interface IUserService
    {
        public Task<Result<IQueryable<User>>> GetUsers();
        public Task<Result<User>> GetUserByIdAsync(int userId);

        public Task<Result<User>> GetUserByUsernameAsync(string userName);

        public Task<Result<User>> GetUserByEmailAsync(string userName);

        public Task<Result<User>> RegisterUserAsync(User user);

        public Task<Result<UserProfile>> GetUserProfileAsync(int userId);

        public Task<Result> UpdateUserAsync(User user);

        public Task<Result> UpdateUserProfileAsync(UserProfile userProfile);

        public Task<Result> DeleteUserAsync(int userId);

        public Task<Result> SendFriendRequestAsync(int userId, int contactId);
        public Task<Result> AcceptFriendRequestAsync(int userId, int contactId);
        public Task<Result> DenyFriendRequestAsync(int userId, int contactId);

        public Task<Result<IEnumerable<UserContact>>> GetPendingFriendRequestsAsync(int userId);

        public Task<Result<IEnumerable<User>>> SearchUsersAsync(string searchTerm);

        public Task<Result> UpdateContact(int userId, int contactId, string description);
    }
}
