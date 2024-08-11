using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualWallet.DATA.Models
{
    public class SavingsWallet: Wallet
    {
        public decimal Interest { get; set; }
    }
}
