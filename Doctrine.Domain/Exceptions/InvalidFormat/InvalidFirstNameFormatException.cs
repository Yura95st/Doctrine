namespace Doctrine.Domain.Exceptions.InvalidFormat
{
    using System;

    public class InvalidFirstNameFormatException : Exception
    {
        #region Constructors

        public InvalidFirstNameFormatException()
        {
        }

        public InvalidFirstNameFormatException(string message)
        : base(message)
        {
        }

        public InvalidFirstNameFormatException(string message, Exception inner)
        : base(message, inner)
        {
        }

        #endregion
    }
}