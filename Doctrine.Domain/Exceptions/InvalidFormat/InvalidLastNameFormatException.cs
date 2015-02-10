namespace Doctrine.Domain.Exceptions.InvalidFormat
{
    using System;

    public class InvalidLastNameFormatException : Exception
    {
        #region Constructors

        public InvalidLastNameFormatException()
        {
        }

        public InvalidLastNameFormatException(string message)
        : base(message)
        {
        }

        public InvalidLastNameFormatException(string message, Exception inner)
        : base(message, inner)
        {
        }

        #endregion
    }
}