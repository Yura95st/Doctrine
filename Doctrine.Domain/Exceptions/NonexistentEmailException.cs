namespace Doctrine.Domain.Exceptions
{
    using System;

    public class NonexistentEmailException : Exception
    {
        #region Constructors

        public NonexistentEmailException()
        {
        }

        public NonexistentEmailException(string message)
            : base(message)
        {
        }

        public NonexistentEmailException(string message, Exception inner)
            : base(message, inner)
        {
        }

        #endregion
    }
}