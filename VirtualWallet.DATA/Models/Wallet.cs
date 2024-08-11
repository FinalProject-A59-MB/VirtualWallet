using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualWallet.DATA.Models.Contract;
using VirtualWallet.DATA.Models.Enums;

namespace VirtualWallet.DATA.Models
{
    public class Wallet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public WalletType WalletType { get; set; }
        public decimal Balance { get; set; }
        public CurrencyType Currency { get; set; }
        public ICollection<UserWallet> UserWallets { get; set; }
        public ICollection<WalletTransaction> WalletTransactions { get; set; }
        public ICollection<CardTransaction> CardTransactions { get; set; }
    }





}
