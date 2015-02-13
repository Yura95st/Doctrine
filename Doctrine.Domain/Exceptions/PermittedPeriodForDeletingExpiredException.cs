namespace Doctrine.Domain.Exceptions
{
    using System;

    public class PermittedPeriodForDeletingExpiredException : Exception
    {
        #region Constructors

        public PermittedPeriodForDeletingExpiredException()
        {
        }

        public PermittedPeriodForDeletingExpiredException(string message)
        : base(message)
        {
        }

        public PermittedPeriodForDeletingExpiredException(string message, Exception inner)
        : base(message, inner)
        {
        }

        #endregion
    }
}