namespace Doctrine.Domain.Exceptions
{
    using System;

    public class InvalidEmailFormatException : Exception
    {
        #region Constructors

        public InvalidEmailFormatException()
        {
        }

        public InvalidEmailFormatException(string message)
            : base(message)
        {
        }

        public InvalidEmailFormatException(string message, Exception inner)
            : base(message, inner)
        {
        }

        #endregion
    }
}