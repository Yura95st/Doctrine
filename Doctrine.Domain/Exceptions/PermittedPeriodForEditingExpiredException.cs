namespace Doctrine.Domain.Exceptions
{
    using System;

    public class PermittedPeriodForEditingExpiredException : Exception
    {
        #region Constructors

        public PermittedPeriodForEditingExpiredException()
        {
        }

        public PermittedPeriodForEditingExpiredException(string message)
        : base(message)
        {
        }

        public PermittedPeriodForEditingExpiredException(string message, Exception inner)
        : base(message, inner)
        {
        }

        #endregion
    }
}