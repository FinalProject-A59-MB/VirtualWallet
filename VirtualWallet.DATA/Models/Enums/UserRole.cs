using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualWallet.DATA.Models.Enums
{
    public enum UserRole
    {
        RegisteredUser,
        EmailVerifiedUser, 
        PendingVerification, 
        VerifiedUser, 
        VerificationStaff, 
        Admin,
        Blocked
    }


}
