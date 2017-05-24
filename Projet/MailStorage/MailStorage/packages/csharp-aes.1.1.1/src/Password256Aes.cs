using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using CsharpAes;

// ReSharper disable once CheckNamespace
namespace CsEncryption
{
    /// <summary>
    ///     Class used to encrypt/decrypt LONG strings using password.
    ///     Speed is ~20.1 MBps
    /// </summary>
    [Obsolete("This class is flawed but cannot be removed because of backwards compatibility. Use PasswordAes instead")]
    public class Password256Aes : IDisposable
    {
        /// <summary>
        ///     Length generated by Sha1
        /// </summary>
        private const int ChecksumLength = 20;

        /// <summary>
        ///     It can be any value considered safe
        /// </summary>
        protected const int SaltLength = 32;

        private readonly SimpleAes _aes;

        /// <summary>
        ///     Initializes a new instance of <see cref="Password256Aes" /> class
        /// </summary>
        /// <param name="password">password protecting the string</param>
        public Password256Aes(string password)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));

            Password = password;
            _aes = new SimpleAes();

            Debug.WriteLine("Hello fellow developer! Class Password256Aes has been found to be severely flawed, please migrate to PasswordAes. The encryption results are not compatible, so you will have to manually convert any data encrypted with Password256Aes. Sorry for the inconvenience, but security goes first.");
        }

        /// <summary>
        ///     Password in use, not the EncryptionKey!
        /// </summary>
        public string Password { get; }

        /// <summary>
        ///     Length of IV. Bytes of this length are
        ///     prepended to every encryped string and contain IV itself
        /// </summary>
        private int IVLength => SimpleAes.IVLength;

        /// <summary>
        ///     Minimal length of cipher that can be passed for decryption in bytes.
        /// </summary>
        public int MinimalCipherLength => SimpleAes.MinimumCipherLength + ChecksumLength + SaltLength + IVLength;

        ///<inheritdoc/>
        public void Dispose()
        {
            _aes?.Dispose();
        }

        /// <summary>
        ///     Generates password from password of any length and hash. EncryptionKey is 256bit long.
        /// </summary>
        /// <param name="password">Any password</param>
        /// <param name="salt">Salt to add</param>
        /// <returns>EncryptionKey of fixed size 256bit</returns>
        protected byte[] GetKey(string password, byte[] salt)
        {
            const int keyLength = 32; //we use 256-bit AES = 32 bytes

            using (var pbkdf = new Rfc2898DeriveBytes(password, salt))
            {
                return pbkdf.GetBytes(keyLength);
            }
        }

        /// <summary>
        ///     Compute a checksum of a string
        /// </summary>
        /// <param name="data">string to be hash computed from</param>
        /// <returns>hash of length <see cref="ChecksumLength" /></returns>
        protected byte[] ComputeChecksum(string data)
        {
            using (var sha1 = new SHA1Managed())
                return sha1.ComputeHash(GetBytes(data));
        }

        /// <summary>
        ///     Generate random salt for password generation
        /// </summary>
        /// <returns>a random salt of length <see cref="SaltLength" /></returns>
        protected byte[] GetRandomSalt()
        {
            var salt = new byte[SaltLength];
            var rngCsp = new RNGCryptoServiceProvider();
            rngCsp.GetBytes(salt);
            return salt;
        }

        /// <summary>
        ///     Convert string to bytes
        /// </summary>
        /// <param name="input">string to be converted to bytes</param>
        /// <returns>string converted to bytes</returns>
        protected byte[] GetBytes(string input)
        {
            return Encoding.UTF8.GetBytes(input);
        }

        /// <summary>
        ///     Encrypts string to bytes using 256bit Rijndael. IV of size <see cref="IVLength" /> occupies first bytes,
        ///     then <see cref="SaltLength" /> salt, <see cref="ChecksumLength" /> checksum and finally encrypted content.
        /// </summary>
        /// <param name="plainText">Text to be encoded</param>
        /// <returns>Cipher in bytes</returns>
        public byte[] Encrypt(string plainText)
        {
            if (plainText == null) throw new ArgumentNullException(nameof(plainText));

            var salt = GetRandomSalt();
            _aes.Key = GetKey(Password, salt);

            //result of encryption
            var encBytes = _aes.Encrypt(plainText);

            //result to be returned
            var result = new List<byte>();

            //first go IV bytes
            result.AddRange(_aes.IV);

            //next goes salt used for EncryptionKey generation
            result.AddRange(salt);

            //next checksum
            result.AddRange(ComputeChecksum(plainText));

            //and finally the encrypted text
            result.AddRange(encBytes);

            return result.ToArray();
        }

        /// <summary>
        ///     Same as <see cref="Encrypt" /> but input argument is object serialized to XML.
        /// </summary>
        /// <typeparam name="T">Type of object to serialize</typeparam>
        /// <param name="obj">Object to be serialized</param>
        /// <returns>Encrypted <see cref="obj" /></returns>
        public byte[] Encrypt<T>(T obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            var ser = new XmlSerializer(typeof (T));
            using (var sw = new StringWriter())
            {
                ser.Serialize(sw, obj);
                return Encrypt(sw.ToString());
            }
        }

        /// <summary>
        ///     Decrypt <paramref name="cipher"/> made by <see cref="Encrypt" /> with known
        ///     <see>
        ///         <cref>_salt</cref>
        ///     </see>
        ///     and password.
        /// </summary>
        /// <param name="cipher">Cipher to be decrypted.</param>
        /// <returns>Decrypted string</returns>
        public string Decrypt(byte[] cipher)
        {
            if (cipher == null) throw new ArgumentNullException(nameof(cipher));
            if (cipher.Length < MinimalCipherLength)
                throw new ArgumentException($"Argument must be {MinimalCipherLength} bytes long at least!",
                    nameof(cipher));

            //IV
            var iv = new byte[IVLength];

            #region read first IVLength bytes which are IV

            for (var i = 0; i < IVLength; i++)
            {
                iv[i] = cipher[i];
            }

            #endregion

            //salt to generate key
            var salt = new byte[SaltLength];

            #region read 32 salt bytes

            for (var i = 0; i < SaltLength; i++)
            {
                salt[i] = cipher[IVLength + i];
            }

            #endregion

            //checksum to check that it's correct password
            var checksum = new byte[ChecksumLength];

            #region read 20 bytes of checksum

            for (var i = 0; i < ChecksumLength; i++)
            {
                checksum[i] = cipher[IVLength + SaltLength + i];
            }

            #endregion

            //actual bytes to be decrypted
            var encBytes = new byte[cipher.Length - IVLength - SaltLength - ChecksumLength];

            #region read bytes to be decrypted

            for (var i = 0; i < encBytes.Length; i++)
            {
                encBytes[i] = cipher[IVLength + SaltLength + ChecksumLength + i];
            }

            #endregion

            _aes.IV = iv;
            _aes.Key = GetKey(Password, salt);

            try
            {
                var decrypted = _aes.Decrypt(encBytes);
                if (!checksum.SequenceEqual(ComputeChecksum(decrypted)))
                    throw new IncorrectPasswordException(); //it is actually pretty unlikely but...

                return decrypted;
            }
            catch (CryptographicException)
            {
                throw new IncorrectPasswordException();
            }
        }

        /// <summary>
        ///     Deserializes <see langword="byte"/>[] generated by generic <see cref="Encrypt" />
        /// </summary>
        /// <typeparam name="T">Type of object serialized</typeparam>
        /// <param name="cipher">Output of <see cref="Encrypt" /></param>
        /// <returns>Deserialized object</returns>
        public T Decrypt<T>(byte[] cipher)
        {
            var dec = Decrypt(cipher);

            var serializer = new XmlSerializer(typeof (T));
            using (TextReader tr = new StringReader(dec))
            {
                return (T) serializer.Deserialize(tr);
            }
        }
    }
}