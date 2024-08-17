using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Repositories.Contracts;

namespace VirtualWallet.DATA.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IUserRepository _userRepository;

        public WalletRepository(ApplicationDbContext dbContext, IUserRepository userRepository)
        {
            _dbContext = dbContext;
            _userRepository = userRepository;
        }


        public async Task AddWalletAsync(Wallet wallet)
        {
            _dbContext.Wallets.Add(wallet);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Wallet> GetWalletByIdAsync(int id)
        {
            var wallet = await _dbContext.Wallets.FirstOrDefaultAsync(w => w.Id == id);

            if (wallet == null)
            {
                throw new Exception();
            }

            return wallet;
        }

        public async Task<Wallet> GetWalletByNameAsync(string walletName)
        {
            var wallet = await _dbContext.Wallets.FirstOrDefaultAsync(w => w.Name == walletName);

            if (wallet == null)
            {
                throw new Exception();
            }

            return wallet;
        }

        public async Task<IEnumerable<Wallet>> GetWalletsByUserIdAsync(int userId)
        {
            var wallets = await _dbContext.Wallets.Where(w => w.UserId == userId).ToListAsync();

            var jointWallets = await _dbContext.UserWallets.Include(w => w.Wallet).Where(x => x.UserId == userId).ToListAsync();

            wallets.AddRange(jointWallets.Select(x => x.Wallet));

            return wallets;
        }

        public async Task RemoveWalletAsync(int walletId)
        {
            var wallet = await GetWalletByIdAsync(walletId);

            if(wallet.WalletType != Models.Enums.WalletType.Joint)
            {
                var user = await _userRepository.GetUserByIdAsync(wallet.UserId);

                //TODO transfer the money from the wallet back to the user card
            }

            _dbContext.Wallets.Remove(wallet);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateWalletAsync(Wallet wallet)
        {
            _dbContext.Update(wallet);
            await _dbContext.SaveChangesAsync();
        }
    }
}
