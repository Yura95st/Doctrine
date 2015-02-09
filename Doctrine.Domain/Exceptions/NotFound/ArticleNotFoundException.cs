namespace Doctrine.Domain.Exceptions.NotFound
{
    using System;

    public class ArticleNotFoundException : Exception
    {
        #region Constructors

        public ArticleNotFoundException()
        {
        }

        public ArticleNotFoundException(string message)
        : base(message)
        {
        }

        public ArticleNotFoundException(string message, Exception inner)
        : base(message, inner)
        {
        }

        #endregion
    }
}