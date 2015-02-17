namespace Doctrine.Domain.Dal.Repositories.Concrete
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using Doctrine.Domain.Dal.Repositories.Abstract;
    using Doctrine.Domain.Dal.Repositories.Common;
    using Doctrine.Domain.Models;

    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        public CommentRepository(DoctrineContext context)
        : base(context)
        {
        }

        public Comment GetById(int commentId, params Expression<Func<Comment, object>>[] selector)
        {
            return this.Get(v => v.CommentId == commentId, selector: selector).SingleOrDefault();
        }
    }
}