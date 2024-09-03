using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualWallet.BUSINESS.Helpers
{
    public class VerificationCode
    {
        public static string Generate()
        {
            Random random = new Random();
            int verificationCode = random.Next(1000, 10000);
            return verificationCode.ToString();
        }
    }
}
