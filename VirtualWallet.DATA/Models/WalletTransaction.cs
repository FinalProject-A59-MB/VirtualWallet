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
    public class WalletTransaction:ITransactionEvent
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public CurrencyType Currency { get; set; }
        public TransactionStatus Status { get; set; }
        public string Origin { get; set; } // Example "Sender.Name"
        public string Destination { get; set; } // Example "Receiver.Name"

        public int SenderId { get; set; }
        public User Sender { get; set; }

        public int RecipientId { get; set; }
        public User Recipient { get; set; }

        public TransactionCategory Category { get; set; }

    }



}
