namespace Doctrine.Domain.Services.Concrete
{
    using Doctrine.Domain.Dal;
    using Doctrine.Domain.Services.Abstract;
    using Doctrine.Domain.Services.Common;

    public class CommentService : ServiceBase, ICommentService
    {
        public CommentService(IUnitOfWork unitOfWork)
        : base(unitOfWork)
        {
        }
    }
}