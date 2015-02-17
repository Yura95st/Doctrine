namespace Doctrine.Domain.Exceptions.NotFound
{
    using System;

    public class TopicNotFoundException : Exception
    {
        #region Constructors

        public TopicNotFoundException()
        {
        }

        public TopicNotFoundException(string message)
        : base(message)
        {
        }

        public TopicNotFoundException(string message, Exception inner)
        : base(message, inner)
        {
        }

        #endregion
    }
}