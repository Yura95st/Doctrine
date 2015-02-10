namespace Doctrine.Domain.Dal.Repositories.Concrete
{
    using Doctrine.Domain.Dal.Repositories.Abstract;
    using Doctrine.Domain.Dal.Repositories.Common;
    using Doctrine.Domain.Models;

    public class TagRepository : Repository<Tag>, ITagRepository
    {
        public TagRepository(DoctrineContext context)
        : base(context)
        {
        }

        public Tag GetByName(string tagName)
        {
            throw new System.NotImplementedException();
        }
    }
}