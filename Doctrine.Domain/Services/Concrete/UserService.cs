namespace Doctrine.Domain.Services.Concrete
{
    using System;
    using System.Text.RegularExpressions;

    using Doctrine.Domain.Dal;
    using Doctrine.Domain.Exceptions;
    using Doctrine.Domain.Models;
    using Doctrine.Domain.Services.Abstract;
    using Doctrine.Domain.Services.Common;
    using Doctrine.Domain.Utils;

    public class UserService : ServiceBase, IUserService
    {
        public UserService(IUnitOfWork unitOfWork)
        : base(unitOfWork)
        {
        }

        #region IUserService Members

        public User Authenticate(string email, string password)
        {
            Guard.NotNullOrEmpty(email, "email");
            Guard.NotNullOrEmpty(password, "password");

            if (!UserService.IsValidEmail(email))
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
                throw new InvalidPasswordException(String.Format("Password '{0}' is invalid.", password));
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

        private static bool IsValidEmail(string email)
        {
            string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
                             + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

            return regex.IsMatch(email);
        }
    }
}