using System.Text.RegularExpressions;
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

            string emailPattern = @"^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$";

            if (!Regex.IsMatch(email, emailPattern))
            {
                return Result.Failure("Email format is invalid.");
            }

            return Result.Success();
        }
    }
}
