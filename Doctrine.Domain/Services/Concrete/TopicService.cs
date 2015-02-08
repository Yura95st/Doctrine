namespace Doctrine.Domain.Services.Concrete
{
    using Doctrine.Domain.Dal;
    using Doctrine.Domain.Services.Abstract;
    using Doctrine.Domain.Services.Common;

    public class TopicService : ServiceBase, ITopicService
    {
        public TopicService(IUnitOfWork unitOfWork)
        : base(unitOfWork)
        {
        }
    }
}