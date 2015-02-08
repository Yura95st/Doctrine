namespace Doctrine.Domain.Dal.Repositories.Concrete
{
    using Doctrine.Domain.Dal.Repositories.Abstract;
    using Doctrine.Domain.Dal.Repositories.Common;
    using Doctrine.Domain.Models;

    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(DoctrineContext context)
        : base(context)
        {
        }

        public User GetByEmail(string email)
        {
            throw new System.NotImplementedException();
        }
    }
}