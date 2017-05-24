using System;
using System.Security;
using System.Security.Cryptography;

namespace CsharpAes
{
    /// <summary>
    ///     Represents an AES encryption algorithm that uses string-based passwords to generate actual key
    /// </summary>
    public class PasswordAes : AdvancedAes
    {
        /// <summary>
        ///     Initializes a new instance of <see cref="PasswordAes" />
        /// </summary>
        /// <param name="password">password to generate key from</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="password" /> is null</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="password" /> is empty</exception>
        public PasswordAes(string password)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (password.Length == 0) throw new ArgumentException("Password cannot be empty", nameof(password));

            Password = password;
            EncryptionKey = CryptoUtils.DeriveKey(Password, EncryptionSalt, EncryptionKeyLength);
        }

        /// <summary>
        ///     Gets the password in use
        /// </summary>
        /// <remarks>
        ///     <see cref="SecureString" /> is not used because <see cref="Rfc2898DeriveBytes" /> doesn't support it
        /// </remarks>
        public string Password { get; }

        /// <inheritdoc />
        protected override byte[] GenerateDecryptionKey(byte[] salt)
        {
            return CryptoUtils.DeriveKey(Password, salt, EncryptionKeyLength);
        }

        /// <inheritdoc />
        protected override void ResetVariables()
        {
            base.ResetVariables();
            EncryptionKey = CryptoUtils.DeriveKey(Password, EncryptionSalt, EncryptionKeyLength);
        }
    }
}