using System;
using System.Security.Cryptography;
using System.Text;

namespace VirtualWallet.DATA.Helpers
{
    public static class PasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;

        public static string HashPassword(string password)
        {
            byte[] saltBytes = new byte[SaltSize];
            RandomNumberGenerator.Fill(saltBytes);

            using (var sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] saltedPassword = new byte[saltBytes.Length + passwordBytes.Length];

                Buffer.BlockCopy(saltBytes, 0, saltedPassword, 0, saltBytes.Length);
                Buffer.BlockCopy(passwordBytes, 0, saltedPassword, saltBytes.Length, passwordBytes.Length);

                byte[] hashBytes = sha256.ComputeHash(saltedPassword);
                byte[] hashWithSaltBytes = new byte[saltBytes.Length + hashBytes.Length];
                Buffer.BlockCopy(saltBytes, 0, hashWithSaltBytes, 0, saltBytes.Length);
                Buffer.BlockCopy(hashBytes, 0, hashWithSaltBytes, saltBytes.Length, hashBytes.Length);
                return Convert.ToBase64String(hashWithSaltBytes);
            }
        }

        public static bool VerifyPassword(string password, string storedHash)
        {

            byte[] hashWithSaltBytes = Convert.FromBase64String(storedHash);
            byte[] saltBytes = new byte[SaltSize];
            Buffer.BlockCopy(hashWithSaltBytes, 0, saltBytes, 0, SaltSize);

            using (var sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] saltedPassword = new byte[saltBytes.Length + passwordBytes.Length];

                Buffer.BlockCopy(saltBytes, 0, saltedPassword, 0, saltBytes.Length);
                Buffer.BlockCopy(passwordBytes, 0, saltedPassword, saltBytes.Length, passwordBytes.Length);

                byte[] hashBytes = sha256.ComputeHash(saltedPassword);
                byte[] storedHashBytes = new byte[HashSize];
                Buffer.BlockCopy(hashWithSaltBytes, SaltSize, storedHashBytes, 0, HashSize);
                for (int i = 0; i < HashSize; i++)
                {
                    if (hashBytes[i] != storedHashBytes[i])
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}