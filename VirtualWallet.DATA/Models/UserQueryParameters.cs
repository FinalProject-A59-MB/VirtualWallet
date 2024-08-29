using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualWallet.DATA.Models.Enums;

namespace VirtualWallet.DATA.Models
{
    public class UserQueryParameters
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int PageSize { get; set; } = 10;
        public int PageNumber { get; set; } = 1;

        public UserVerificationStatus VerificationStatus { get; set; }

        public UserRole Role { get; set; }
    }
}
