namespace Doctrine.Domain.Dal.Repositories.Concrete
{
    using Doctrine.Domain.Dal.Repositories.Abstract;
    using Doctrine.Domain.Dal.Repositories.Common;
    using Doctrine.Domain.Models;

    public class ArticleRepository : Repository<Article>, IArticleRepository
    {
        public ArticleRepository(DoctrineContext context)
        : base(context)
        {
        }
    }
}