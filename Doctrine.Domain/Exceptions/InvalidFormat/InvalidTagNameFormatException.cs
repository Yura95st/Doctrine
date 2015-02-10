namespace Doctrine.Domain.Exceptions.InvalidFormat
{
    using System;

    public class InvalidTagNameFormatException : Exception
    {
        #region Constructors

        public InvalidTagNameFormatException()
        {
        }

        public InvalidTagNameFormatException(string message)
        : base(message)
        {
        }

        public InvalidTagNameFormatException(string message, Exception inner)
        : base(message, inner)
        {
        }

        #endregion
    }
}