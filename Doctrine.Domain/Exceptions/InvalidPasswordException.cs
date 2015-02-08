namespace Doctrine.Domain.Exceptions
{
    using System;

    public class InvalidPasswordException : Exception
    {
        #region Constructors

        public InvalidPasswordException()
        {
        }

        public InvalidPasswordException(string message)
            : base(message)
        {
        }

        public InvalidPasswordException(string message, Exception inner)
            : base(message, inner)
        {
        }

        #endregion
    }
}