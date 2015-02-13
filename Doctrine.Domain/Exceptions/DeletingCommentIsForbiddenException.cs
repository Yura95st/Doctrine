namespace Doctrine.Domain.Exceptions
{
    using System;

    public class DeletingCommentIsForbiddenException : Exception
    {
        #region Constructors

        public DeletingCommentIsForbiddenException()
        {
        }

        public DeletingCommentIsForbiddenException(string message)
        : base(message)
        {
        }

        public DeletingCommentIsForbiddenException(string message, Exception inner)
        : base(message, inner)
        {
        }

        #endregion
    }
}