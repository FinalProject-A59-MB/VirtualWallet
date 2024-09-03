using VirtualWallet.DATA.Models;
using VirtualWallet.BUSINESS.Results;
using VirtualWallet.DATA.Models.Enums;

namespace VirtualWallet.DATA.Services.Contracts
{
    public interface IUserService
    {
        public Task<Result<IEnumerable<User>>> GetUsers();

        Task<Result<IEnumerable<User>>> GetUserContacts(int userid);
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

        Task<Result> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<Result> ChangeEmailAsync(int userId, string newEmail, string currentPassword);

        public Task<Result<IEnumerable<User>>> FilterUsersAsync(UserQueryParameters parameters);
        public Task<Result<int>> GetTotalUserCountAsync(UserQueryParameters parameters);

        public Task<Result<(decimal TotalAmount, CurrencyType Currency)>> GetTotalBalanceInMainWalletCurrencyAsync(int userId);

    }
}
