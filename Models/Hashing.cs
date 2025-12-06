using System.Security.Cryptography;
using System.Text;

namespace fitnessCenter.Models
{
    public static class PasswordHasher
    {
        // 1. Define a STATIC Salt (Instead of Random)
        // This acts like a "Secret Key". Do not lose this, or all passwords become invalid.
        private static readonly byte[] FixedSalt = Encoding.UTF8.GetBytes("MySuperSecretFixedSaltKey123!");

        public static string HashPassword(string password)
        {
            // 2. Use the Fixed Salt
            // This ensures "12345" always produces the exact same string.
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                FixedSalt,
                100000,
                HashAlgorithmName.SHA256,
                32
            );

            return Convert.ToBase64String(hash);
        }

        // With a fixed salt, you don't need a complex Verify function.
        // You just hash the input and check if it matches the database string.
        public static bool VerifyPassword(string inputPassword, string storedHash)
        {
            string newHash = HashPassword(inputPassword);
            return newHash == storedHash;
        }
    }
}