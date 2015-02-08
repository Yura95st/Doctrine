namespace Doctrine.Domain.Services.Concrete
{
    using Doctrine.Domain.Dal;
    using Doctrine.Domain.Services.Abstract;
    using Doctrine.Domain.Services.Common;

    public class UserService : ServiceBase, IUserService
    {
        public UserService(IUnitOfWork unitOfWork)
        : base(unitOfWork)
        {
        }
    }
}