namespace Doctrine.Domain.Exceptions.InvalidFormat
{
    using System;

    public class InvalidTopicNameFormatException : Exception
    {
        #region Constructors

        public InvalidTopicNameFormatException()
        {
        }

        public InvalidTopicNameFormatException(string message)
        : base(message)
        {
        }

        public InvalidTopicNameFormatException(string message, Exception inner)
        : base(message, inner)
        {
        }

        #endregion
    }
}