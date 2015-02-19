namespace Doctrine.Domain.Exceptions.NotFound
{
    using System;

    public class TagNotFoundException : Exception
    {
        #region Constructors

        public TagNotFoundException()
        {
        }

        public TagNotFoundException(string message)
        : base(message)
        {
        }

        public TagNotFoundException(string message, Exception inner)
        : base(message, inner)
        {
        }

        #endregion
    }
}