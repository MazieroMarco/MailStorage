using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace CsharpAes
{
    /// <summary>
    ///     Represents a wrapper around <see cref="SimpleAes" /> that has a single string input and single string output.
    /// </summary>
    public partial class AdvancedAes : IDisposable
    {
        /// <summary>
        ///     Current version of algorithm and header
        /// </summary>
        public const byte CurrentAlgVersion = 1;

        /// <summary>
        ///     Length of <see cref="EncryptionKey" />. 256 bits = 32 bytes
        /// </summary>
        public const int EncryptionKeyLength = 32;

        /// <summary>
        ///     It can be any value considered safe. Length of <see cref="EncryptionSalt" />
        /// </summary>
        protected const int EncryptionSaltLength = 64;

        /// <summary>
        ///     Length of <see cref="HMACSalt" />
        /// </summary>
        protected const int HMACSaltLength = 64;

        /// <summary>
        ///     Length of <c>HMAC</c> key. 64 is suggested, so keep it that way.
        /// </summary>
        protected const int HMACKeyLength = 64;

        /// <summary>
        ///     Length of IV
        /// </summary>
        private const int IVLength = SimpleAes.IVLength;

        /// <summary>
        ///     Separates blocks of output/input strings
        /// </summary>
        public const char Separator = '$';

        /// <summary>
        ///     What is <see langword="this" /> class wrapper around
        /// </summary>
        private readonly SimpleAes _aes;

        /// <summary>
        ///     Encoding to convert input from/to byte array. (UTF-8)
        /// </summary>
        protected readonly Encoding InputEncoding = Encoding.UTF8;

        /// <summary>
        ///     Encoding used internally to interract with IO
        /// </summary>
        protected readonly Encoding InternalEncoding = Encoding.ASCII;

        /// <summary>
        ///     If instance has been disposed yet
        /// </summary>
        private bool _disposed;

        /// <summary>
        ///     Initializes a new instance of <see cref="AdvancedAes" />
        /// </summary>
        /// <param name="key">
        ///     Encryption hmacKey. Must be 256 bits long. Pass null to generate
        ///     random.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="key" /> is not 256 bits long</exception>
        public AdvancedAes(byte[] key = null)
        {
            if (key != null)
                ValidateEncryptionKey(key);

            _aes = new SimpleAes();
            if (key != null)
                EncryptionKey = key;

            EncryptionSalt = CryptoUtils.GenerateBytes(EncryptionSaltLength);
            HMACSalt = CryptoUtils.GenerateBytes(HMACSaltLength);
        }

        /// <summary>
        ///     Gets the Key used for encryption.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value" /> is not 256 bits long</exception>
        public byte[] EncryptionKey
        {
            get { return _aes.Key; }
            protected set
            {
                CheckDisposed();
                if (value == null) throw new ArgumentNullException(nameof(value));
                ValidateEncryptionKey(value);
                _aes.Key = value;
            }
        }

        /// <summary>
        ///     Gets salt for encryption/decryption key generation
        /// </summary>
        protected byte[] EncryptionSalt { get; set; }

        /// <summary>
        ///     Gets or sets the current algorithm version
        /// </summary>
        public byte AlgorithmVersion { get; set; } = CurrentAlgVersion;

        /// <summary>
        ///     Gets or sets the salt for <c>HMAC</c>
        /// </summary>
        protected byte[] HMACSalt { get; set; }

        /// <summary>
        ///     Computes HMACSHA265 of given <paramref name="input" /> using given hmacKey
        /// </summary>
        /// <param name="input">data to HMAC</param>
        /// <param name="hmacKey">key to use for HMAC. Ideal length is 64 bytes</param>
        /// <returns></returns>
        protected virtual byte[] ComputeHMAC(Stream input, byte[] hmacKey)
        {
            using (var hmac = new HMACSHA256(hmacKey))
            {
                return hmac.ComputeHash(input);
            }
        }

        /// <summary>
        ///     Generates a key of length <see cref="HMACKeyLength" />
        /// </summary>
        /// <param name="salt">hmac salt</param>
        /// <param name="encryptionKey">encryption key to generate HMAC hmacKey from</param>
        /// <returns></returns>
        protected virtual byte[] GenerateHMACKey(byte[] salt, byte[] encryptionKey)
            => CryptoUtils.DeriveKey(encryptionKey, salt, HMACKeyLength);

        /// <summary>
        ///     Compute <c>SHA1</c> checksum of <paramref name="source" /> and return it as a base64 string
        /// </summary>
        /// <param name="source">what to compute checksum of</param>
        /// <returns>base64 of SHA1 checksum of <paramref name="source" /></returns>
        protected virtual string ComputeChecksum(Stream source)
        {
            using (var alg = new SHA1Managed())
            {
                var hash = alg.ComputeHash(source);
                return Convert.ToBase64String(hash);
            }
        }

        /// <summary>
        ///     Checks is <paramref name="key" /> is 256 bits long and if not, <see cref="ArgumentOutOfRangeException" /> is thrown
        /// </summary>
        /// <param name="key"></param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="key" /> is not 256 bits long</exception>
        private static void ValidateEncryptionKey(byte[] key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            if (key.Length != EncryptionKeyLength)
                throw new ArgumentOutOfRangeException(nameof(key), "EncryptionKey must be 256 bits long");
        }

        /// <summary>
        ///     Performs an encryption on streams
        /// </summary>
        /// <param name="source">What to encrypt. Contents will be read from the set position.</param>
        /// <param name="destination">Where should the encryption result be. The content will be appended after set position.</param>
        /// <param name="closeStreams">whether stream should be closed once the method has finished</param>
        /// <exception cref="IOException">Thrown when an <see cref="IOException" /> is thrown upon encrypting.</exception>
        /// <remarks>
        ///     Calls <see cref="ResetVariables" /> after executing i.e. IV and salts are generated newly.
        /// </remarks>
        public void Encrypt(Stream source, Stream destination, bool closeStreams = true)
        {
            try
            {
                CheckDisposed();
                source.ArgumentCheck(StreamCheck.Null | StreamCheck.Read, nameof(source));
                destination.ArgumentCheck(StreamCheck.Read | StreamCheck.Write, nameof(destination));

                //save steams' positions to allow appending to existing streams
                var originalSourcePosition = source.Position;
                var originalDestinationPosition = destination.Position;
                destination.SetLength(originalDestinationPosition);

                #region assembling header

                var sb = new StringBuilder();

                // 1) version
                sb.AppendBase64(AlgorithmVersion);
                sb.Append(Separator);

                // 2) IV
                sb.AppendBase64(_aes.IV);
                sb.Append(Separator);

                // 3) encryption salt
                sb.AppendBase64(EncryptionSalt);
                sb.Append(Separator);

                // 4) HMAC salt
                sb.AppendBase64(HMACSalt);
                sb.Append(Separator);

                // 5) encrypted checksum

                var checksum = ComputeChecksum(source);
                var encryptedChecksum = _aes.Encrypt(checksum);
                sb.AppendBase64(encryptedChecksum);
                sb.Append(Separator);

                #endregion

                //header is complete
                var header = InternalEncoding.GetBytes(sb.ToString());

                #region writing to stream

                var writer = new BinaryWriter(destination);

                //write headers
                writer.Write(header);

                //write encrypted content

                source.Position = originalSourcePosition;
                var cs = new CryptoStream(destination, new ToBase64Transform(), CryptoStreamMode.Write);
                _aes.Encrypt(source, cs, false);
                if (!cs.HasFlushedFinalBlock)
                    cs.FlushFinalBlock();

                #region HMAC

                destination.Flush();

                //if we didn't move to the origin, empty string would be HMACed
                destination.Position = originalDestinationPosition;

                var hmacKey = GenerateHMACKey(HMACSalt, EncryptionKey);
                //compute HMAC of everything so far written to destination
                var hmac = ComputeHMAC(destination, hmacKey);

                //base64 encrypted HMAC prepended with $
                var end = Separator + GetString(hmac);
                writer.Write(InternalEncoding.GetBytes(end));

                #endregion

                writer.Flush();

                #endregion
            }
            catch (IOException ioException)
            {
                throw new IOException("An I/O exception was thrown while encrypting", ioException);
            }
            finally
            {
                if (closeStreams)
                {
                    source.Close();
                    destination.Close();
                }
                ResetVariables();
            }
        }

        /// <summary>
        ///     Encrypts string to base64 using 256bit <c>Rijndael</c>
        /// </summary>
        /// <param name="plainText">Text to be encrypted</param>
        /// <returns>Cipher as base64 string</returns>
        public string Encrypt(string plainText)
        {
            CheckDisposed();
            if (plainText == null) throw new ArgumentNullException(nameof(plainText));

            using (var destinationStream = new MemoryStream())
            {
                using (var sourceStream = new MemoryStream(InputEncoding.GetBytes(plainText)))
                {
                    Encrypt(sourceStream, destinationStream, false);
                    return InternalEncoding.GetString(destinationStream.ToArray());
                }
            }
        }

        /// <summary>
        ///     Encrypts an <see langword="object" /> of type T.
        ///     serialized to XML.
        /// </summary>
        /// <typeparam name="T">Type of object to serialize</typeparam>
        /// <param name="obj">Object to be serialized</param>
        /// <returns>Encrypted <paramref name="obj" /></returns>
        /// <remarks>
        ///     Uses <see cref="XmlSerializer" />
        /// </remarks>
        public string Encrypt<T>(T obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            var ser = new XmlSerializer(typeof(T));
            using (var sw = new StringWriter())
            {
                ser.Serialize(sw, obj);
                return Encrypt(sw.ToString());
            }
        }

        /// <summary>
        ///     Decrypts a product of <see cref="Encrypt(System.IO.Stream,System.IO.Stream,bool)" /> stored in a stream into
        ///     <paramref name="destination" /> without loading ciphertext into memory. Very large files can be encrypted this way
        ///     with minimum memory usage.
        /// </summary>
        /// <param name="cipher">Encrypted contents, the contents must be at the end of the stream.</param>
        /// <param name="destination">
        ///     Stream to write to. Must be writable and readable. Contents are written starting with
        ///     position.
        /// </param>
        /// <param name="closeStreams">Whether streams should be closed after method finishes.</param>
        /// <param name="buffer">
        ///     Temporary storage. Pass null to use a buffer in memory. Note that it will be emptied in the
        ///     beginning and full in the end.
        /// </param>
        /// <exception cref="IncorrectPasswordException">
        ///     Thrown when given key/password won't decrypt contents or HMAC/plaintext
        ///     checksum authentication fails.
        /// </exception>
        /// <exception cref="FormatException">Thrown <paramref name="cipher" /> is in an invalid format.</exception>
        /// <exception cref="IOException">Thrown when an I/O exception occurrs while decrypting.</exception>
        public void Decrypt(Stream cipher, Stream destination, bool closeStreams = true, Stream buffer = null)
        {
            CheckDisposed();
            cipher.ArgumentCheck(StreamCheck.Null | StreamCheck.Read, nameof(cipher));
            destination.ArgumentCheck(StreamCheck.Null | StreamCheck.Write | StreamCheck.Read, nameof(destination));
            buffer?.ArgumentCheck(StreamCheck.Read | StreamCheck.Write, nameof(buffer));

            if (buffer == null)
                buffer = new MemoryStream();

            try
            {
                //destroy all contents of buffer
                buffer.Position = 0;
                buffer.SetLength(0);

                //save the position stream was at when handed over
                var originalCipherPosition = cipher.Position;
                var originalDestinationPosition = destination.Position;
                destination.SetLength(originalDestinationPosition);

                TextReader reader = new StreamReader(cipher, InternalEncoding);
                //number of base64 encoded parts separated by $
                const int headerPartsCount = 5;

                #region loading header sections as raw bytes

                //header parts as strings loading
                var headerParts = new string[headerPartsCount];
                for (var i = 0; i < headerPartsCount; i++)
                {
                    var part = ReadUntilSeparator(reader);
                    if (part.Length == 0) //end of stream
                        throw new FormatException($"invalid number of parts separated by {Separator}. Stopped at {i}");
                    headerParts[i] = part;
                }

                //actual parts are byte arrays
                var header = new byte[headerPartsCount][];
                for (var i = 0; i < headerParts.Length; i++)
                    header[i] = Convert.FromBase64String(headerParts[i]);

                #endregion

                var versionArray = header[0];
                var iv = header[1];
                var encryptionSalt = header[2];
                var hmacSalt = header[3];
                var checksum = header[4];

                #region sections validation

                if (versionArray.Length != 1)
                    throw new FormatException("Version must be 1 byte long.")
                    {
                        Data =
                        {
                            {"bad length", versionArray.Length}
                        }
                    };
                if (iv.Length != IVLength)
                    throw new FormatException($"IV must be {IVLength} bytes long.")
                    {
                        Data =
                        {
                            {"bad length", iv.Length}
                        }
                    };
                if (encryptionSalt.Length != EncryptionSaltLength)
                    throw new FormatException($"Encryption salt must be {EncryptionSaltLength} long.")
                    {
                        Data =
                        {
                            {"bad length", encryptionSalt.Length}
                        }
                    };
                if (hmacSalt.Length != HMACSaltLength)
                    throw new FormatException($"HMAC salt length must be {HMACSaltLength}.")
                    {
                        Data =
                        {
                            {"bad length", hmacSalt.Length}
                        }
                    };

                #endregion

                var version = versionArray[0];
                if (version != 1)
                    throw new NotSupportedException(
                        "You are attempting to decrypt a cipher that is incompatible with this library. Make sure you are running the latest version.");

                var decryptionKey = GenerateDecryptionKey(encryptionSalt);
                var hmacKey = GenerateHMACKey(hmacSalt, decryptionKey);

                var bufferWriter = new StreamWriter(buffer, InternalEncoding);

                #region write everything except HMAC to buffer

                foreach (var headerPart in headerParts)
                    bufferWriter.Write(headerPart + Separator);
                CopyUntilSeparator(reader, bufferWriter);

                //whole cipher except last separator and HMAC is written in buffer stream

                #endregion

                #region read HMAC from the rest of the cipher

                if (cipher.Length - cipher.Position - 1 > ReaderMaxLength)
                    throw new FormatException("Last sector representing HMAC is too long. Reading aborted.");
                /*
             * remaining contents are much longer than expected, abort reading
             * so that the string won't eat up all the memory
             */

                byte[] expectedHmac;
                try
                {
                    //only HMAC is remaining
                    expectedHmac = GetBytes(reader.ReadToEnd());
                }
                catch (FormatException exception)
                {
                    throw new FormatException("Unable to convert HMAC from base64", exception);
                }

                #endregion

                #region HMAC verification

                //buffer stream has been populated with data that are to be HMACed

                buffer.Position = 0;
                var computedHmac = ComputeHMAC(buffer, hmacKey);
                if (!CryptoUtils.SlowCompare(expectedHmac, computedHmac))
                    throw new IncorrectPasswordException("HMAC authentication failed");

                #endregion

                //clear buffer stream
                buffer.SetLength(0);
                buffer.Flush();

                #region save encryption variables

                var encryptionKey = new byte[EncryptionKey.Length];
                var encryptionIv = new byte[_aes.IV.Length];
                EncryptionKey.CopyTo(encryptionKey, 0);
                _aes.IV.CopyTo(encryptionIv, 0);

                #endregion

                //clear buffer
                buffer.SetLength(0);
                try
                {
                    _aes.IV = iv;
                    _aes.Key = decryptionKey;

                    //checksum was originally encrypted to eliminate any attacks on checksum
                    var decryptedChecksum = _aes.Decrypt(checksum);

                    #region write ciphertext to buffer

                    cipher.Position = originalCipherPosition;
                    for (var i = 0; i < headerPartsCount; i++)
                        ReadUntilSeparator(reader);
                    //get position to the beginning of ciphertext (after header)

                    //copy ciphertext to buffer as base64 decoded
                    var fromBase64 = new CryptoStream(buffer, new FromBase64Transform(), CryptoStreamMode.Write);

                    //actual writing
                    CopyUntilSeparator(reader, new StreamWriter(fromBase64, InternalEncoding));
                    if (!fromBase64.HasFlushedFinalBlock)
                        fromBase64.FlushFinalBlock();
                    buffer.Flush();

                    #endregion

                    buffer.Position = 0;
                    _aes.Decrypt(buffer, destination, false);

                    #region checksum verification

                    destination.Position = originalDestinationPosition;
                    var computedChecksum = ComputeChecksum(destination);
                    if (computedChecksum != decryptedChecksum)
                        throw new IncorrectPasswordException(
                            "The decryption key works, but plaintext authentication failed.");

                    #endregion

                    destination.Flush();
                }
                finally
                {
                    //set variables back as they were changed for decryption purposes
                    EncryptionKey = encryptionKey;
                    _aes.IV = encryptionIv;
                }
            }
            catch (IOException ioException)
            {
                throw new IOException("An IO exception occurred during decryption.", ioException);
            }
            finally
            {
                if (closeStreams)
                {
                    destination.Close();
                    cipher.Close();
                    buffer.Close();
                }
            }
        }

        /// <summary>
        ///     Decrypt <paramref name="cipher" /> made by
        ///     <see cref="Encrypt(string)" /> with known
        ///     <see>
        ///         <cref>_salt</cref>
        ///     </see>
        ///     and password.
        /// </summary>
        /// <param name="cipher">Cipher to be decrypted.</param>
        /// <returns>Decrypted string</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when
        ///     <paramref name="cipher" /> is null
        /// </exception>
        /// <exception cref="IncorrectPasswordException">
        ///     Thrown when encryption
        ///     hmacKey doesn't correspond to the correct one
        /// </exception>
        /// <exception cref="FormatException">Thrown when cipher is badly formated</exception>
        public string Decrypt(string cipher)
        {
            using (var destination = new MemoryStream())
            {
                using (var cipherStream = new MemoryStream(InternalEncoding.GetBytes(cipher)))
                {
                    //write deciphered bytes into destination stream
                    Decrypt(cipherStream, destination, false);

                    return InputEncoding.GetString(destination.ToArray());
                }
            }
        }

        /// <summary>
        ///     Decrypts previously encrypted instance.
        /// </summary>
        /// <typeparam name="T"><c>Type</c> of instance serialized</typeparam>
        /// <param name="cipher">ciphertext</param>
        /// <returns>Deserialized instance</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="cipher" /> is null</exception>
        /// <exception cref="IncorrectPasswordException">Thrown when encryption hmacKey doesn't correspond to the correct one</exception>
        /// <exception cref="FormatException">Thrown when cipher is badly formated</exception>
        public T Decrypt<T>(string cipher)
        {
            var dec = Decrypt(cipher);

            var serializer = new XmlSerializer(typeof(T));
            using (TextReader tr = new StringReader(dec))
            {
                return (T) serializer.Deserialize(tr);
            }
        }

        /// <summary>
        ///     Generates new salts and IV. Called after each encryption automatically)
        /// </summary>
        protected virtual void ResetVariables()
        {
            CheckDisposed();
            EncryptionSalt = CryptoUtils.GenerateBytes(EncryptionSaltLength);
            HMACSalt = CryptoUtils.GenerateBytes(HMACSaltLength);
            _aes.ResetIV();
        }

        /// <summary>
        ///     Generate a decryption hmacKey from <paramref name="salt" /> saved in cipher
        /// </summary>
        /// <param name="salt">salt from cipher</param>
        /// <returns>hmacKey for decryption</returns>
        protected virtual byte[] GenerateDecryptionKey(byte[] salt)
        {
            return EncryptionKey;
        }
    }
}