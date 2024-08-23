using Microsoft.EntityFrameworkCore;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Repositories.Contracts;

namespace VirtualWallet.DATA.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        private IQueryable<User> GetUsersWithDetails()
        {
            return _context.Users
                .Include(u => u.UserProfile)
                .Include(u => u.Cards)
                .Include(u => u.BlockedRecord)
                .Include(u => u.Wallets)
                .Include(u => u.MainWallet);
        }

        public IQueryable<User> GetAllUsers()
        {
            return GetUsersWithDetails();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await GetUsersWithDetails()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await GetUsersWithDetails()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await GetUsersWithDetails()
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task AddUserProfileAsync(UserProfile userProfile)
        {
            _context.UserProfiles.Add(userProfile);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int userId)
        {
            var user = await GetUserByIdAsync(userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<UserProfile?> GetUserProfileAsync(int userId)
        {
            return await _context.UserProfiles
                .FirstOrDefaultAsync(up => up.UserId == userId);
        }

        public async Task UpdateUserProfileAsync(UserProfile userProfile)
        {
            _context.UserProfiles.Update(userProfile);
            await _context.SaveChangesAsync();
        }

        public async Task AddBlockedRecordAsync(BlockedRecord blockedRecord)
        {
            await _context.BlockedRecords.AddAsync(blockedRecord);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<BlockedRecord>> GetBlockedRecordsAsync(int userId)
        {
            return await _context.BlockedRecords
                .Where(br => br.UserId == userId)
                .ToListAsync();
        }
        public async Task AddContactAsync(UserContact userContact)
        {
            _context.UserContacts.Add(userContact);
            await _context.SaveChangesAsync();
        }

        public async Task<List<User>> GetUserContactsAsync(int userId)
        {
            return await _context.UserContacts
                .Where(uc => uc.UserId == userId)
                .Select(uc => uc.Contact)
                .ToListAsync();
        }

        public async Task<UserContact> GetUserContactAsync(int userId, int contactId)
        {
            return await _context.UserContacts
                .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.ContactId == contactId);
        }

        public async Task RemoveContactAsync(UserContact userContact)
        {
            _context.UserContacts.Remove(userContact);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsContactExistsAsync(int userId, int contactId)
        {
            return await _context.UserContacts
                .AnyAsync(uc => uc.UserId == userId && uc.ContactId == contactId);
        }
    }
}

