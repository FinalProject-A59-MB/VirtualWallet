using System.Text.RegularExpressions;
using VirtualWallet.BUSINESS.Results;

namespace VirtualWallet.BUSINESS.Helpers
{
    public class PasswordValidator
    {
        public static Result Validate(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return Result.Failure("Password cannot be empty.");
            }

            if (password.Length < 8)
            {
                return Result.Failure("Password must be at least 8 characters long.");
            }

            if (!Regex.IsMatch(password, @"[A-Z]"))
            {
                return Result.Failure("Password must contain at least one uppercase letter.");
            }

            if (!Regex.IsMatch(password, @"\d"))
            {
                return Result.Failure("Password must contain at least one digit.");
            }

            if (!Regex.IsMatch(password, @"[^a-zA-Z0-9\s]"))
            {
                return Result.Failure("Password must contain at least one special character.");
            }

            return Result.Success();
        }
    }
}
