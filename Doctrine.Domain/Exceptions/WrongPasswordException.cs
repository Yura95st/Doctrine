namespace Doctrine.Domain.Exceptions
{
    using System;

    public class WrongPasswordException : Exception
    {
        #region Constructors

        public WrongPasswordException()
        {
        }

        public WrongPasswordException(string message)
        : base(message)
        {
        }

        public WrongPasswordException(string message, Exception inner)
        : base(message, inner)
        {
        }

        #endregion
    }
}