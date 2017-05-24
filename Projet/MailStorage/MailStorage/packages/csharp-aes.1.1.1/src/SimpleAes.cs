using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CsharpAes
{
    /// <summary>
    ///     Simple wrapper around <see cref="RijndaelManaged" /> class. Uses 256-bit
    ///     keys.
    /// </summary>
    public class SimpleAes : IDisposable
    {
        /// <summary>
        ///     Gets the length of
        ///     <see cref="SymmetricAlgorithm.IV" /> .
        ///     Bytes of this length are prepended to every encrypted string and
        ///     contain iv itself
        /// </summary>
        /// <remarks>
        ///     Length of arrray, not bits
        /// </remarks>
        public const int IVLength = BlockSize / 8;

        /// <summary>
        ///     Gets the default key size
        /// </summary>
        public const int DefaultKeySize = 256;

        /// <summary>
        ///     Gets the min length to be deciphered
        /// </summary>
        /// <remarks>
        ///     Length of the array, not in bits
        /// </remarks>
        public const int MinimumCipherLength = BlockSize / 8;

        /// <summary>
        ///     Gets the block size of <c>Rijndael</c> alg in bits
        /// </summary>
        public const int BlockSize = 128;

        /// <summary>
        ///     Gets the valid key sizes in bits.
        /// </summary>
        public static readonly int[] ValidKeySizes = { 128, 192, 256 };

        private readonly RijndaelManaged _rijndael;

        /// <summary>
        ///     Encoding used to get bytes/text from one another. UTF-8.
        /// </summary>
        protected readonly Encoding Encoding = Encoding.UTF8;

        private bool _disposed;

        /// <summary>
        ///     New <see cref="SimpleAes" /> with known <paramref name="key" /> and
        ///     <see cref="IV" />
        /// </summary>
        /// <param name="key"><see cref="Key" /> used to encrypt/decrypt</param>
        /// <param name="iv">
        ///     <see cref="IV" /> used to encrypt/decrypt. Use <see langword="null" />
        ///     to generate random
        /// </param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="key" /> or <paramref name="iv" /> are of invalid size</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key" /> is null</exception>
        public SimpleAes(byte[] key, byte[] iv = null)
        {
            _rijndael = DefaultRijndaelInstance();
            Key = key;
            if (iv == null)
                _rijndael.GenerateIV();
            else
                IV = iv;
        }

        /// <summary>
        ///     New <see cref="SimpleAes" /> with random
        ///     <see cref="SimpleAes.Key" /> and
        ///     <see cref="SimpleAes.IV" />
        /// </summary>
        public SimpleAes()
        {
            _rijndael = DefaultRijndaelInstance();

            _rijndael.GenerateKey();
            ResetIV();
        }

        /// <summary>
        ///     Gets or sets the key in use.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when <paramref name="value" /> is of invalid size</exception>
        /// ///
        /// <exception cref="ArgumentNullException">Trown when <paramref name="value" /> is null</exception>
        public byte[] Key
        {
            get { return _rijndael?.Key; }
            set
            {
                CheckDisposed();
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (!ValidateKeySize(value))
                    throw new ArgumentException("Key length is invalid!");
                _rijndael.Key = value;
            }
        }

        /// <summary>
        ///     Gets or sets the initialization vector in use
        /// </summary>
        /// <remarks>
        ///     The array length must be <see cref="IVLength" />
        /// </remarks>
        public byte[] IV
        {
            get { return _rijndael?.IV; }
            set
            {
                CheckDisposed();
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (value.Length != IVLength)
                    throw new ArgumentException("Invalid iv size");
                _rijndael.IV = value;
            }
        }

        #region IDisposable Members

        /// <summary>
        ///     <see cref="Dispose(bool)" />
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        ///     Releases underlying resources
        /// </summary>
        /// <param name="disposing">whether dispose wrapped managed classes</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
                _rijndael.Dispose();
            _disposed = true;
        }

        /// <summary>
        ///     Encrypts contents of <paramref name="source" /> and writes them to <paramref name="destination" />.
        /// </summary>
        /// <param name="source">data to encrypt</param>
        /// <param name="destination">where to copy to</param>
        /// <param name="closeStream">if destination stream should be closed</param>
        /// <exception cref="ArgumentException">
        ///     Thrown when <paramref name="source" /> is not readable or
        ///     <paramref name="destination" /> is not writable
        /// </exception>
        /// <exception cref="ArgumentNullException">Thrown when either of streams is null</exception>
        public void Encrypt(Stream source, Stream destination, bool closeStream = true)
        {
            CheckDisposed();
            source.ArgumentCheck(StreamCheck.Null | StreamCheck.Read, nameof(source));
            destination.ArgumentCheck(StreamCheck.Write | StreamCheck.Null, nameof(destination));

            var encryptor = _rijndael.CreateEncryptor();

            var encStream = new CryptoStream(destination, encryptor, CryptoStreamMode.Write);
            try
            {
                source.CopyTo(encStream);
            }
            finally
            {
                if (!encStream.HasFlushedFinalBlock)
                    encStream.FlushFinalBlock();
                if (closeStream)
                    encStream.Dispose();
            }
        }

        /// <summary>
        ///     Encrypts string to bytes using 256bit <c>Rijndael</c> .
        /// </summary>
        /// <param name="plainText">Text to be encoded</param>
        /// <returns>
        ///     Cipher in bytes
        /// </returns>
        public byte[] Encrypt(string plainText)
        {
            if (plainText == null) throw new ArgumentNullException(nameof(plainText));

            using (var destination = new MemoryStream())
            {
                using (var source = new MemoryStream(Encoding.GetBytes(plainText)))
                {
                    Encrypt(source, destination);
                    return destination.ToArray();
                }
            }
        }

        /// <summary>
        ///     Decrypts contents of <paramref name="cipherSource" /> and writes them to <paramref name="destination" />.
        /// </summary>
        /// <param name="cipherSource">data to encrypt</param>
        /// <param name="destination">where to copy to</param>
        /// <param name="closeStream">if destination stream should be closed</param>
        /// <exception cref="ArgumentException">
        ///     Thrown when <paramref name="cipherSource" /> is not readable or
        ///     <paramref name="destination" /> is not writable
        /// </exception>
        /// <exception cref="ArgumentNullException">Thrown when either of streams is null</exception>
        public void Decrypt(Stream cipherSource, Stream destination, bool closeStream = true)
        {
            CheckDisposed();
            cipherSource.ArgumentCheck(StreamCheck.Null | StreamCheck.Read, nameof(cipherSource));
            destination.ArgumentCheck(StreamCheck.Null | StreamCheck.Write, nameof(destination));

            // decryptor that transforms the stream
            var decryptor = _rijndael.CreateDecryptor();

            // decryption stream
            var decryptStream = new CryptoStream(cipherSource, decryptor, CryptoStreamMode.Read);

            try
            {
                //write contents of cipherSource to destination while decrypting them on the way
                decryptStream.CopyTo(destination);
            }
            catch (CryptographicException)
            {
                throw new IncorrectPasswordException();
            }
            finally
            {
                if (!decryptStream.HasFlushedFinalBlock)
                    decryptStream.FlushFinalBlock();
                if (closeStream)
                    decryptStream.Dispose();
            }
        }

        /// <summary>
        ///     Decrypts <paramref name="cipher" /> back to plaintext
        /// </summary>
        /// <param name="cipher">Cipher to be decrypted.</param>
        /// <returns>
        ///     Decrypted string
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="cipher" /> is null</exception>
        /// <exception cref="IncorrectPasswordException">Thrown when encryption key doesn't correspond to the correct one</exception>
        public string Decrypt(byte[] cipher)
        {
            if (cipher == null) throw new ArgumentNullException(nameof(cipher));

            using (var destination = new MemoryStream())
            {
                using (var source = new MemoryStream(cipher))
                {
                    Decrypt(source, destination);
                    return Encoding.GetString(destination.ToArray());
                }
            }
        }

        /// <summary>
        ///     Generate new <see cref="IV" />. This should be called before each repetitive encryption.
        /// </summary>
        public void ResetIV()
        {
            CheckDisposed();
            _rijndael.GenerateIV();
        }

        /// <summary>
        ///     Validates the length size of <paramref name="key" />
        /// </summary>
        /// <param name="key">key to validate</param>
        /// <returns>
        ///     if <paramref name="key" /> is valid
        /// </returns>
        protected static bool ValidateKeySize(byte[] key)
            => ValidKeySizes.Contains(key?.Length * 8 ?? -1);

        /// <summary>
        ///     Gets a default instance of <see cref="RijndaelManaged" /> with
        ///     explicitly set block and key sizes.
        /// </summary>
        /// <remarks>
        ///     Block size=128, key size=256
        /// </remarks>
        /// <returns>
        /// </returns>
        protected static RijndaelManaged DefaultRijndaelInstance()
            => new RijndaelManaged
            {
                BlockSize = BlockSize,
                KeySize = DefaultKeySize
            };

        /// <summary>
        ///     Checks if instance has already been disposed and throws <see cref="ObjectDisposedException" /> if yes.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown when instance has been disposed</exception>
        protected void CheckDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }

        /// <inheritdoc />
        ~SimpleAes()
        {
            Dispose(false);
        }
    }
}