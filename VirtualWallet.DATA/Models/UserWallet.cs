using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualWallet.DATA.Models.Enums;

namespace VirtualWallet.DATA.Models
{
    public class UserWallet
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int WalletId { get; set; }
        public Wallet Wallet { get; set; }

        public UserWalletRole Role { get; set; }
        public DateTime JoinedDate { get; set; }
    }


}
