namespace Doctrine.Domain.Services.Concrete
{
    using Doctrine.Domain.Dal;
    using Doctrine.Domain.Services.Abstract;
    using Doctrine.Domain.Services.Common;

    public class ArticleService : ServiceBase, IArticleService
    {
        public ArticleService(IUnitOfWork unitOfWork)
        : base(unitOfWork)
        {
        }
    }
}