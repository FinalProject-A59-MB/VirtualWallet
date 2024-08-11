using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualWallet.DATA.Models.Enums;

namespace VirtualWallet.DATA.Models
{
    public class RecurringPayment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public decimal Amount { get; set; }
        public CurrencyType Currency { get; set; }
        public int RecipientId { get; set; }
        public User Recipient { get; set; }
        public Frequency Frequency { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime NextPaymentDate { get; set; }
        public bool IsActive { get; set; }
        public int? WalletId { get; set; }
        public Wallet Wallet { get; set; }
    }



}
