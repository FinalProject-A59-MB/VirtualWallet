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
        User GetUserById(int id);
        User GetUserByEmail(string email);
        User GetUserByUsername(string username);
        IEnumerable<User> GetAllUsers();
        void AddUser(User user);
        void UpdateUser(User user);
        void DeleteUser(int userId);

        UserProfile GetUserProfile(int userId);
        void UpdateUserProfile(UserProfile userProfile);

        void AddBlockedRecord(BlockedRecord blockedRecord);
        IEnumerable<BlockedRecord> GetBlockedRecords(int userId);
    }
}
