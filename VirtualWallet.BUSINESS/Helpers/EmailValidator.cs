using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VirtualWallet.BUSINESS.Results;

namespace VirtualWallet.BUSINESS.Helpers
{
    public class EmailValidator
    {
        public static Result Validate(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Result.Failure("Email cannot be empty.");
            }

            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            if (!Regex.IsMatch(email, emailPattern))
            {
                return Result.Failure("Email format is invalid.");
            }

            return Result.Success();
        }
    }
}
