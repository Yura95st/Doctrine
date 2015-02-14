namespace Doctrine.Domain.Exceptions
{
    using System;

    public class PasswordIsNotStrongEnoughException : Exception
    {
        #region Constructors

        public PasswordIsNotStrongEnoughException()
        {
        }

        public PasswordIsNotStrongEnoughException(string message)
        : base(message)
        {
        }

        public PasswordIsNotStrongEnoughException(string message, Exception inner)
        : base(message, inner)
        {
        }

        #endregion
    }
}