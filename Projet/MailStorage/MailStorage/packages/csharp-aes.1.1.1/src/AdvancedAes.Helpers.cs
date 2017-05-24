using System;
using System.IO;
using System.Text;

namespace CsharpAes
{
    //this file has rather helper methods that are not directly related to cryptowork
    public partial class AdvancedAes
    {
        /// <summary>
        ///     Max number of characters StreamReaders read upon decrypting before realizing it's invalid garbage.
        /// </summary>
        private const int ReaderMaxLength = 2048;

        #region IDisposable Members

        /// <summary>
        ///     Releases underlying resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        ///     Releases underlying resources and marks instance as disposed, i.e. many methods won't be available
        /// </summary>
        /// <param name="disposing">if resources should be actually disposed</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
                _aes?.Dispose();

            _disposed = true;
        }

        /// <summary>
        ///     Reads from <paramref name="reader" /> until separator or end of the stream
        /// </summary>
        /// <param name="reader">where to read</param>
        /// <param name="abortAfter">
        ///     max number of character before aborting (e.g. wrong format is passed and you might want to
        ///     avoid chewing throw GB of badly formated data)
        /// </param>
        /// <returns>the text excluding separator</returns>
        private static string ReadUntilSeparator(TextReader reader, int abortAfter = ReaderMaxLength)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < abortAfter; i++)
            {
                var read = reader.Read();

                if (read == -1)
                    break;
                var ch = (char) read;
                if (ch == Separator)
                    break;
                sb.Append(ch);
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Copy read contents by <paramref name="sourceReader" /> and write them with <see cref="destinationWriter" /> until
        ///     <see cref="Separator" /> is hit (exclusive).
        /// </summary>
        /// <param name="sourceReader">read with/from</param>
        /// <param name="destinationWriter">write with/to</param>
        private static void CopyUntilSeparator(TextReader sourceReader, TextWriter destinationWriter)
        {
            while (true)
            {
                var nextChar = sourceReader.Read();
                if ((nextChar == -1) || (nextChar == Separator)) break;
                destinationWriter.Write((char) nextChar);
            }
            destinationWriter.Flush();
        }

        /// <summary>
        ///     Checks if instance has already been disposed and throws <see cref="ObjectDisposedException" /> if yes.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown when object has been disposed</exception>
        protected void CheckDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName,
                    "This instance has already been disposed. You may reuse a single instance for multiple encryptions/decryptions");
        }

        /// <summary>
        /// Get base64 representation of <paramref name="bytes"/>
        /// </summary>
        /// <param name="bytes">what to base64</param>
        /// <returns></returns>
        private static string GetString(byte[] bytes)
            => Convert.ToBase64String(bytes);

        /// <summary>
        /// Converts <paramref name="base64"/> string back to string
        /// </summary>
        /// <param name="base64">to convert from base64</param>
        /// <returns></returns>
        private static byte[] GetBytes(string base64)
            => Convert.FromBase64String(base64);

        /// <inheritdoc />
        ~AdvancedAes()
        {
            Dispose(false);
        }
    }
}