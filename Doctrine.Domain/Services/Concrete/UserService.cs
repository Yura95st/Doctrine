namespace Doctrine.Domain.Services.Concrete
{
    using System;
    using System.Collections.Generic;

    using Doctrine.Domain.Models;
    using Doctrine.Domain.Services.Abstract;

    public class UserService : IUserService
    {
        #region IUserService Members

        public void Create(User entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(User entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> GetAll()
        {
            throw new NotImplementedException();
        }

        public User GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(User entity)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}