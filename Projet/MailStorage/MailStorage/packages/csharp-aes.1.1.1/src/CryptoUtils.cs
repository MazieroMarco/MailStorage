using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace CsharpAes
{
    /// <summary>
    ///     Provides simple methods for encryption
    /// </summary>
    public static class CryptoUtils
    {
        private static int _rfc2898DeriveBytesIterations = 20000;

        /// <summary>
        ///     Number of iterations <see cref="Rfc2898DeriveBytes" /> does while
        ///     creating a key from password.
        /// </summary>
        /// <remarks>
        ///     At least 1 000 iterations are advised, default value is 20 000
        /// </remarks>
        public static int Rfc2898DeriveBytesIterations
        {
            get { return _rfc2898DeriveBytesIterations; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value), "The value must be grater than 0");

                _rfc2898DeriveBytesIterations = value;
            }
        }

        /// <summary>
        ///     Generates an encryption key from <paramref name="password" /> of any
        ///     length. EncryptionKey is 256 bits long.
        /// </summary>
        /// <param name="password">string- based password</param>
        /// <param name="salt">Salt to add</param>
        /// <param name="length">length of the resulting key</param>
        /// <returns>
        ///     EncryptionKey of fixed size 256bit
        /// </returns>
        public static byte[] DeriveKey(string password, byte[] salt, int length)
        {
            using (var alg = new Rfc2898DeriveBytes(password, salt))
            {
                alg.IterationCount = Rfc2898DeriveBytesIterations;
                return alg.GetBytes(length);
            }
        }

        /// <summary>
        /// Derive a key from another key and <paramref name="salt"/>
        /// </summary>
        /// <param name="password">password as bytes to derive from</param>
        /// <param name="salt">salt to derive from</param>
        /// <param name="length">of which length should output be</param>
        /// <returns></returns>
        public static byte[] DeriveKey(byte[] password, byte[] salt, int length)
        {
            using (var alg=new Rfc2898DeriveBytes(password,salt,Rfc2898DeriveBytesIterations))
            {
                return alg.GetBytes(length);
            }
        }

        /// <summary>
        ///     Generate cryptographically strong random bytes, typically salt
        /// </summary>
        /// <returns>
        ///     a random salt of specified <paramref name="length" />
        /// </returns>
        public static byte[] GenerateBytes(int length)
        {
            var salt = new byte[length];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(salt);
            }
            return salt;
        }

        /// <summary>
        ///     Constant-time array comparison. Returns if arrays are identical
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>
        /// </returns>
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public static bool SlowCompare(Array a, Array b)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            if (b == null) throw new ArgumentNullException(nameof(b));

            var same = a.Length == b.Length;
            var shorterLength = a.Length <= b.Length ? a.Length : b.Length;

            for (var i = 0; i < shorterLength; i++)
                if (!a.GetValue(i).Equals(b.GetValue(i)))
                    same = false;

            return same;
        }

        /// <summary>
        ///     Constant-time string comparison. Returns if string are identical
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>
        /// </returns>
        public static bool SlowCompare(string a, string b)
            => SlowCompare(a.ToCharArray(), b.ToCharArray());
    }
}