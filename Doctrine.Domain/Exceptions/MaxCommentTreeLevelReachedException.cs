namespace Doctrine.Domain.Exceptions
{
    using System;

    public class MaxCommentTreeLevelReachedException : Exception
    {
        #region Constructors

        public MaxCommentTreeLevelReachedException()
        {
        }

        public MaxCommentTreeLevelReachedException(string message)
        : base(message)
        {
        }

        public MaxCommentTreeLevelReachedException(string message, Exception inner)
        : base(message, inner)
        {
        }

        #endregion
    }
}