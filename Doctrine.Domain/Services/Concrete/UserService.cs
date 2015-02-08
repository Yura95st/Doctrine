namespace Doctrine.Domain.Services.Concrete
{
    using Doctrine.Domain.Dal;
    using Doctrine.Domain.Models;
    using Doctrine.Domain.Services.Abstract;
    using Doctrine.Domain.Services.Common;

    public class UserService : ServiceBase, IUserService
    {
        public UserService(IUnitOfWork unitOfWork)
        : base(unitOfWork)
        {
        }

        public User Authenticate(string email, string password)
        {
            throw new System.NotImplementedException();
        }

        public User Create(string email, string fullName, string password)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(int userId)
        {
            throw new System.NotImplementedException();
        }

        public User GetById(int userId)
        {
            throw new System.NotImplementedException();
        }
    }
}