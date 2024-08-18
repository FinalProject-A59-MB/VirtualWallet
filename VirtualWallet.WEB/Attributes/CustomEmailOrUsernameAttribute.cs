using System.ComponentModel.DataAnnotations;

namespace VirtualWallet.WEB.Attributes
{
    public class CustomEmailOrUsernameAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var input = value as string;
            if (string.IsNullOrEmpty(input))
            {
                return new ValidationResult("Username or email is required.");
            }
            if (!IsValidEmailOrUsername(input))
            {
                return new ValidationResult("Invalid username or email format.");
            }

            return ValidationResult.Success;
        }

        private bool IsValidEmailOrUsername(string input)
        {
            if (input.Contains("@"))
            {
                return IsValidEmail(input);
            }
            else
            {
                return IsValidUsername(input);
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidUsername(string username)
        {
            if (username.Length < 2 || username.Length > 20)
            {
                return false;
            }
            return username.All(char.IsLetterOrDigit);
        }
    }

}
