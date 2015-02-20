namespace Doctrine.Domain.Exceptions
{
    using System;

    public class DeletingArticleIsForbiddenException : Exception
    {
        #region Constructors

        public DeletingArticleIsForbiddenException()
        {
        }

        public DeletingArticleIsForbiddenException(string message)
        : base(message)
        {
        }

        public DeletingArticleIsForbiddenException(string message, Exception inner)
        : base(message, inner)
        {
        }

        #endregion
    }
}