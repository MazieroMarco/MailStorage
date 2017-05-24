using System;
using System.IO;
using System.Text;

namespace CsharpAes
{
    /// <summary>
    ///     Container class for extension methods
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        ///     Append bytes encoded as base64 string
        /// </summary>
        /// <param name="builder">to perform action on</param>
        /// <param name="append">what should be appended</param>
        public static void AppendBase64(this StringBuilder builder, params byte[] append)
        {
            if ((builder == null) || (append == null)) return;
            builder.Append(Convert.ToBase64String(append));
        }

        /// <summary>
        ///     Checks if <paramref name="stream" /> satisfies given conditions,
        ///     throws appropriate exceptions when not.
        /// </summary>
        /// <param name="stream">stream to check</param>
        /// <param name="checkFor">what to check for</param>
        /// <param name="paramName">name of the parameter for exceptions</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="checkFor" /> has
        ///     <see cref="CsharpAes.StreamCheck.Null" /> and
        ///     <paramref name="stream" /> is null.
        /// </exception>
        /// <exception cref="NullReferenceException">
        ///     Thrown when <paramref name="stream" /> is <see langword="null" /> but
        ///     <paramref name="checkFor" /> doesn't have Null
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown when <paramref name="stream" /> doesn't satisfy given
        ///     conditions
        /// </exception>
        // ReSharper disable once UnusedParameter.Global
        public static void ArgumentCheck(this Stream stream, StreamCheck checkFor, string paramName)
        {
            if (string.IsNullOrEmpty(paramName)) throw new ArgumentNullException(nameof(paramName));
            if (checkFor.HasFlag(StreamCheck.None)) return;

            if (checkFor.HasFlag(StreamCheck.Null) && (stream == null))
                throw new ArgumentNullException(paramName);
            if (stream == null)
                throw new NullReferenceException("Unable to check on null");

            if (checkFor.HasFlag(StreamCheck.Read) && !stream.CanRead)
                throw new ArgumentException("Stream must support reading", paramName);

            if (checkFor.HasFlag(StreamCheck.Write) && !stream.CanWrite)
                throw new ArgumentException("Stream must support writing", paramName);
        }
    }

    /// <summary>
    ///     Specifies for what exactly should be a stream tested for.
    /// </summary>
    [Flags]
    public enum StreamCheck
    {
        /// <summary>
        ///     Don't test
        /// </summary>
        None = 0,

        /// <summary>
        ///     Test if is not <see langword="null" />
        /// </summary>
        Null = 1,

        /// <summary>
        ///     Test for reading
        /// </summary>
        Read = 2,

        /// <summary>
        ///     Test for writing
        /// </summary>
        Write = 4
    }
}