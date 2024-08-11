using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualWallet.DATA.Models.Enums;

namespace VirtualWallet.DATA.Models
{
    public class Card
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string CardNumber { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string CardHolderName { get; set; }
        public string CheckNumber { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public CardType CardType { get; set; }

        public ICollection<CardTransaction> CardTransactions { get; set; }
        public string PaymentProcessorToken { get; set; }
    }

}
