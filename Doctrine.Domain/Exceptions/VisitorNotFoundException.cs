namespace Doctrine.Domain.Exceptions
{
    using System;

    public class VisitorNotFoundException : Exception
    {
        #region Constructors

        public VisitorNotFoundException()
        {
        }

        public VisitorNotFoundException(string message)
        : base(message)
        {
        }

        public VisitorNotFoundException(string message, Exception inner)
        : base(message, inner)
        {
        }

        #endregion
    }
}