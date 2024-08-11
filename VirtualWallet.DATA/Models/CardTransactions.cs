using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualWallet.DATA.Models.Contract;
using VirtualWallet.DATA.Models.Enums;

namespace VirtualWallet.DATA.Models
{
    public class CardTransaction : ITransactionEvent
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public CurrencyType Currency { get; set; }
        public TransactionStatus Status { get; set; }
        public string Origin { get; set; } //Example: "From Wallet.Name"
        public string Destination { get; set; }//Example: "To Card.Name or Card.Number"
        //----------------------------------------//
        public int WalletId { get; set; }
        public Wallet Wallet { get; set; }     
        public int CardId { get; set; }
        public Card Card { get; set; }        
        public TransactionType TransactionType { get; set; }
        
    }

}
