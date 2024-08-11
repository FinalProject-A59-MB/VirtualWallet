using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualWallet.DATA.Models
{
    public class UserContact
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int ContactId { get; set; }
        public User Contact { get; set; }

        public DateTime AddedDate { get; set; }
    }

}
