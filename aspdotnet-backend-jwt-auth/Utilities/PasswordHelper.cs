using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace JWTBackendAuth.Utilities
{
    public static class PasswordHelper
    {
        public static string Hash(string password, byte[] salt)
        {
            // Derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password!,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            return hashed;
        }

        public static byte[] GenerateSalt()
        {
            // Generate a 128 - bit salt using a sequence of cryptographically strong random bytes.
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8); // Divide by 8 to convert bits to bytes}
            return salt;
        }
    }
}
