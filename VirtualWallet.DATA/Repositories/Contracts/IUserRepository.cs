using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public Task UpdateUserAsync(User user);

        public Task DeleteUserAsync(int userId);

        public Task<UserProfile?> GetUserProfileAsync(int userId);

        public Task UpdateUserProfileAsync(UserProfile userProfile);

        public Task AddBlockedRecordAsync(BlockedRecord blockedRecord);

        public Task<IEnumerable<BlockedRecord>> GetBlockedRecordsAsync(int userId);
    }
}
