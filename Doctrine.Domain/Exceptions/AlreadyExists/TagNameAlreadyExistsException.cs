namespace Doctrine.Domain.Exceptions.AlreadyExists
{
    using System;

    public class TagNameAlreadyExistsException : Exception
    {
        #region Constructors

        public TagNameAlreadyExistsException()
        {
        }

        public TagNameAlreadyExistsException(string message)
        : base(message)
        {
        }

        public TagNameAlreadyExistsException(string message, Exception inner)
        : base(message, inner)
        {
        }

        #endregion
    }
}