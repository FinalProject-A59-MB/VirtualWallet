using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using VirtualWallet.DATA.Models.Contract;
using VirtualWallet.DATA.Models.Enums;
using TransactionStatus = VirtualWallet.DATA.Models.Enums.TransactionStatus;

namespace VirtualWallet.DATA.Models
{
    public class WalletTransaction : ITransactionEvent
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public CurrencyType Currency { get; set; }
        public TransactionStatus Status { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; } 

        public int WalletId { get; set; } 
        public Wallet Wallet { get; set; } 
        public TransactionCategory Category { get; set; }

        public int SenderWalletId { get; set; }
        public int RecipientWalletId { get; set; }
        public Wallet SenderWallet { get; set; }
        public Wallet RecipientWallet { get; set; }
    }



}
