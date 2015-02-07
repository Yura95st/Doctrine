namespace Doctrine.Domain.Dal.Repositories.Concrete
{
    using Doctrine.Domain.Dal.Repositories.Abstract;
    using Doctrine.Domain.Dal.Repositories.Common;
    using Doctrine.Domain.Models;

    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        public CommentRepository(DoctrineContext context)
        : base(context)
        {
        }
    }
}