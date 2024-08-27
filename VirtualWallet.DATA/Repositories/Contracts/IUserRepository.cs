using VirtualWallet.DATA.Models;

namespace VirtualWallet.DATA.Repositories.Contracts
{
    public interface IUserRepository
    {
        public IQueryable<User> GetAllUsers();

        public Task<User?> GetUserByIdAsync(int id);

        public Task<User?> GetUserByEmailAsync(string email);

        public Task<User?> GetUserByUsernameAsync(string username);

        public Task AddUserAsync(User user);
        public Task AddUserProfileAsync(UserProfile userProfile);

        public Task UpdateUserAsync(User user);

        public Task DeleteUserAsync(int userId);

        public Task<UserProfile?> GetUserProfileAsync(int userId);

        public Task UpdateUserProfileAsync(UserProfile userProfile);

        public Task AddBlockedRecordAsync(BlockedRecord blockedRecord);

        public Task<IEnumerable<BlockedRecord>> GetBlockedRecordsAsync(int userId);

        public Task AddContactAsync(UserContact userContact);
        public Task UpdateContactAsync(UserContact userContact);
        public Task<List<User>> GetUserContactsAsync(int userId);

        public Task<UserContact> GetUserContactAsync(int userId, int contactId);

        public Task RemoveContactAsync(UserContact userContact);

        public Task<bool> IsContactExistsAsync(int userId, int contactId);

        public Task<IEnumerable<User>> SearchUsersAsync(string searchTerm);

        public Task<IEnumerable<UserContact>> GetPendingFriendRequestsAsync(int userId);
    }
}
