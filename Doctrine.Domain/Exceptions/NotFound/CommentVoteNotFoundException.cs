namespace Doctrine.Domain.Exceptions.NotFound
{
    using System;

    public class CommentVoteNotFoundException : Exception
    {
        #region Constructors

        public CommentVoteNotFoundException()
        {
        }

        public CommentVoteNotFoundException(string message)
        : base(message)
        {
        }

        public CommentVoteNotFoundException(string message, Exception inner)
        : base(message, inner)
        {
        }

        #endregion
    }
}