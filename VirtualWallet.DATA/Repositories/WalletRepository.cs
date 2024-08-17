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
        public void AddWallet(Wallet wallet)
        {
            _dbContext.Wallets.Add(wallet);
            _dbContext.SaveChanges();
        }

        public Wallet GetWalletById(int id)
        {
           return _dbContext.Wallets.FirstOrDefault(w => w.Id == id);
        }

        public Wallet GetWalletByName(string walletName)
        {
            return _dbContext.Wallets.FirstOrDefault(w => w.Name == walletName);
        }

        public IEnumerable<Wallet> GetWalletsByUserId(int userId)
        {
            List<Wallet> wallets = _dbContext.Wallets.Where(w => w.UserId == userId).ToList();
            
            List<UserWallet> jointWallets = _dbContext.UserWallets.Include(w => w.Wallet).Where(x => x.UserId == userId).ToList();

            wallets.AddRange(jointWallets.Select(x => x.Wallet));

            return wallets;
        }

        public void RemoveWallet(int walletId)
        {
            var wallet = _dbContext.Wallets.FirstOrDefault(w => w.Id == walletId);

            if (wallet == null)
            {
                throw new Exception();
            }

            if(wallet.WalletType != Models.Enums.WalletType.Joint)
            {
                User user = _userRepository.GetUserById(wallet.UserId);

                //TODO transfer the money from the wallet back to the user card
            }

            _dbContext.Wallets.Remove(wallet);
            _dbContext.SaveChanges();
        }

        public void UpdateWallet(int walletId, Wallet wallet)
        {
            var walletToUpdate = GetWalletById(walletId);

            if (walletToUpdate == null)
            {
                throw new Exception();
            }

            walletToUpdate.Currency = wallet.Currency;
            walletToUpdate.Balance = wallet.Balance;
            walletToUpdate.Name = wallet.Name;

            // What can we update?
        }
    }
}
