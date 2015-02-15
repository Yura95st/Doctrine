namespace Doctrine.Domain.Exceptions
{
    using System;

    public class EditingCommentIsForbiddenException : Exception
    {
        #region Constructors

        public EditingCommentIsForbiddenException()
        {
        }

        public EditingCommentIsForbiddenException(string message)
        : base(message)
        {
        }

        public EditingCommentIsForbiddenException(string message, Exception inner)
        : base(message, inner)
        {
        }

        #endregion
    }
}