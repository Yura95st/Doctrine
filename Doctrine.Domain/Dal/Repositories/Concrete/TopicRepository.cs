namespace Doctrine.Domain.Dal.Repositories.Concrete
{
    using Doctrine.Domain.Dal.Repositories.Abstract;
    using Doctrine.Domain.Dal.Repositories.Common;
    using Doctrine.Domain.Models;

    public class TopicRepository : Repository<Topic>, ITopicRepository
    {
        public TopicRepository(DoctrineContext context)
        : base(context)
        {
        }

        public Topic GetByName(string topicName)
        {
            throw new System.NotImplementedException();
        }
    }
}