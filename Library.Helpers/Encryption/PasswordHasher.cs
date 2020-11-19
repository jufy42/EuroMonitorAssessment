using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;

namespace Library.Helpers
{
    public sealed class PasswordHasher<TUser> : IPasswordHasher<TUser> where TUser : class
    {
        private const int _NUMBER_OF_SALT_BYTES = 32;
        private const int _NUMBER_OF_HASH_BYTES = 32;
        private const int _NUMBER_OF_PBKDF2_ITERATIONS = 32000;
        private readonly int _numberOfSaltBytes;
        private readonly int _numberOfHashBytes;
        private readonly int _numberOfPbkdf2Iterations;

        public PasswordHasher()
        {
            _numberOfSaltBytes = _NUMBER_OF_SALT_BYTES;
            _numberOfHashBytes = _NUMBER_OF_HASH_BYTES;
            _numberOfPbkdf2Iterations = _NUMBER_OF_PBKDF2_ITERATIONS;
        }

        public string HashPassword(TUser user, string password)
        {
            if (password == null)
                throw new ArgumentNullException(nameof(password));
            byte[] salt;
            byte[] bytes;
            using (Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, _numberOfSaltBytes, _numberOfPbkdf2Iterations))
            {
                salt = rfc2898DeriveBytes.Salt;
                bytes = rfc2898DeriveBytes.GetBytes(_numberOfHashBytes);
            }

            byte[] inArray = new byte[1 + _numberOfSaltBytes + this._numberOfHashBytes];
            inArray[0] = 0;
            Buffer.BlockCopy(salt, 0, inArray, 1, _numberOfSaltBytes);
            Buffer.BlockCopy(bytes, 0, inArray, 1 + _numberOfSaltBytes, _numberOfHashBytes);
            return Convert.ToBase64String(inArray);
        }

        public PasswordVerificationResult VerifyHashedPassword(
            TUser user,
            string hashedPassword,
            string providedPassword)
        {
            if (hashedPassword == null)
                return PasswordVerificationResult.Failed;
            if (providedPassword == null)
                throw new ArgumentNullException(nameof(providedPassword));
            byte[] numArray = Convert.FromBase64String(hashedPassword);
            if (numArray.Length != 1 + this._numberOfSaltBytes + _numberOfHashBytes || numArray[0] > 0)
                return PasswordVerificationResult.Failed;
            byte[] salt = new byte[_numberOfSaltBytes];
            Buffer.BlockCopy(numArray, 1, salt, 0, _numberOfSaltBytes);
            byte[] originalHash = new byte[_numberOfHashBytes];
            Buffer.BlockCopy(numArray, 1 + this._numberOfSaltBytes, originalHash, 0, _numberOfHashBytes);
            byte[] bytes;
            using (Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(providedPassword, salt, _numberOfPbkdf2Iterations))
                bytes = rfc2898DeriveBytes.GetBytes(_numberOfHashBytes);
            return SlowEquals(originalHash, bytes) ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
        }

        public string DecryptText(string value) => value;

        public string EncryptText(string value) => value;

        private static bool SlowEquals(byte[] originalHash, byte[] comparisonHash)
        {
            uint num = (uint) (originalHash.Length ^ comparisonHash.Length);
            for (int index = 0; index < originalHash.Length && index < comparisonHash.Length; ++index)
                num |= originalHash[index] ^ (uint) comparisonHash[index];
            return num == 0U;
        }

    }
}
