namespace Doctrine.Domain.Exceptions.AlreadyExists
{
    using System;

    public class TopicNameAlreadyExistsException : Exception
    {
        #region Constructors

        public TopicNameAlreadyExistsException()
        {
        }

        public TopicNameAlreadyExistsException(string message)
        : base(message)
        {
        }

        public TopicNameAlreadyExistsException(string message, Exception inner)
        : base(message, inner)
        {
        }

        #endregion
    }
}