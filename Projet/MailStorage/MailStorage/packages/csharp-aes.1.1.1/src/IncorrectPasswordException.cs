using System;

namespace CsharpAes
{
    /// <summary>
    /// Empty class. It is used to explicitly tell that it password is incorrect
    /// </summary>
    [Serializable]
    public class IncorrectPasswordException : Exception
    {
        /// <summary>
        /// Initializes a new instance of <see cref="IncorrectPasswordException"/>
        /// </summary>
        public IncorrectPasswordException() { }

        /// <summary>
        /// Initializes a new instance of <see cref="IncorrectPasswordException"/> with a message
        /// </summary>
        /// <param name="message"></param>
        public IncorrectPasswordException(string message) : base(message)
        {
        }
    }
}
