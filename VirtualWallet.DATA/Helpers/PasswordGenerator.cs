using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualWallet.DATA.Helpers
{
    public class PasswordGenerator
    {
        private static readonly Random _random = new Random();
        private const string UpperCaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string LowerCaseChars = "abcdefghijklmnopqrstuvwxyz";
        private const string DigitChars = "0123456789";
        private const string SpecialChars = "+-*&^%$#@!";

        public static string GenerateSecurePassword(int length = 8)
        {
            if (length < 8)
            {
                throw new ArgumentException("Password length must be at least 8 characters.");
            }

            var passwordBuilder = new StringBuilder();

            // Ensure at least one character of each required type
            passwordBuilder.Append(UpperCaseChars[_random.Next(UpperCaseChars.Length)]);
            passwordBuilder.Append(LowerCaseChars[_random.Next(LowerCaseChars.Length)]);
            passwordBuilder.Append(DigitChars[_random.Next(DigitChars.Length)]);
            passwordBuilder.Append(SpecialChars[_random.Next(SpecialChars.Length)]);

            // Fill the remaining characters
            var allChars = UpperCaseChars + LowerCaseChars + DigitChars + SpecialChars;
            for (int i = 4; i < length; i++)
            {
                passwordBuilder.Append(allChars[_random.Next(allChars.Length)]);
            }

            // Shuffle the characters to avoid predictable patterns
            return new string(passwordBuilder.ToString().OrderBy(_ => _random.Next()).ToArray());
        }
    }
}
