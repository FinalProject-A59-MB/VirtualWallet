using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualWallet.DATA.Models.Enums;

namespace VirtualWallet.DATA.Models
{
    public class WalletTransactionQueryParameters
    {
        public int? WalletId { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
        public int? SenderId { get; set; }
        public User? Sender { get; set; }
        public int? RecipientId { get; set; }
        public User? Recipient { get; set; }
        public TransactionStatus? Status { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; } = "desc";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? TransactionDirection { get; set; }
    }
}
