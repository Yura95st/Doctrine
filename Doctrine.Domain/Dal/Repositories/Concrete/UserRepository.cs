namespace Doctrine.Domain.Dal.Repositories.Concrete
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

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

        public User GetById(int userId, params Expression<Func<User, object>>[] selector)
        {
            return this.Get(v => v.UserId == userId, selector: selector).SingleOrDefault();
        }
    }
}