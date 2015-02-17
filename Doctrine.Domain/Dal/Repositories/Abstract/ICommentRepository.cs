namespace Doctrine.Domain.Dal.Repositories.Abstract
{
    using System;
    using System.Linq.Expressions;

    using Doctrine.Domain.Dal.Repositories.Common;
    using Doctrine.Domain.Models;

    public interface ICommentRepository : IRepository<Comment>
    {
        /// <summary>Gets the comment by identifier.</summary>
        /// <param name="commentId">The comment's identifier.</param>
        /// <param name="selector">The selector.</param>
        /// <returns>The comment.</returns>
        Comment GetById(int commentId, params Expression<Func<Comment, object>>[] selector);
    }
}