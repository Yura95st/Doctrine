namespace Doctrine.Domain.Tests.Services.Concrete
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Doctrine.Domain.Dal;
    using Doctrine.Domain.Dal.Repositories.Abstract;
    using Doctrine.Domain.Enums;
    using Doctrine.Domain.Exceptions;
    using Doctrine.Domain.Exceptions.InvalidFormat;
    using Doctrine.Domain.Exceptions.NotFound;
    using Doctrine.Domain.Models;
    using Doctrine.Domain.Services.Abstract;
    using Doctrine.Domain.Services.Concrete;
    using Doctrine.Domain.Tests.TestHelpers;
    using Doctrine.Domain.Utils.SecuredPasswordHelper;
    using Doctrine.Domain.Utils.SecuredPasswordHelper.Model;
    using Doctrine.Domain.Validation.Abstract;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class UserServiceTests
    {
        private Mock<ISecuredPasswordHelper> _securedPasswordHelperMock;

        private Mock<IUserValidation> _userValidationMock;

        [Test]
        public void AddArticleToFavorites_AllCredentialsAreValid_AddsArticleToFavorites()
        {
            // Arrange
            int userId = 1;
            int articleId = 2;
            UserFavorite[] userFavorites =
            {
                new UserFavorite { UserId = userId, ArticleId = articleId + 1 },
                new UserFavorite { UserId = userId, ArticleId = articleId + 1 }
            };

            User user = new User { UserId = userId, UserFavorites = userFavorites.ToList() };
            Article article = new Article { ArticleId = articleId };

            Expression<Func<User, object>>[] propertiesToInclude = { u => u.UserFavorites };

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(
            r =>
            r.GetById(userId,
            It.Is<Expression<Func<User, object>>[]>(
            selector => ExpressionHelper.AreExpressionArraysEqual(selector, propertiesToInclude))))
            .Returns(user);

            IEnumerable<UserFavorite> newUserFavorites = null;

            userRepositoryMock.Setup(r => r.Update(It.Is<User>(u => u.UserId == userId)))
            .Callback((User u) => newUserFavorites = u.UserFavorites);

            // Arrange - mock articleRepository
            Mock<IArticleRepository> articleRepositoryMock = new Mock<IArticleRepository>();
            articleRepositoryMock.Setup(r => r.GetById(articleId))
            .Returns(article);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            unitOfWorkMock.SetupGet(u => u.ArticleRepository)
            .Returns(articleRepositoryMock.Object);

            // Arrange - create target
            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object,
            this._securedPasswordHelperMock.Object);

            // Act
            target.AddArticleToFavorites(userId, articleId);

            // Assert
            Assert.IsNotNull(newUserFavorites);
            Assert.AreEqual(userFavorites.Count() + 1, newUserFavorites.Count());

            UserFavorite userFavorite = newUserFavorites.FirstOrDefault(v => v.ArticleId == articleId);

            Assert.IsNotNull(userFavorite);
            Assert.IsTrue(new DateTime() != userFavorite.AddDate);

            userRepositoryMock.Verify(
            r =>
            r.GetById(userId,
            It.Is<Expression<Func<User, object>>[]>(
            selector => ExpressionHelper.AreExpressionArraysEqual(selector, propertiesToInclude))), Times.Once);

            userRepositoryMock.Verify(r => r.Update(It.Is<User>(u => u.UserId == userId)), Times.Once);

            articleRepositoryMock.Verify(r => r.GetById(articleId), Times.Once);

            unitOfWorkMock.Verify(u => u.Save(), Times.Once);
        }

        [Test]
        public void AddArticleToFavorites_ArticleAlreadyAdded_UpdatesAddedDate()
        {
            // Arrange
            int userId = 1;
            int articleId = 2;
            DateTime addDate = new DateTime();
            UserFavorite[] userFavorites =
            {
                new UserFavorite { UserId = userId, ArticleId = articleId, AddDate = addDate },
                new UserFavorite { UserId = userId, ArticleId = articleId + 1 },
                new UserFavorite { UserId = userId, ArticleId = articleId + 1 }
            };

            User user = new User { UserId = userId, UserFavorites = userFavorites.ToList() };

            Expression<Func<User, object>>[] propertiesToInclude = { u => u.UserFavorites };

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(
            r =>
            r.GetById(userId,
            It.Is<Expression<Func<User, object>>[]>(
            selector => ExpressionHelper.AreExpressionArraysEqual(selector, propertiesToInclude))))
            .Returns(user);

            IEnumerable<UserFavorite> newUserFavorites = null;

            userRepositoryMock.Setup(r => r.Update(It.Is<User>(u => u.UserId == userId)))
            .Callback((User u) => newUserFavorites = u.UserFavorites);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            // Arrange - create target
            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object,
            this._securedPasswordHelperMock.Object);

            // Act
            target.AddArticleToFavorites(userId, articleId);

            // Assert
            Assert.IsNotNull(newUserFavorites);
            Assert.AreEqual(userFavorites.Count(), newUserFavorites.Count());

            UserFavorite userFavorite = newUserFavorites.FirstOrDefault(v => v.UserId == userId && v.ArticleId == articleId);

            Assert.IsNotNull(userFavorite);
            Assert.IsTrue(userFavorite.AddDate.Subtract(addDate)
            .TotalMilliseconds > 0);

            userRepositoryMock.Verify(
            r =>
            r.GetById(userId,
            It.Is<Expression<Func<User, object>>[]>(
            selector => ExpressionHelper.AreExpressionArraysEqual(selector, propertiesToInclude))), Times.Once);

            userRepositoryMock.Verify(r => r.Update(It.Is<User>(u => u.UserId == userId)), Times.Once);

            unitOfWorkMock.Verify(u => u.Save(), Times.Once);
        }

        [Test]
        public void AddArticleToFavorites_ArticleIdIsLessOrEqualToZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            int userId = 1;

            IUserService target = new UserService(new Mock<IUnitOfWork>().Object, this._userValidationMock.Object,
            this._securedPasswordHelperMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => target.AddArticleToFavorites(userId, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.AddArticleToFavorites(userId, 0));
        }

        [Test]
        public void AddArticleToFavorites_NonexistentArticleId_ThrowsArticleNotFoundException()
        {
            // Arrange
            User user = new User { UserId = 1 };

            int articleId = 2;

            Expression<Func<User, object>>[] propertiesToInclude = { u => u.UserFavorites };

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(
            r =>
            r.GetById(user.UserId,
            It.Is<Expression<Func<User, object>>[]>(
            selector => ExpressionHelper.AreExpressionArraysEqual(selector, propertiesToInclude))))
            .Returns(user);

            // Arrange - mock articleRepository
            Mock<IArticleRepository> articleRepositoryMock = new Mock<IArticleRepository>();

            articleRepositoryMock.Setup(r => r.GetById(articleId))
            .Returns((Article)null);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            unitOfWorkMock.SetupGet(u => u.ArticleRepository)
            .Returns(articleRepositoryMock.Object);

            // Arrange - create target
            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object,
            this._securedPasswordHelperMock.Object);

            // Act and Assert
            Assert.Throws<ArticleNotFoundException>(() => target.AddArticleToFavorites(user.UserId, articleId));

            userRepositoryMock.Verify(
            r =>
            r.GetById(user.UserId,
            It.Is<Expression<Func<User, object>>[]>(
            selector => ExpressionHelper.AreExpressionArraysEqual(selector, propertiesToInclude))), Times.Once);

            userRepositoryMock.Verify(r => r.Update(It.Is<User>(u => u.UserId == user.UserId)), Times.Never);

            articleRepositoryMock.Verify(r => r.GetById(articleId), Times.Once);

            unitOfWorkMock.Verify(u => u.Save(), Times.Never);
        }

        [Test]
        public void AddArticleToFavorites_NonexistentUserId_ThrowsUserNotFoundException()
        {
            // Arrange
            int userId = 1;
            int articleId = 2;

            Expression<Func<User, object>>[] propertiesToInclude = { u => u.UserFavorites };

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(
            r =>
            r.GetById(userId,
            It.Is<Expression<Func<User, object>>[]>(
            selector => ExpressionHelper.AreExpressionArraysEqual(selector, propertiesToInclude))))
            .Returns((User)null);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            // Arrange - create target
            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object,
            this._securedPasswordHelperMock.Object);

            // Act and Assert
            Assert.Throws<UserNotFoundException>(() => target.AddArticleToFavorites(userId, articleId));

            userRepositoryMock.Verify(
            r =>
            r.GetById(userId,
            It.Is<Expression<Func<User, object>>[]>(
            selector => ExpressionHelper.AreExpressionArraysEqual(selector, propertiesToInclude))), Times.Once);

            userRepositoryMock.Verify(r => r.Update(It.Is<User>(u => u.UserId == userId)), Times.Never);

            unitOfWorkMock.Verify(u => u.Save(), Times.Never);
        }

        [Test]
        public void AddArticleToFavorites_UserIdIsLessOrEqualToZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            int articleId = 1;

            IUserService target = new UserService(new Mock<IUnitOfWork>().Object, this._userValidationMock.Object,
            this._securedPasswordHelperMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => target.AddArticleToFavorites(-1, articleId));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.AddArticleToFavorites(0, articleId));
        }

        [Test]
        public void Authenticate_CredentialsAreValid_AuthenticatesVisitorAndReturnsUser()
        {
            // Arrange
            int visitorId = 1;
            int userId = 2;
            UserActivity[] userActivities =
            {
                new UserActivity { ActivityId = 1, UserId = userId, VisitorId = visitorId + 1 },
                new UserActivity { ActivityId = 2, UserId = userId, VisitorId = visitorId + 2 }
            };

            Visitor visitor = new Visitor { VisitorId = visitorId, IpAddress = "127.0.0.1" };
            User user = new User
            {
                UserId = userId, Email = "email", Password = "password", Salt = "password_salt",
                UserActivities = userActivities.ToList()
            };

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(r => r.GetByEmail(user.Email))
            .Returns(user);

            // Arrange - mock visitorRepository
            Mock<IVisitorRepository> visitorRepositoryMock = new Mock<IVisitorRepository>();

            visitorRepositoryMock.Setup(r => r.GetById(visitorId))
            .Returns(visitor);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            unitOfWorkMock.SetupGet(u => u.VisitorRepository)
            .Returns(visitorRepositoryMock.Object);

            // Arrange - create target
            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object,
            this._securedPasswordHelperMock.Object);

            // Act
            User authenticatedUser = target.Authenticate(visitorId, user.Email, user.Password);

            // Assert
            Assert.AreSame(user, authenticatedUser);
            Assert.AreEqual(userActivities.Count() + 1, authenticatedUser.UserActivities.Count());

            UserActivity userActivity = authenticatedUser.UserActivities.FirstOrDefault(u => u.VisitorId == visitorId);

            Assert.IsNotNull(userActivity);
            Assert.IsTrue(new DateTime() != userActivity.LogOnDate);

            userRepositoryMock.Verify(r => r.GetByEmail(user.Email), Times.Once);
            userRepositoryMock.Verify(r => r.Update(It.Is<User>(u => u.UserId == userId)), Times.Once);

            visitorRepositoryMock.Verify(r => r.GetById(visitor.VisitorId), Times.Once);

            unitOfWorkMock.Verify(u => u.Save(), Times.Once);
        }

        [Test]
        public void Authenticate_EmailOrPasswordIsEmpty_ThrowsArgumentException()
        {
            // Arrange
            int visitorId = 1;

            IUserService target = new UserService(new Mock<IUnitOfWork>().Object, this._userValidationMock.Object,
            this._securedPasswordHelperMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentException>(() => target.Authenticate(visitorId, "", "password"));
            Assert.Throws<ArgumentException>(() => target.Authenticate(visitorId, "email", ""));
        }

        [Test]
        public void Authenticate_EmailOrPasswordIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            int visitorId = 1;

            IUserService target = new UserService(new Mock<IUnitOfWork>().Object, this._userValidationMock.Object,
            this._securedPasswordHelperMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.Authenticate(visitorId, "email", null));
            Assert.Throws<ArgumentNullException>(() => target.Authenticate(visitorId, null, "password"));
        }

        [Test]
        public void Authenticate_InvalidEmailFormat_ThrowsInvalidEmailFormatException()
        {
            // Arrange
            int visitorId = 1;
            string email = "invalid_email";
            string password = "password";

            this._userValidationMock.Setup(v => v.IsValidEmail(email))
            .Returns(false);

            // Arrange - create target
            IUserService target = new UserService(new Mock<IUnitOfWork>().Object, this._userValidationMock.Object,
            this._securedPasswordHelperMock.Object);

            // Act and Assert
            Assert.Throws<InvalidEmailFormatException>(() => target.Authenticate(visitorId, email, password));

            this._userValidationMock.Verify(v => v.IsValidEmail(email), Times.Once);
        }

        [Test]
        public void Authenticate_NonexistentEmail_ThrowsUserNotFoundException()
        {
            // Arrange
            int visitorId = 1;
            string email = "nonexistent_email";
            string password = "password";

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(r => r.GetByEmail(email))
            .Returns((User)null);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            // Arrange - create target
            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object,
            this._securedPasswordHelperMock.Object);

            // Act and Assert
            Assert.Throws<UserNotFoundException>(() => target.Authenticate(visitorId, email, password));

            userRepositoryMock.Verify(r => r.GetByEmail(email), Times.Once);
            userRepositoryMock.Verify(r => r.Update(It.Is<User>(u => u.Email == email)), Times.Never);

            unitOfWorkMock.Verify(u => u.Save(), Times.Never);
        }

        [Test]
        public void Authenticate_NonexistentVisitorId_ThrowsVisitorNotFoundException()
        {
            // Arrange
            int visitorId = 1;

            User user = new User { UserId = 1, Email = "email", Password = "password", Salt = "password_salt" };

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(r => r.GetByEmail(user.Email))
            .Returns(user);

            // Arrange - mock visitorRepository
            Mock<IVisitorRepository> visitorRepositoryMock = new Mock<IVisitorRepository>();

            visitorRepositoryMock.Setup(r => r.GetById(visitorId))
            .Returns((Visitor)null);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            unitOfWorkMock.SetupGet(u => u.VisitorRepository)
            .Returns(visitorRepositoryMock.Object);

            // Arrange - create target
            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object,
            this._securedPasswordHelperMock.Object);

            // Act and Assert
            Assert.Throws<VisitorNotFoundException>(() => target.Authenticate(visitorId, user.Email, user.Password));

            userRepositoryMock.Verify(r => r.GetByEmail(user.Email), Times.Once);
            userRepositoryMock.Verify(r => r.Update(It.Is<User>(u => u.UserId == user.UserId)), Times.Never);

            visitorRepositoryMock.Verify(r => r.GetById(visitorId), Times.Once);

            unitOfWorkMock.Verify(u => u.Save(), Times.Never);
        }

        [Test]
        public void Authenticate_VisitorIdIsLessOrEqualToZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            string email = "email";
            string password = "password";

            IUserService target = new UserService(new Mock<IUnitOfWork>().Object, this._userValidationMock.Object,
            this._securedPasswordHelperMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Authenticate(-1, email, password));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Authenticate(0, email, password));
        }

        [Test]
        public void Authenticate_WrongPassword_ThrowsWrongPasswordException()
        {
            // Arrange
            int visitorId = 1;

            User user = new User { UserId = 1, Email = "email", Password = "wrong_password", Salt = "password_salt" };

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(r => r.GetByEmail(user.Email))
            .Returns(user);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            // Arrange - mock securedPasswordHelper
            this._securedPasswordHelperMock.Setup(
            h =>
            h.ArePasswordsEqual(user.Password, It.Is<SecuredPassword>(p => p.Hash == user.Password && p.Salt == user.Salt)))
            .Returns(false);

            // Arrange - create target
            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object,
            this._securedPasswordHelperMock.Object);

            // Act
            Assert.Throws<WrongPasswordException>(() => target.Authenticate(visitorId, user.Email, user.Password));

            // Assert
            userRepositoryMock.Verify(r => r.GetByEmail(user.Email), Times.Once);
            userRepositoryMock.Verify(r => r.Update(It.Is<User>(u => u.UserId == user.UserId)), Times.Never);

            unitOfWorkMock.Verify(u => u.Save(), Times.Never);

            this._securedPasswordHelperMock.Verify(
            h =>
            h.ArePasswordsEqual(user.Password, It.Is<SecuredPassword>(p => p.Hash == user.Password && p.Salt == user.Salt)),
            Times.Once);
        }

        [Test]
        public void Create_AllCredentialsAreValid_CreatesAndReturnsNewUser()
        {
            // Arrange
            string email = "email";
            string firstName = "firstName";
            string lastName = "lastName";
            string password = "password";

            SecuredPassword securedPassword = new SecuredPassword("secured_password", "password_salt");

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(r => r.GetByEmail(email))
            .Returns((User)null);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            // Arrange - mock securedPasswordHelper
            this._securedPasswordHelperMock.Setup(h => h.GetSecuredPassword(password))
            .Returns(securedPassword);

            // Arrange - create target
            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object,
            this._securedPasswordHelperMock.Object);

            // Act
            User user = target.Create(email, firstName, lastName, password);

            // Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(email, user.Email);
            Assert.AreEqual(firstName, user.FirstName);
            Assert.AreEqual(lastName, user.LastName);
            Assert.AreEqual(securedPassword.Hash, user.Password);
            Assert.AreEqual(securedPassword.Salt, user.Salt);
            Assert.IsTrue(new DateTime() != user.RegistrationDate);

            userRepositoryMock.Verify(r => r.GetByEmail(email), Times.Once);
            userRepositoryMock.Verify(r => r.Insert(It.Is<User>(u => u.Email == email)), Times.Once);

            unitOfWorkMock.Verify(u => u.Save(), Times.Once);

            this._securedPasswordHelperMock.Verify(h => h.GetSecuredPassword(password), Times.Once);
        }

        [Test]
        public void Create_EmailAlreadyExists_ThrowsEmailAlreadyExistsException()
        {
            // Arrange
            string email = "user@email.com";
            string firstName = "user_firstName";
            string lastName = "user_lastName";
            string password = "user_password";

            User user = new User();

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(r => r.GetByEmail(email))
            .Returns(user);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            // Arrange - create target
            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object,
            this._securedPasswordHelperMock.Object);

            // Act and Assert
            Assert.Throws<EmailAlreadyExistsException>(() => target.Create(email, firstName, lastName, password));

            userRepositoryMock.Verify(r => r.GetByEmail(email), Times.Once);
            userRepositoryMock.Verify(r => r.Insert(It.Is<User>(u => u.Email == email)), Times.Never);

            unitOfWorkMock.Verify(u => u.Save(), Times.Never);
        }

        [Test]
        public void Create_EmailOrFirstNameOrLastNameOrPasswordIsEmpty_ThrowsArgumentException()
        {
            // Arrange
            string email = "email";
            string firstName = "firstName";
            string lastName = "lastName";
            string password = "password";

            IUserService target = new UserService(new Mock<IUnitOfWork>().Object, this._userValidationMock.Object,
            this._securedPasswordHelperMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentException>(() => target.Create("", firstName, lastName, password));
            Assert.Throws<ArgumentException>(() => target.Create(email, "", lastName, password));
            Assert.Throws<ArgumentException>(() => target.Create(email, firstName, "", password));
            Assert.Throws<ArgumentException>(() => target.Create(email, firstName, lastName, ""));
        }

        [Test]
        public void Create_EmailOrFirstNameOrLastNameOrPasswordIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            string email = "email";
            string firstName = "firstName";
            string lastName = "lastName";
            string password = "password";

            IUserService target = new UserService(new Mock<IUnitOfWork>().Object, this._userValidationMock.Object,
            this._securedPasswordHelperMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.Create(null, firstName, lastName, password));
            Assert.Throws<ArgumentNullException>(() => target.Create(email, null, lastName, password));
            Assert.Throws<ArgumentNullException>(() => target.Create(email, firstName, null, password));
            Assert.Throws<ArgumentNullException>(() => target.Create(email, firstName, lastName, null));
        }

        [Test]
        public void Create_InvalidEmailFormat_ThrowsInvalidEmailFormatException()
        {
            // Arrange
            string email = "invalid_email";
            string firstName = "firstName";
            string lastName = "lastName";
            string password = "password";

            // Arrange - mock userValidation
            this._userValidationMock.Setup(v => v.IsValidEmail(email))
            .Returns(false);

            // Arrange - create target
            IUserService target = new UserService(new Mock<IUnitOfWork>().Object, this._userValidationMock.Object,
            this._securedPasswordHelperMock.Object);

            // Act and Assert
            Assert.Throws<InvalidEmailFormatException>(() => target.Create(email, firstName, lastName, password));

            this._userValidationMock.Verify(v => v.IsValidEmail(email), Times.Once);
        }

        [Test]
        public void Create_InvalidFirstNameFormat_ThrowsInvalidFirstNameFormatException()
        {
            // Arrange
            string email = "email";
            string firstName = "invalid_firstName";
            string lastName = "lastName";
            string password = "password";

            // Arrange - mock userValidation
            this._userValidationMock.Setup(v => v.IsValidName(firstName))
            .Returns(false);

            // Arrange - create target
            IUserService target = new UserService(new Mock<IUnitOfWork>().Object, this._userValidationMock.Object,
            this._securedPasswordHelperMock.Object);

            // Act and Assert
            Assert.Throws<InvalidFirstNameFormatException>(() => target.Create(email, firstName, lastName, password));

            this._userValidationMock.Verify(v => v.IsValidName(firstName), Times.Once);
        }

        [Test]
        public void Create_InvalidLastNameFormat_ThrowsInvalidLastNameFormatException()
        {
            // Arrange
            string email = "email";
            string firstName = "firstName";
            string lastName = "invalid_lastName";
            string password = "password";

            // Arrange - mock userValidation
            this._userValidationMock.Setup(v => v.IsValidName(lastName))
            .Returns(false);

            // Arrange - create target
            IUserService target = new UserService(new Mock<IUnitOfWork>().Object, this._userValidationMock.Object,
            this._securedPasswordHelperMock.Object);

            // Act and Assert
            Assert.Throws<InvalidLastNameFormatException>(() => target.Create(email, firstName, lastName, password));

            this._userValidationMock.Verify(v => v.IsValidName(lastName), Times.Once);
        }

        [Test]
        public void Create_PasswordStrengthIsVeryWeakOrWeakOrMedium_ThrowsPasswordIsNotStrongEnoughException()
        {
            // Arrange
            string email = "email";
            string firstName = "firstName";
            string lastName = "lastName";
            string password = "invalid_password";

            PasswordStrength[] passwordStrengths =
            {
                PasswordStrength.VeryWeak, PasswordStrength.Weak,
                PasswordStrength.Medium
            };

            foreach (PasswordStrength passwordStrength in passwordStrengths)
            {
                // Arrange - mock userValidation
                this._userValidationMock.ResetCalls();

                this._userValidationMock.Setup(v => v.GetPasswordStrength(password))
                .Returns(passwordStrength);

                // Arrange - create target
                IUserService target = new UserService(new Mock<IUnitOfWork>().Object, this._userValidationMock.Object,
                this._securedPasswordHelperMock.Object);

                // Act and Assert
                Assert.Throws<PasswordIsNotStrongEnoughException>(() => target.Create(email, firstName, lastName, password));

                this._userValidationMock.Verify(v => v.GetPasswordStrength(password), Times.Once);
            }
        }

        [Test]
        public void GetById_NonexistentUserId_ReturnsNull()
        {
            // Arrange
            int userId = 1;

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(r => r.GetById(userId))
            .Returns((User)null);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            // Arrange - create target
            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object,
            this._securedPasswordHelperMock.Object);

            // Act
            User user = target.GetById(userId);

            // Assert
            Assert.IsNull(user);

            userRepositoryMock.Verify(r => r.GetById(userId), Times.Once);
        }

        [Test]
        public void GetById_UserIdIsLessOrEqualToZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            IUserService target = new UserService(new Mock<IUnitOfWork>().Object, this._userValidationMock.Object,
            this._securedPasswordHelperMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => target.GetById(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.GetById(0));
        }

        [Test]
        public void GetById_UserIdIsValid_ReturnsUser()
        {
            // Arrange
            User testUser = new User { UserId = 1 };

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(r => r.GetById(testUser.UserId))
            .Returns(testUser);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            // Arrange - create target
            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object,
            this._securedPasswordHelperMock.Object);

            // Act
            User user = target.GetById(testUser.UserId);

            // Assert
            Assert.AreSame(testUser, user);

            userRepositoryMock.Verify(r => r.GetById(testUser.UserId), Times.Once);
        }

        [SetUp]
        public void Init()
        {
            this.MockUserValidation();

            this.MockSecuredPasswordHelper();
        }

        [Test]
        public void ReadArticle_AllCredentialsAreValid_ReadsArticle()
        {
            // Arrange
            int userId = 1;
            int articleId = 2;
            UserReadHistory[] userReadHistories =
            {
                new UserReadHistory { UserId = userId, ArticleId = articleId + 1 },
                new UserReadHistory { UserId = userId, ArticleId = articleId + 2 }
            };

            User user = new User { UserId = userId, UserReadHistories = userReadHistories.ToList() };
            Article article = new Article { ArticleId = articleId };

            Expression<Func<User, object>>[] propertiesToInclude = { u => u.UserReadHistories };

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(
            r =>
            r.GetById(userId,
            It.Is<Expression<Func<User, object>>[]>(
            selector => ExpressionHelper.AreExpressionArraysEqual(selector, propertiesToInclude))))
            .Returns(user);

            IEnumerable<UserReadHistory> newUserReadHistories = null;

            userRepositoryMock.Setup(r => r.Update(It.Is<User>(u => u.UserId == userId)))
            .Callback((User u) => newUserReadHistories = u.UserReadHistories);

            // Arrange - mock articleRepository
            Mock<IArticleRepository> articleRepositoryMock = new Mock<IArticleRepository>();

            articleRepositoryMock.Setup(r => r.GetById(articleId))
            .Returns(article);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            unitOfWorkMock.SetupGet(u => u.ArticleRepository)
            .Returns(articleRepositoryMock.Object);

            // Arrange - create target
            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object,
            this._securedPasswordHelperMock.Object);

            // Act
            target.ReadArticle(userId, articleId);

            // Assert
            Assert.IsNotNull(newUserReadHistories);
            Assert.AreEqual(userReadHistories.Count() + 1, newUserReadHistories.Count());

            UserReadHistory userReadHistory = newUserReadHistories.FirstOrDefault(v => v.ArticleId == articleId);

            Assert.IsNotNull(userReadHistory);
            Assert.IsTrue(new DateTime() != userReadHistory.ReadDate);

            userRepositoryMock.Verify(
            r =>
            r.GetById(userId,
            It.Is<Expression<Func<User, object>>[]>(
            selector => ExpressionHelper.AreExpressionArraysEqual(selector, propertiesToInclude))), Times.Once);

            userRepositoryMock.Verify(r => r.Update(It.Is<User>(u => u.UserId == userId)), Times.Once);

            articleRepositoryMock.Verify(r => r.GetById(articleId), Times.Once);

            unitOfWorkMock.Verify(u => u.Save(), Times.Once);
        }

        [Test]
        public void ReadArticle_ArticleAlreadyRead_UpdatesReadDate()
        {
            // Arrange
            int userId = 1;
            int articleId = 2;
            DateTime readDate = DateTime.Now;
            UserReadHistory[] userReadHistories =
            {
                new UserReadHistory { UserId = userId, ArticleId = articleId, ReadDate = readDate },
                new UserReadHistory { UserId = userId, ArticleId = articleId + 1 },
                new UserReadHistory { UserId = userId, ArticleId = articleId + 2 }
            };

            User user = new User { UserId = userId, UserReadHistories = userReadHistories.ToList() };

            Expression<Func<User, object>>[] propertiesToInclude = { u => u.UserReadHistories };

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(
            r =>
            r.GetById(userId,
            It.Is<Expression<Func<User, object>>[]>(
            selector => ExpressionHelper.AreExpressionArraysEqual(selector, propertiesToInclude))))
            .Returns(user);

            IEnumerable<UserReadHistory> newUserReadHistories = null;

            userRepositoryMock.Setup(r => r.Update(It.Is<User>(u => u.UserId == userId)))
            .Callback((User u) => newUserReadHistories = u.UserReadHistories);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            // Arrange - create target
            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object,
            this._securedPasswordHelperMock.Object);

            // Act
            target.ReadArticle(userId, articleId);

            // Assert
            Assert.IsNotNull(newUserReadHistories);
            Assert.AreEqual(userReadHistories.Count(), newUserReadHistories.Count());

            UserReadHistory userReadHistory = newUserReadHistories.FirstOrDefault(v => v.ArticleId == articleId);

            Assert.IsNotNull(userReadHistory);
            Assert.IsTrue(userReadHistory.ReadDate.Subtract(readDate)
            .TotalMilliseconds > 0);

            userRepositoryMock.Verify(
            r =>
            r.GetById(userId,
            It.Is<Expression<Func<User, object>>[]>(
            selector => ExpressionHelper.AreExpressionArraysEqual(selector, propertiesToInclude))), Times.Once);

            userRepositoryMock.Verify(r => r.Update(It.Is<User>(u => u.UserId == userId)), Times.Once);

            unitOfWorkMock.Verify(u => u.Save(), Times.Once);
        }

        [Test]
        public void ReadArticle_ArticleIdIsLessOrEqualToZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            int userId = 1;

            IUserService target = new UserService(new Mock<IUnitOfWork>().Object, this._userValidationMock.Object,
            this._securedPasswordHelperMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => target.ReadArticle(userId, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.ReadArticle(userId, 0));
        }

        [Test]
        public void ReadArticle_NonexistentArticleId_ThrowsArticleNotFoundException()
        {
            // Arrange
            User user = new User { UserId = 1 };

            int articleId = 2;

            Expression<Func<User, object>>[] propertiesToInclude = { u => u.UserReadHistories };

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(
            r =>
            r.GetById(user.UserId,
            It.Is<Expression<Func<User, object>>[]>(
            selector => ExpressionHelper.AreExpressionArraysEqual(selector, propertiesToInclude))))
            .Returns(user);

            // Arrange - mock articleRepository
            Mock<IArticleRepository> articleRepositoryMock = new Mock<IArticleRepository>();

            articleRepositoryMock.Setup(r => r.GetById(articleId))
            .Returns((Article)null);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            unitOfWorkMock.SetupGet(u => u.ArticleRepository)
            .Returns(articleRepositoryMock.Object);

            // Arrange - create target
            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object,
            this._securedPasswordHelperMock.Object);

            // Act and Assert
            Assert.Throws<ArticleNotFoundException>(() => target.ReadArticle(user.UserId, articleId));

            userRepositoryMock.Verify(
            r =>
            r.GetById(user.UserId,
            It.Is<Expression<Func<User, object>>[]>(
            selector => ExpressionHelper.AreExpressionArraysEqual(selector, propertiesToInclude))), Times.Once);

            userRepositoryMock.Verify(r => r.Update(It.Is<User>(u => u.UserId == user.UserId)), Times.Never);

            articleRepositoryMock.Verify(r => r.GetById(articleId), Times.Once);

            unitOfWorkMock.Verify(u => u.Save(), Times.Never);
        }

        [Test]
        public void ReadArticle_NonexistentUserId_ThrowsUserNotFoundException()
        {
            // Arrange
            int userId = 1;
            int articleId = 2;

            Expression<Func<User, object>>[] propertiesToInclude = { u => u.UserReadHistories };

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(
            r =>
            r.GetById(userId,
            It.Is<Expression<Func<User, object>>[]>(
            selector => ExpressionHelper.AreExpressionArraysEqual(selector, propertiesToInclude))))
            .Returns((User)null);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            // Arrange - create target
            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object,
            this._securedPasswordHelperMock.Object);

            // Act and Assert
            Assert.Throws<UserNotFoundException>(() => target.ReadArticle(userId, articleId));

            userRepositoryMock.Verify(
            r =>
            r.GetById(userId,
            It.Is<Expression<Func<User, object>>[]>(
            selector => ExpressionHelper.AreExpressionArraysEqual(selector, propertiesToInclude))), Times.Once);

            userRepositoryMock.Verify(r => r.Update(It.Is<User>(u => u.UserId == userId)), Times.Never);

            unitOfWorkMock.Verify(u => u.Save(), Times.Never);
        }

        [Test]
        public void ReadArticle_UserIdIsLessOrEqualToZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            int articleId = 1;

            IUserService target = new UserService(new Mock<IUnitOfWork>().Object, this._userValidationMock.Object,
            this._securedPasswordHelperMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => target.ReadArticle(-1, articleId));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.ReadArticle(0, articleId));
        }

        [Test]
        public void RemoveArticleFromFavorites_AllCredentialsAreValid_RemovesArticleFromFavorites()
        {
            // Arrange
            int userId = 1;
            int articleId = 2;
            UserFavorite[] userFavorites =
            {
                new UserFavorite { UserId = userId, ArticleId = articleId, AddDate = DateTime.Now },
                new UserFavorite { UserId = userId, ArticleId = articleId + 1, AddDate = DateTime.Now },
                new UserFavorite { UserId = userId, ArticleId = articleId + 2, AddDate = DateTime.Now }
            };

            User user = new User { UserId = userId, UserFavorites = userFavorites.ToList() };

            Expression<Func<User, object>>[] propertiesToInclude = { u => u.UserFavorites };

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(
            r =>
            r.GetById(userId,
            It.Is<Expression<Func<User, object>>[]>(
            selector => ExpressionHelper.AreExpressionArraysEqual(selector, propertiesToInclude))))
            .Returns(user);

            IEnumerable<UserFavorite> newUserFavorites = null;

            userRepositoryMock.Setup(r => r.Update(It.Is<User>(u => u.UserId == userId)))
            .Callback((User u) => newUserFavorites = u.UserFavorites);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            // Arrange - create target
            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object,
            this._securedPasswordHelperMock.Object);

            // Act
            target.RemoveArticleFromFavorites(userId, articleId);

            // Assert
            Assert.NotNull(newUserFavorites);
            Assert.AreEqual(userFavorites.Count() - 1, newUserFavorites.Count());
            Assert.AreEqual(0, newUserFavorites.Count(f => f.ArticleId == articleId));

            userRepositoryMock.Verify(
            r =>
            r.GetById(userId,
            It.Is<Expression<Func<User, object>>[]>(
            selector => ExpressionHelper.AreExpressionArraysEqual(selector, propertiesToInclude))), Times.Once);

            userRepositoryMock.Verify(r => r.Update(It.Is<User>(u => u.UserId == userId)), Times.Once);

            unitOfWorkMock.Verify(u => u.Save(), Times.Once);
        }

        [Test]
        public void RemoveArticleFromFavorites_ArticleHasNotBeenAddedToFavorites_ThrowsArticleNotFoundException()
        {
            // Arrange
            int userId = 1;
            int articleId = 2;

            User user = new User { UserId = userId, UserFavorites = new List<UserFavorite>() };

            Expression<Func<User, object>>[] propertiesToInclude = { u => u.UserFavorites };

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(
            r =>
            r.GetById(userId,
            It.Is<Expression<Func<User, object>>[]>(
            selector => ExpressionHelper.AreExpressionArraysEqual(selector, propertiesToInclude))))
            .Returns(user);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            // Arrange - create target
            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object,
            this._securedPasswordHelperMock.Object);

            // Act and Assert
            Assert.Throws<ArticleNotFoundException>(() => target.RemoveArticleFromFavorites(userId, articleId));

            userRepositoryMock.Verify(
            r =>
            r.GetById(userId,
            It.Is<Expression<Func<User, object>>[]>(
            selector => ExpressionHelper.AreExpressionArraysEqual(selector, propertiesToInclude))), Times.Once);

            userRepositoryMock.Verify(r => r.Update(It.Is<User>(u => u.UserId == userId)), Times.Never);

            unitOfWorkMock.Verify(u => u.Save(), Times.Never);
        }

        [Test]
        public void RemoveArticleFromFavorites_ArticleIdIsLessOrEqualToZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            int userId = 1;

            IUserService target = new UserService(new Mock<IUnitOfWork>().Object, this._userValidationMock.Object,
            this._securedPasswordHelperMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => target.RemoveArticleFromFavorites(userId, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.RemoveArticleFromFavorites(userId, 0));
        }

        [Test]
        public void RemoveArticleFromFavorites_NonexistentUserId_ThrowsUserNotFoundException()
        {
            // Arrange
            int userId = 1;
            int articleId = 2;

            Expression<Func<User, object>>[] propertiesToInclude = { u => u.UserFavorites };

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(
            r =>
            r.GetById(userId,
            It.Is<Expression<Func<User, object>>[]>(
            selector => ExpressionHelper.AreExpressionArraysEqual(selector, propertiesToInclude))))
            .Returns((User)null);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            // Arrange - create target
            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object,
            this._securedPasswordHelperMock.Object);

            // Act and Assert
            Assert.Throws<UserNotFoundException>(() => target.RemoveArticleFromFavorites(userId, articleId));

            userRepositoryMock.Verify(
            r =>
            r.GetById(userId,
            It.Is<Expression<Func<User, object>>[]>(
            selector => ExpressionHelper.AreExpressionArraysEqual(selector, propertiesToInclude))), Times.Once);

            userRepositoryMock.Verify(r => r.Update(It.Is<User>(u => u.UserId == userId)), Times.Never);

            unitOfWorkMock.Verify(u => u.Save(), Times.Never);
        }

        [Test]
        public void RemoveArticleFromFavorites_UserIdIsLessOrEqualToZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            int articleId = 1;

            IUserService target = new UserService(new Mock<IUnitOfWork>().Object, this._userValidationMock.Object,
            this._securedPasswordHelperMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => target.RemoveArticleFromFavorites(-1, articleId));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.RemoveArticleFromFavorites(0, articleId));
        }

        [Test]
        public void UnreadArticle_AllCredentialsAreValid_RemovesArticleFromFavorites()
        {
            // Arrange
            int userId = 1;
            int articleId = 2;
            UserReadHistory[] userReadHistories =
            {
                new UserReadHistory { UserId = userId, ArticleId = articleId, ReadDate = DateTime.Now },
                new UserReadHistory { UserId = userId, ArticleId = articleId + 1, ReadDate = DateTime.Now },
                new UserReadHistory { UserId = userId, ArticleId = articleId + 2, ReadDate = DateTime.Now }
            };

            User user = new User { UserId = userId, UserReadHistories = userReadHistories.ToList() };

            Expression<Func<User, object>>[] propertiesToInclude = { u => u.UserReadHistories };

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(
            r =>
            r.GetById(userId,
            It.Is<Expression<Func<User, object>>[]>(
            selector => ExpressionHelper.AreExpressionArraysEqual(selector, propertiesToInclude))))
            .Returns(user);

            IEnumerable<UserReadHistory> newUserReadHistories = null;

            userRepositoryMock.Setup(r => r.Update(It.Is<User>(u => u.UserId == userId)))
            .Callback((User u) => newUserReadHistories = u.UserReadHistories);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            // Arrange - create target
            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object,
            this._securedPasswordHelperMock.Object);

            // Act
            target.UnreadArticle(userId, articleId);

            // Assert
            Assert.IsNotNull(newUserReadHistories);
            Assert.AreEqual(userReadHistories.Count() - 1, newUserReadHistories.Count());
            Assert.AreEqual(0, newUserReadHistories.Count(h => h.UserId == userId && h.ArticleId == articleId));

            userRepositoryMock.Verify(
            r =>
            r.GetById(userId,
            It.Is<Expression<Func<User, object>>[]>(
            selector => ExpressionHelper.AreExpressionArraysEqual(selector, propertiesToInclude))), Times.Once);

            userRepositoryMock.Verify(r => r.Update(It.Is<User>(u => u.UserId == userId)), Times.Once);

            unitOfWorkMock.Verify(u => u.Save(), Times.Once);
        }

        [Test]
        public void UnreadArticle_ArticleHasNotBeenRead_ThrowsArticleNotFoundException()
        {
            // Arrange
            int userId = 1;
            int articleId = 2;

            User user = new User { UserId = userId, UserReadHistories = new List<UserReadHistory>() };

            Expression<Func<User, object>>[] propertiesToInclude = { u => u.UserReadHistories };

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(
            r =>
            r.GetById(userId,
            It.Is<Expression<Func<User, object>>[]>(
            selector => ExpressionHelper.AreExpressionArraysEqual(selector, propertiesToInclude))))
            .Returns(user);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            // Arrange - create target
            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object,
            this._securedPasswordHelperMock.Object);

            // Act and Assert
            Assert.Throws<ArticleNotFoundException>(() => target.UnreadArticle(userId, articleId));

            userRepositoryMock.Verify(
            r =>
            r.GetById(userId,
            It.Is<Expression<Func<User, object>>[]>(
            selector => ExpressionHelper.AreExpressionArraysEqual(selector, propertiesToInclude))), Times.Once);

            userRepositoryMock.Verify(r => r.Update(It.Is<User>(u => u.UserId == userId)), Times.Never);

            unitOfWorkMock.Verify(u => u.Save(), Times.Never);
        }

        [Test]
        public void UnreadArticle_ArticleIdIsLessOrEqualToZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            int userId = 1;

            IUserService target = new UserService(new Mock<IUnitOfWork>().Object, this._userValidationMock.Object,
            this._securedPasswordHelperMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => target.UnreadArticle(userId, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.UnreadArticle(userId, 0));
        }

        [Test]
        public void UnreadArticle_NonexistentUserId_ThrowsUserNotFoundException()
        {
            // Arrange
            int userId = 1;
            int articleId = 2;

            Expression<Func<User, object>>[] propertiesToInclude = { u => u.UserReadHistories };

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(
            r =>
            r.GetById(userId,
            It.Is<Expression<Func<User, object>>[]>(
            selector => ExpressionHelper.AreExpressionArraysEqual(selector, propertiesToInclude))))
            .Returns((User)null);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            // Arrange - create target
            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object,
            this._securedPasswordHelperMock.Object);

            // Act and Assert
            Assert.Throws<UserNotFoundException>(() => target.UnreadArticle(userId, articleId));

            userRepositoryMock.Verify(
            r =>
            r.GetById(userId,
            It.Is<Expression<Func<User, object>>[]>(
            selector => ExpressionHelper.AreExpressionArraysEqual(selector, propertiesToInclude))), Times.Once);

            userRepositoryMock.Verify(r => r.Update(It.Is<User>(u => u.UserId == userId)), Times.Never);

            unitOfWorkMock.Verify(u => u.Save(), Times.Never);
        }

        [Test]
        public void UnreadArticle_UserIdIsLessOrEqualToZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            int articleId = 1;

            IUserService target = new UserService(new Mock<IUnitOfWork>().Object, this._userValidationMock.Object,
            this._securedPasswordHelperMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => target.UnreadArticle(-1, articleId));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.UnreadArticle(0, articleId));
        }

        private void MockSecuredPasswordHelper()
        {
            this._securedPasswordHelperMock = new Mock<ISecuredPasswordHelper>();

            this._securedPasswordHelperMock.Setup(h => h.GetSecuredPassword(It.IsAny<string>()))
            .Returns((string password) => new SecuredPassword(password, "password_salt"));

            this._securedPasswordHelperMock.Setup(h => h.ArePasswordsEqual(It.IsAny<string>(), It.IsAny<SecuredPassword>()))
            .Returns((string password, SecuredPassword securedPassword) => password == securedPassword.Hash);
        }

        private void MockUserValidation()
        {
            this._userValidationMock = new Mock<IUserValidation>();

            this._userValidationMock.Setup(v => v.IsValidEmail(It.IsAny<string>()))
            .Returns(true);

            this._userValidationMock.Setup(v => v.IsValidName(It.IsAny<string>()))
            .Returns(true);

            this._userValidationMock.Setup(v => v.GetPasswordStrength(It.IsAny<string>()))
            .Returns(PasswordStrength.Strong);
        }
    }
}