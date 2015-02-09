namespace Doctrine.Domain.Exceptions
{
    using System;

    public class UserNotFoundException : Exception
    {
        #region Constructors

        public UserNotFoundException()
        {
        }

        public UserNotFoundException(string message)
        : base(message)
        {
        }

        public UserNotFoundException(string message, Exception inner)
        : base(message, inner)
        {
        }

        #endregion
    }
}