namespace Doctrine.Domain.Tests.Services.Concrete
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Doctrine.Domain.Dal;
    using Doctrine.Domain.Dal.Repositories.Abstract;
    using Doctrine.Domain.Exceptions;
    using Doctrine.Domain.Exceptions.InvalidFormat;
    using Doctrine.Domain.Exceptions.NotFound;
    using Doctrine.Domain.Models;
    using Doctrine.Domain.Services.Abstract;
    using Doctrine.Domain.Services.Concrete;
    using Doctrine.Domain.Validation.Abstract;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IUserValidation> _userValidationMock;

        [Test]
        public void AddArticleToFavorites_AllCredentialsAreValid_AddsArticleToFavorites()
        {
            // Arrange
            User user = new User { UserId = 1 };

            Article article = new Article { ArticleId = 2 };

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(
            r => r.Get(It.IsAny<Expression<Func<User, bool>>>(), null, It.IsAny<Expression<Func<User, object>>[]>()))
            .Returns(new[] { user });

            UserFavorite newUserFavorite = null;
            userRepositoryMock.Setup(r => r.Update(It.Is<User>(u => u.UserId == user.UserId)))
            .Callback((User u) => newUserFavorite = u.UserFavorites.FirstOrDefault());

            // Arrange - mock articleRepository
            Mock<IArticleRepository> articleRepositoryMock = new Mock<IArticleRepository>();
            articleRepositoryMock.Setup(r => r.GetById(article.ArticleId))
            .Returns(article);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            unitOfWorkMock.SetupGet(u => u.ArticleRepository)
            .Returns(articleRepositoryMock.Object);

            unitOfWorkMock.Setup(u => u.Save())
            .Callback(() => newUserFavorite.UserId = user.UserId);

            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object);

            // Act
            target.AddArticleToFavorites(user.UserId, article.ArticleId);

            // Assert
            Assert.AreEqual(user.UserId, newUserFavorite.UserId);
            Assert.AreEqual(article.ArticleId, newUserFavorite.ArticleId);
            Assert.IsTrue(new DateTime() != newUserFavorite.AddDate);

            userRepositoryMock.Verify(
            r => r.Get(It.IsAny<Expression<Func<User, bool>>>(), null, It.IsAny<Expression<Func<User, object>>[]>()),
            Times.Once);
            userRepositoryMock.Verify(r => r.Update(It.Is<User>(u => u.UserId == user.UserId)), Times.Once);

            articleRepositoryMock.Verify(r => r.GetById(article.ArticleId), Times.Once);

            unitOfWorkMock.Verify(u => u.Save(), Times.Once);
        }

        [Test]
        public void AddArticleToFavorites_ArticleAlreadyAdded_UpdatesAddedDate()
        {
            // Arrange
            int userId = 1;
            int articleId = 2;
            DateTime userFavoriteAddDate = new DateTime();

            UserFavorite userFavorite = new UserFavorite
            { UserId = userId, ArticleId = articleId, AddDate = userFavoriteAddDate };

            User user = new User { UserId = userId, UserFavorites = new[] { userFavorite } };

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(
            r => r.Get(It.IsAny<Expression<Func<User, bool>>>(), null, It.IsAny<Expression<Func<User, object>>[]>()))
            .Returns(new[] { user });

            UserFavorite newUserFavorite = null;
            userRepositoryMock.Setup(r => r.Update(It.Is<User>(u => u.UserId == userId)))
            .Callback((User u) => newUserFavorite = u.UserFavorites.FirstOrDefault());

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object);

            // Act
            target.AddArticleToFavorites(userId, articleId);

            // Assert
            Assert.AreEqual(userId, newUserFavorite.UserId);
            Assert.AreEqual(articleId, newUserFavorite.ArticleId);
            Assert.IsTrue(userFavoriteAddDate != newUserFavorite.AddDate);

            userRepositoryMock.Verify(
            r => r.Get(It.IsAny<Expression<Func<User, bool>>>(), null, It.IsAny<Expression<Func<User, object>>[]>()),
            Times.Once);
            userRepositoryMock.Verify(r => r.Update(It.Is<User>(u => u.UserId == userId)), Times.Once);

            unitOfWorkMock.Verify(u => u.Save(), Times.Once);
        }

        [Test]
        public void AddArticleToFavorites_ArticleIdIsLessOrEqualToZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            int userId = 1;

            IUserService target = new UserService(new Mock<IUnitOfWork>().Object, this._userValidationMock.Object);

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

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(
            r => r.Get(It.IsAny<Expression<Func<User, bool>>>(), null, It.IsAny<Expression<Func<User, object>>[]>()))
            .Returns(new[] { user });

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

            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArticleNotFoundException>(() => target.AddArticleToFavorites(user.UserId, articleId));

            userRepositoryMock.Verify(
            r => r.Get(It.IsAny<Expression<Func<User, bool>>>(), null, It.IsAny<Expression<Func<User, object>>[]>()),
            Times.Once);
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

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(
            r => r.Get(It.IsAny<Expression<Func<User, bool>>>(), null, It.IsAny<Expression<Func<User, object>>[]>()))
            .Returns(new User[] { });

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object);

            // Act and Assert
            Assert.Throws<UserNotFoundException>(() => target.AddArticleToFavorites(userId, articleId));

            userRepositoryMock.Verify(
            r => r.Get(It.IsAny<Expression<Func<User, bool>>>(), null, It.IsAny<Expression<Func<User, object>>[]>()),
            Times.Once);
            userRepositoryMock.Verify(r => r.Update(It.Is<User>(u => u.UserId == userId)), Times.Never);

            unitOfWorkMock.Verify(u => u.Save(), Times.Never);
        }

        [Test]
        public void AddArticleToFavorites_UserIdIsLessOrEqualToZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            int articleId = 1;

            IUserService target = new UserService(new Mock<IUnitOfWork>().Object, this._userValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => target.AddArticleToFavorites(-1, articleId));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.AddArticleToFavorites(0, articleId));
        }

        [Test]
        public void Authenticate_CredentialsAreValid_AuthenticatesVisitorAndReturnsUser()
        {
            // Arrange
            Visitor visitor = new Visitor { VisitorId = 1, IpAddress = "127.0.0.1" };

            User user = new User { UserId = 2, Email = "user@email.com", Password = "user_password" };

            int userActivityId = 1;

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(r => r.GetByEmail(user.Email))
            .Returns(user);

            UserActivity newUserActivity = null;
            userRepositoryMock.Setup(r => r.Update(It.Is<User>(u => u.UserId == user.UserId)))
            .Callback((User u) => newUserActivity = u.UserActivities.FirstOrDefault());

            // Arrange - mock visitorRepository
            Mock<IVisitorRepository> visitorRepositoryMock = new Mock<IVisitorRepository>();
            visitorRepositoryMock.Setup(r => r.GetById(visitor.VisitorId))
            .Returns(visitor);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            unitOfWorkMock.SetupGet(u => u.VisitorRepository)
            .Returns(visitorRepositoryMock.Object);

            unitOfWorkMock.Setup(u => u.Save())
            .Callback(() =>
                      {
                          newUserActivity.ActivityId = userActivityId;
                          newUserActivity.UserId = user.UserId;
                      });

            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object);

            // Act
            User authenticatedUser = target.Authenticate(visitor.VisitorId, user.Email, user.Password);

            // Assert
            Assert.AreEqual(user.UserId, authenticatedUser.UserId);
            Assert.AreEqual(user.Email, authenticatedUser.Email);
            Assert.AreEqual(user.Password, authenticatedUser.Password);

            UserActivity lastUserActivity = authenticatedUser.UserActivities.FirstOrDefault();

            Assert.IsNotNull(lastUserActivity);
            Assert.AreEqual(userActivityId, lastUserActivity.ActivityId);
            Assert.AreEqual(user.UserId, lastUserActivity.UserId);
            Assert.AreEqual(visitor.VisitorId, lastUserActivity.VisitorId);
            Assert.IsTrue(new DateTime() != lastUserActivity.LogOnDate);

            userRepositoryMock.Verify(r => r.GetByEmail(user.Email), Times.Once);
            userRepositoryMock.Verify(r => r.Update(It.Is<User>(u => u.UserId == user.UserId)), Times.Once);

            visitorRepositoryMock.Verify(r => r.GetById(visitor.VisitorId), Times.Once);

            unitOfWorkMock.Verify(u => u.Save(), Times.Once);
        }

        [Test]
        public void Authenticate_EmailOrPasswordIsEmpty_ThrowsArgumentException()
        {
            // Arrange
            int visitorId = 1;

            IUserService target = new UserService(new Mock<IUnitOfWork>().Object, this._userValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentException>(() => target.Authenticate(visitorId, "", "password"));
            Assert.Throws<ArgumentException>(() => target.Authenticate(visitorId, "user@email.com", ""));
        }

        [Test]
        public void Authenticate_EmailOrPasswordIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            int visitorId = 1;

            IUserService target = new UserService(new Mock<IUnitOfWork>().Object, this._userValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.Authenticate(visitorId, null, null));
            Assert.Throws<ArgumentNullException>(() => target.Authenticate(visitorId, "user@email.com", null));
            Assert.Throws<ArgumentNullException>(() => target.Authenticate(visitorId, null, "password"));
        }

        [Test]
        public void Authenticate_InvalidEmailFormat_ThrowsInvalidEmailFormatException()
        {
            // Arrange
            int visitorId = 1;
            string email = "invalid@email.com";
            string password = "password";

            this._userValidationMock.Setup(v => v.IsValidEmail(email))
            .Returns(false);

            IUserService target = new UserService(new Mock<IUnitOfWork>().Object, this._userValidationMock.Object);

            // Act and Assert
            Assert.Throws<InvalidEmailFormatException>(() => target.Authenticate(visitorId, email, password));
        }

        [Test]
        public void Authenticate_NonexistentEmail_ThrowsUserNotFoundException()
        {
            // Arrange
            int visitorId = 1;
            string email = "nonexistent@email.com";
            string password = "password";

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(r => r.GetByEmail(email))
            .Returns((User)null);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object);

            // Act and Assert
            Assert.Throws<UserNotFoundException>(() => target.Authenticate(visitorId, email, password));

            userRepositoryMock.Verify(r => r.GetByEmail(email), Times.Once);
        }

        [Test]
        public void Authenticate_NonexistentVisitorId_ThrowsVisitorNotFoundException()
        {
            // Arrange
            int visitorId = 1;

            User user = new User { UserId = 1, Email = "user@email.com", Password = "user_password" };

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

            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object);

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
            string email = "user@email.com";
            string password = "password";

            IUserService target = new UserService(new Mock<IUnitOfWork>().Object, this._userValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Authenticate(-1, email, password));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Authenticate(0, email, password));
        }

        [Test]
        public void Authenticate_WrongPassword_ThrowsWrongPasswordException()
        {
            // Arrange
            int visitorId = 1;
            string password = "wrong_password";

            User user = new User { UserId = 1, Email = "user@email.com", Password = "user_password" };

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(r => r.GetByEmail(user.Email))
            .Returns(user);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object);

            // Act
            Assert.Throws<WrongPasswordException>(() => target.Authenticate(visitorId, user.Email, password));

            // Assert
            userRepositoryMock.Verify(r => r.GetByEmail(user.Email), Times.Once);
        }

        [Test]
        public void Create_AllCredentialsAreValid_CreatesAndReturnsNewUser()
        {
            // Arrange
            string fullName = "user_fullName";
            string email = "user@email.com";
            string password = "user_password";

            int userId = 1;

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(r => r.GetByEmail(email))
            .Returns((User)null);

            User newUser = null;
            userRepositoryMock.Setup(r => r.Insert(It.Is<User>(u => u.Email == email)))
            .Callback((User u) => newUser = u);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            unitOfWorkMock.Setup(u => u.Save())
            .Callback(() => newUser.UserId = userId);

            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object);

            // Act
            User user = target.Create(email, fullName, password);

            // Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(userId, user.UserId);
            Assert.AreEqual(email, user.Email);
            Assert.AreEqual(fullName, user.FullName);
            Assert.AreEqual(password, user.Password);
            Assert.IsTrue(new DateTime() != user.RegistrationDate);

            userRepositoryMock.Verify(r => r.GetByEmail(email), Times.Once);
            userRepositoryMock.Verify(
            r => r.Insert(It.Is<User>(u => u.Email == email && u.FullName == fullName && u.Password == password)), Times.Once);

            unitOfWorkMock.Verify(u => u.Save(), Times.Once);
        }

        [Test]
        public void Create_EmailAlreadyExists_ThrowsEmailAlreadyExistsException()
        {
            // Arrange
            string fullName = "user_fullName";
            string email = "user@email.com";
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

            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object);

            // Act and Assert
            Assert.Throws<EmailAlreadyExistsException>(() => target.Create(email, fullName, password));

            userRepositoryMock.Verify(r => r.GetByEmail(email), Times.Once);
            userRepositoryMock.Verify(r => r.Insert(It.Is<User>(u => u.Email == email)), Times.Never);

            unitOfWorkMock.Verify(u => u.Save(), Times.Never);
        }

        [Test]
        public void Create_EmailOrFullNameOrPasswordIsEmpty_ThrowsArgumentException()
        {
            // Arrange
            IUserService target = new UserService(new Mock<IUnitOfWork>().Object, this._userValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentException>(() => target.Create("", "fullName", "password"));
            Assert.Throws<ArgumentException>(() => target.Create("user@email.com", "", "password"));
            Assert.Throws<ArgumentException>(() => target.Create("user@email.com", "fullName", ""));
        }

        [Test]
        public void Create_EmailOrFullNameOrPasswordIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IUserService target = new UserService(new Mock<IUnitOfWork>().Object, this._userValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.Create(null, "fullName", "password"));
            Assert.Throws<ArgumentNullException>(() => target.Create("user@email.com", null, "password"));
            Assert.Throws<ArgumentNullException>(() => target.Create("user@email.com", "fullName", null));
        }

        [Test]
        public void Create_InvalidEmailFormat_ThrowsInvalidEmailFormatException()
        {
            // Arrange
            string invalidEmail = "invalid@email.com";
            string fullName = "fullName";
            string password = "password";

            this._userValidationMock.Setup(v => v.IsValidEmail(invalidEmail))
            .Returns(false);

            IUserService target = new UserService(new Mock<IUnitOfWork>().Object, this._userValidationMock.Object);

            // Act and Assert
            Assert.Throws<InvalidEmailFormatException>(() => target.Create(invalidEmail, fullName, password));
        }

        [Test]
        public void Create_InvalidEmailFormat_ThrowsInvalidFullNameFormatException()
        {
            // Arrange
            string email = "invalid@email.com";
            string invalidFullName = "fullName";
            string password = "password";

            this._userValidationMock.Setup(v => v.IsValidFullName(invalidFullName))
            .Returns(false);

            IUserService target = new UserService(new Mock<IUnitOfWork>().Object, this._userValidationMock.Object);

            // Act and Assert
            Assert.Throws<InvalidFullNameFormatException>(() => target.Create(email, invalidFullName, password));
        }

        [Test]
        public void Create_InvalidPasswordFormat_ThrowsInvalidPasswordFormatException()
        {
            // Arrange
            string email = "user@email.com";
            string fullName = "fullName";
            string invalidPassword = "invalid_password";

            this._userValidationMock.Setup(v => v.IsValidPassword(invalidPassword))
            .Returns(false);

            IUserService target = new UserService(new Mock<IUnitOfWork>().Object, this._userValidationMock.Object);

            // Act and Assert
            Assert.Throws<InvalidPasswordFormatException>(() => target.Create(email, fullName, invalidPassword));
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

            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object);

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
            IUserService target = new UserService(new Mock<IUnitOfWork>().Object, this._userValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => target.GetById(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.GetById(0));
        }

        [Test]
        public void GetById_UserIdIsValid_ReturnsUser()
        {
            // Arrange
            User testUser = new User
            {
                UserId = 1, FullName = "user_fullName", Email = "user@email.com", Password = "user_password",
                RegistrationDate = DateTime.Now
            };

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(r => r.GetById(testUser.UserId))
            .Returns(testUser);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object);

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
        }

        [Test]
        public void ReadArticle_AllCredentialsAreValid_ReadsArticle()
        {
            // Arrange
            User user = new User { UserId = 1 };

            Article article = new Article { ArticleId = 2 };

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(
            r => r.Get(It.IsAny<Expression<Func<User, bool>>>(), null, It.IsAny<Expression<Func<User, object>>[]>()))
            .Returns(new[] { user });

            UserReadHistory userReadHistory = null;
            userRepositoryMock.Setup(r => r.Update(It.Is<User>(u => u.UserId == user.UserId)))
            .Callback((User u) => userReadHistory = u.UserReadHistories.FirstOrDefault());

            // Arrange - mock articleRepository
            Mock<IArticleRepository> articleRepositoryMock = new Mock<IArticleRepository>();
            articleRepositoryMock.Setup(r => r.GetById(article.ArticleId))
            .Returns(article);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            unitOfWorkMock.SetupGet(u => u.ArticleRepository)
            .Returns(articleRepositoryMock.Object);

            unitOfWorkMock.Setup(u => u.Save())
            .Callback(() => userReadHistory.UserId = user.UserId);

            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object);

            // Act
            target.ReadArticle(user.UserId, article.ArticleId);

            // Assert
            Assert.AreEqual(user.UserId, userReadHistory.UserId);
            Assert.AreEqual(article.ArticleId, userReadHistory.ArticleId);
            Assert.IsTrue(new DateTime() != userReadHistory.ReadDate);

            userRepositoryMock.Verify(
            r => r.Get(It.IsAny<Expression<Func<User, bool>>>(), null, It.IsAny<Expression<Func<User, object>>[]>()),
            Times.Once);
            userRepositoryMock.Verify(r => r.Update(It.Is<User>(u => u.UserId == user.UserId)), Times.Once);

            articleRepositoryMock.Verify(r => r.GetById(article.ArticleId), Times.Once);

            unitOfWorkMock.Verify(u => u.Save(), Times.Once);
        }

        [Test]
        public void ReadArticle_ArticleAlreadyRead_UpdatesReadDate()
        {
            // Arrange
            int userId = 1;
            int articleId = 2;
            DateTime readDate = new DateTime();

            UserReadHistory userReadHistory = new UserReadHistory
            { UserId = userId, ArticleId = articleId, ReadDate = readDate };

            User user = new User { UserId = userId, UserReadHistories = new[] { userReadHistory } };

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(
            r => r.Get(It.IsAny<Expression<Func<User, bool>>>(), null, It.IsAny<Expression<Func<User, object>>[]>()))
            .Returns(new[] { user });

            UserReadHistory newUserReadHistory = null;
            userRepositoryMock.Setup(r => r.Update(It.Is<User>(u => u.UserId == userId)))
            .Callback((User u) => newUserReadHistory = u.UserReadHistories.FirstOrDefault());

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object);

            // Act
            target.ReadArticle(userId, articleId);

            // Assert
            Assert.AreEqual(userId, newUserReadHistory.UserId);
            Assert.AreEqual(articleId, newUserReadHistory.ArticleId);
            Assert.IsTrue(readDate != newUserReadHistory.ReadDate);

            userRepositoryMock.Verify(
            r => r.Get(It.IsAny<Expression<Func<User, bool>>>(), null, It.IsAny<Expression<Func<User, object>>[]>()),
            Times.Once);
            userRepositoryMock.Verify(r => r.Update(It.Is<User>(u => u.UserId == userId)), Times.Once);

            unitOfWorkMock.Verify(u => u.Save(), Times.Once);
        }

        [Test]
        public void ReadArticle_ArticleIdIsLessOrEqualToZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            int userId = 1;

            IUserService target = new UserService(new Mock<IUnitOfWork>().Object, this._userValidationMock.Object);

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

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(
            r => r.Get(It.IsAny<Expression<Func<User, bool>>>(), null, It.IsAny<Expression<Func<User, object>>[]>()))
            .Returns(new[] { user });

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

            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArticleNotFoundException>(() => target.ReadArticle(user.UserId, articleId));

            userRepositoryMock.Verify(
            r => r.Get(It.IsAny<Expression<Func<User, bool>>>(), null, It.IsAny<Expression<Func<User, object>>[]>()),
            Times.Once);
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

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(
            r => r.Get(It.IsAny<Expression<Func<User, bool>>>(), null, It.IsAny<Expression<Func<User, object>>[]>()))
            .Returns(new User[] { });

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object);

            // Act and Assert
            Assert.Throws<UserNotFoundException>(() => target.ReadArticle(userId, articleId));

            userRepositoryMock.Verify(
            r => r.Get(It.IsAny<Expression<Func<User, bool>>>(), null, It.IsAny<Expression<Func<User, object>>[]>()),
            Times.Once);
            userRepositoryMock.Verify(r => r.Update(It.Is<User>(u => u.UserId == userId)), Times.Never);

            unitOfWorkMock.Verify(u => u.Save(), Times.Never);
        }

        [Test]
        public void ReadArticle_UserIdIsLessOrEqualToZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            int articleId = 1;

            IUserService target = new UserService(new Mock<IUnitOfWork>().Object, this._userValidationMock.Object);

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

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(
            r => r.Get(It.IsAny<Expression<Func<User, bool>>>(), null, It.IsAny<Expression<Func<User, object>>[]>()))
            .Returns(new[] { user });

            IEnumerable<UserFavorite> newUserFavorites = null;
            userRepositoryMock.Setup(r => r.Update(It.Is<User>(u => u.UserId == userId)))
            .Callback((User u) => newUserFavorites = u.UserFavorites);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object);

            // Act
            target.RemoveArticleFromFavorites(userId, articleId);

            // Assert
            Assert.AreEqual(0, newUserFavorites.Count(f => f.ArticleId == articleId));
            Assert.AreEqual(userFavorites.Count() - 1, newUserFavorites.Count());

            userRepositoryMock.Verify(
            r => r.Get(It.IsAny<Expression<Func<User, bool>>>(), null, It.IsAny<Expression<Func<User, object>>[]>()),
            Times.Once);
            userRepositoryMock.Verify(r => r.Update(It.Is<User>(u => u.UserId == userId)), Times.Once);

            unitOfWorkMock.Verify(u => u.Save(), Times.Once);
        }

        [Test]
        public void RemoveArticleFromFavorites_ArticleHasNotBeenAddedToFavorites_ThrowsArticleNotFoundException()
        {
            // Arrange
            int userId = 1;
            int articleId = 2;

            User user = new User { UserId = userId, UserFavorites = new UserFavorite[] { } };

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(
            r => r.Get(It.IsAny<Expression<Func<User, bool>>>(), null, It.IsAny<Expression<Func<User, object>>[]>()))
            .Returns(new[] { user });

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArticleNotFoundException>(() => target.RemoveArticleFromFavorites(userId, articleId));

            userRepositoryMock.Verify(
            r => r.Get(It.IsAny<Expression<Func<User, bool>>>(), null, It.IsAny<Expression<Func<User, object>>[]>()),
            Times.Once);
            userRepositoryMock.Verify(r => r.Update(It.Is<User>(u => u.UserId == userId)), Times.Never);

            unitOfWorkMock.Verify(u => u.Save(), Times.Never);
        }

        [Test]
        public void RemoveArticleFromFavorites_ArticleIdIsLessOrEqualToZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            int userId = 1;

            IUserService target = new UserService(new Mock<IUnitOfWork>().Object, this._userValidationMock.Object);

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

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(
            r => r.Get(It.IsAny<Expression<Func<User, bool>>>(), null, It.IsAny<Expression<Func<User, object>>[]>()))
            .Returns(new User[] { });

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object);

            // Act and Assert
            Assert.Throws<UserNotFoundException>(() => target.RemoveArticleFromFavorites(userId, articleId));

            userRepositoryMock.Verify(
            r => r.Get(It.IsAny<Expression<Func<User, bool>>>(), null, It.IsAny<Expression<Func<User, object>>[]>()),
            Times.Once);
            userRepositoryMock.Verify(r => r.Update(It.Is<User>(u => u.UserId == userId)), Times.Never);

            unitOfWorkMock.Verify(u => u.Save(), Times.Never);
        }

        [Test]
        public void RemoveArticleFromFavorites_UserIdIsLessOrEqualToZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            int articleId = 1;

            IUserService target = new UserService(new Mock<IUnitOfWork>().Object, this._userValidationMock.Object);

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

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(
            r => r.Get(It.IsAny<Expression<Func<User, bool>>>(), null, It.IsAny<Expression<Func<User, object>>[]>()))
            .Returns(new[] { user });

            IEnumerable<UserReadHistory> newUserReadHistories = null;
            userRepositoryMock.Setup(r => r.Update(It.Is<User>(u => u.UserId == userId)))
            .Callback((User u) => newUserReadHistories = u.UserReadHistories);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object);

            // Act
            target.UnreadArticle(userId, articleId);

            // Assert
            Assert.IsNotNull(newUserReadHistories);
            Assert.AreEqual(0, newUserReadHistories.Count(f => f.ArticleId == articleId));
            Assert.AreEqual(userReadHistories.Count() - 1, newUserReadHistories.Count());

            userRepositoryMock.Verify(
            r => r.Get(It.IsAny<Expression<Func<User, bool>>>(), null, It.IsAny<Expression<Func<User, object>>[]>()),
            Times.Once);
            userRepositoryMock.Verify(r => r.Update(It.Is<User>(u => u.UserId == userId)), Times.Once);

            unitOfWorkMock.Verify(u => u.Save(), Times.Once);
        }

        [Test]
        public void UnreadArticle_ArticleHasNotBeenRead_ThrowsArticleNotFoundException()
        {
            // Arrange
            int userId = 1;
            int articleId = 2;

            User user = new User { UserId = userId, UserReadHistories = new UserReadHistory[] { } };

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(
            r => r.Get(It.IsAny<Expression<Func<User, bool>>>(), null, It.IsAny<Expression<Func<User, object>>[]>()))
            .Returns(new[] { user });

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArticleNotFoundException>(() => target.UnreadArticle(userId, articleId));

            userRepositoryMock.Verify(
            r => r.Get(It.IsAny<Expression<Func<User, bool>>>(), null, It.IsAny<Expression<Func<User, object>>[]>()),
            Times.Once);
            userRepositoryMock.Verify(r => r.Update(It.Is<User>(u => u.UserId == userId)), Times.Never);

            unitOfWorkMock.Verify(u => u.Save(), Times.Never);
        }

        [Test]
        public void UnreadArticle_ArticleIdIsLessOrEqualToZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            int userId = 1;

            IUserService target = new UserService(new Mock<IUnitOfWork>().Object, this._userValidationMock.Object);

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

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(
            r => r.Get(It.IsAny<Expression<Func<User, bool>>>(), null, It.IsAny<Expression<Func<User, object>>[]>()))
            .Returns(new User[] { });

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object);

            // Act and Assert
            Assert.Throws<UserNotFoundException>(() => target.UnreadArticle(userId, articleId));

            userRepositoryMock.Verify(
            r => r.Get(It.IsAny<Expression<Func<User, bool>>>(), null, It.IsAny<Expression<Func<User, object>>[]>()),
            Times.Once);
            userRepositoryMock.Verify(r => r.Update(It.Is<User>(u => u.UserId == userId)), Times.Never);

            unitOfWorkMock.Verify(u => u.Save(), Times.Never);
        }

        [Test]
        public void UnreadArticle_UserIdIsLessOrEqualToZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            int articleId = 1;

            IUserService target = new UserService(new Mock<IUnitOfWork>().Object, this._userValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => target.UnreadArticle(-1, articleId));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.UnreadArticle(0, articleId));
        }

        private void MockUserValidation()
        {
            this._userValidationMock = new Mock<IUserValidation>();

            this._userValidationMock.Setup(v => v.IsValidEmail(It.IsAny<string>()))
            .Returns(true);

            this._userValidationMock.Setup(v => v.IsValidFullName(It.IsAny<string>()))
            .Returns(true);

            this._userValidationMock.Setup(v => v.IsValidPassword(It.IsAny<string>()))
            .Returns(true);
        }
    }
}