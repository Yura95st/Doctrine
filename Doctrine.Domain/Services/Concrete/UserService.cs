namespace Doctrine.Domain.Services.Concrete
{
    using System;

    using Doctrine.Domain.Dal;
    using Doctrine.Domain.Exceptions;
    using Doctrine.Domain.Models;
    using Doctrine.Domain.Services.Abstract;
    using Doctrine.Domain.Services.Common;
    using Doctrine.Domain.Utils;
    using Doctrine.Domain.Validation.Abstract;

    public class UserService : ServiceBase, IUserService
    {
        private readonly IUserValidation _userValidation;

        public UserService(IUnitOfWork unitOfWork, IUserValidation userValidation)
        : base(unitOfWork)
        {
            Guard.NotNull(userValidation, "userValidation");

            this._userValidation = userValidation;
        }

        #region IUserService Members

        public User Authenticate(string email, string password)
        {
            Guard.NotNullOrEmpty(email, "email");
            Guard.NotNullOrEmpty(password, "password");

            if (!this._userValidation.IsValidEmail(email))
            {
                throw new InvalidEmailFormatException(String.Format("Email '{0}' has invalid format.", email));
            }

            User user = this._unitOfWork.UserRepository.GetByEmail(email);

            if (user == null)
            {
                throw new NonexistentEmailException(String.Format("User with email '{0}' doesn't exist.", email));
            }

            if (user.Password != password)
            {
                throw new WrongPasswordException(String.Format("Password '{0}' is wrong.", password));
            }

            return user;
        }

        public User Create(string email, string fullName, string password)
        {
            throw new NotImplementedException();
        }

        public void Delete(int userId)
        {
            throw new NotImplementedException();
        }

        public User GetById(int userId)
        {
            Guard.IntMoreThanZero(userId, "userId");

            return this._unitOfWork.UserRepository.GetById(userId);
        }

        #endregion
    }
}