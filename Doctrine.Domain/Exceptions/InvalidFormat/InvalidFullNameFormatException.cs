namespace Doctrine.Domain.Exceptions.InvalidFormat
{
    using System;

    public class InvalidFullNameFormatException : Exception
    {
        #region Constructors

        public InvalidFullNameFormatException()
        {
        }

        public InvalidFullNameFormatException(string message)
        : base(message)
        {
        }

        public InvalidFullNameFormatException(string message, Exception inner)
        : base(message, inner)
        {
        }

        #endregion
    }
}