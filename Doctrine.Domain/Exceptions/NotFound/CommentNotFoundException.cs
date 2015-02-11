namespace Doctrine.Domain.Exceptions.NotFound
{
    using System;

    public class CommentNotFoundException : Exception
    {
        #region Constructors

        public CommentNotFoundException()
        {
        }

        public CommentNotFoundException(string message)
        : base(message)
        {
        }

        public CommentNotFoundException(string message, Exception inner)
        : base(message, inner)
        {
        }

        #endregion
    }
}