namespace Doctrine.Domain.Exceptions
{
    using System;

    public class EmailAlreadyExistsException : Exception
    {
        #region Constructors

        public EmailAlreadyExistsException()
        {
        }

        public EmailAlreadyExistsException(string message)
        : base(message)
        {
        }

        public EmailAlreadyExistsException(string message, Exception inner)
        : base(message, inner)
        {
        }

        #endregion
    }
}