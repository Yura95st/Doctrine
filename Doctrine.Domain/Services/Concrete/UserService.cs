﻿namespace Doctrine.Domain.Services.Concrete
{
    using System;
    using System.Linq;

    using Doctrine.Domain.Dal;
    using Doctrine.Domain.Exceptions;
    using Doctrine.Domain.Exceptions.InvalidFormat;
    using Doctrine.Domain.Exceptions.NotFound;
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

        public void AddArticleToFavorites(int userId, int articleId)
        {
            this.AddOrRemoveArticleFromFavorites(userId, articleId, false);
        }

        public User Authenticate(int visitorId, string email, string password)
        {
            Guard.IntMoreThanZero(visitorId, "visitorId");
            Guard.NotNullOrEmpty(email, "email");
            Guard.NotNullOrEmpty(password, "password");

            if (!this._userValidation.IsValidEmail(email))
            {
                throw new InvalidEmailFormatException(String.Format("Email '{0}' has invalid format.", email));
            }

            User user = this._unitOfWork.UserRepository.GetByEmail(email);

            if (user == null)
            {
                throw new UserNotFoundException(String.Format("User with email '{0}' was not found.", email));
            }

            if (user.Password != password)
            {
                throw new WrongPasswordException(String.Format("Password '{0}' is wrong.", password));
            }

            Visitor visitor = this._unitOfWork.VisitorRepository.GetById(visitorId);

            if (visitor == null)
            {
                throw new VisitorNotFoundException(String.Format("Visitor with id '{0}' was not found.", visitorId));
            }

            // Save user LogOn activity
            user.UserActivities.Add(new UserActivity
            { UserId = user.UserId, VisitorId = visitor.VisitorId, LogOnDate = DateTime.Now });

            this._unitOfWork.UserRepository.Update(user);
            this._unitOfWork.Save();

            return user;
        }

        public User Create(string email, string fullName, string password)
        {
            Guard.NotNullOrEmpty(email, "email");
            Guard.NotNullOrEmpty(fullName, "fullName");
            Guard.NotNullOrEmpty(password, "password");

            if (!this._userValidation.IsValidEmail(email))
            {
                throw new InvalidEmailFormatException(String.Format("Email '{0}' has invalid format.", email));
            }

            if (!this._userValidation.IsValidFullName(fullName))
            {
                throw new InvalidFullNameFormatException(String.Format("Full name '{0}' has invalid format.", fullName));
            }

            if (!this._userValidation.IsValidPassword(password))
            {
                throw new InvalidPasswordFormatException(String.Format("Password '{0}' has invalid format.", password));
            }

            User user = this._unitOfWork.UserRepository.GetByEmail(email);

            if (user != null)
            {
                throw new EmailAlreadyExistsException(String.Format("User with email '{0}' already exists.", email));
            }

            user = new User { Email = email, FullName = fullName, Password = password, RegistrationDate = DateTime.Now };

            this._unitOfWork.UserRepository.Insert(user);
            this._unitOfWork.Save();

            return user;
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

        public void ReadArticle(int userId, int articleId)
        {
            throw new NotImplementedException();
        }

        public void RemoveArticleFromFavorites(int userId, int articleId)
        {
            this.AddOrRemoveArticleFromFavorites(userId, articleId, true);
        }

        public void UnreadArticle(int userId, int articleId)
        {
            throw new NotImplementedException();
        }

        #endregion

        private void AddOrRemoveArticleFromFavorites(int userId, int articleId, bool removeFromFavorites)
        {
            Guard.IntMoreThanZero(userId, "userId");
            Guard.IntMoreThanZero(articleId, "articleId");

            User user = this._unitOfWork.UserRepository.Get(u => u.UserId == userId, selector: u => u.UserFavorites)
            .FirstOrDefault();

            if (user == null)
            {
                throw new UserNotFoundException(String.Format("User with ID '{0}' was not found.", userId));
            }

            UserFavorite userFavorite = user.UserFavorites.FirstOrDefault(f => f.ArticleId == articleId);

            if (userFavorite == null)
            {
                if (removeFromFavorites)
                {
                    throw new ArticleNotFoundException(String.Format("Article with ID '{0}' was not found.", articleId));
                }

                Article article = this._unitOfWork.ArticleRepository.GetById(articleId);

                if (article == null)
                {
                    throw new ArticleNotFoundException(String.Format("Article with ID '{0}' was not found.", articleId));
                }

                userFavorite = new UserFavorite { UserId = user.UserId, ArticleId = article.ArticleId };

                user.UserFavorites.Add(userFavorite);
            }

            if (removeFromFavorites)
            {
                user.UserFavorites.Remove(userFavorite);
            }
            else
            {
                userFavorite.AddDate = DateTime.Now;
            }

            this._unitOfWork.UserRepository.Update(user);
            this._unitOfWork.Save();
        }
    }
}