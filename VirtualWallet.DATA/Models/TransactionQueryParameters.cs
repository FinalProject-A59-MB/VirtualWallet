using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualWallet.DATA.Models.Enums;

namespace VirtualWallet.DATA.Models
{
    public class TransactionQueryParameters
    {
        public User Sender { get; set; }
        public User Recipient { get; set; }
        public string Direction { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; } = "asc";
        public int PageSize { get; set; } = 10;
        public int PageNumber { get; set; } = 1;
    }
}
