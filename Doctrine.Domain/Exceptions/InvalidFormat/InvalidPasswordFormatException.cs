namespace Doctrine.Domain.Exceptions.InvalidFormat
{
    using System;

    public class InvalidPasswordFormatException : Exception
    {
        #region Constructors

        public InvalidPasswordFormatException()
        {
        }

        public InvalidPasswordFormatException(string message)
        : base(message)
        {
        }

        public InvalidPasswordFormatException(string message, Exception inner)
        : base(message, inner)
        {
        }

        #endregion
    }
}