using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualWallet.DATA.Models.Enums;

namespace VirtualWallet.DATA.Models
{
    public class RealCard
    {
        public int Id { get; set; }
        public string CardNumber { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string CardHolderName { get; set; }
        public string CheckNumber { get; set; }
        public CardType CardType { get; set; }
        public decimal Balance { get; set; }
    }

}
