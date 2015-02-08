namespace Doctrine.Domain.Services.Concrete
{
    using Doctrine.Domain.Dal;
    using Doctrine.Domain.Services.Abstract;
    using Doctrine.Domain.Services.Common;

    public class TagService : ServiceBase, ITagService
    {
        public TagService(IUnitOfWork unitOfWork)
        : base(unitOfWork)
        {
        }
    }
}