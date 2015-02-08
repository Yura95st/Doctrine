namespace Doctrine.Domain.Tests.Services.Concrete
{
    using System;

    using Doctrine.Domain.Dal;
    using Doctrine.Domain.Dal.Repositories.Abstract;
    using Doctrine.Domain.Exceptions;
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
        public void Authenticate_CredentialsAreValid_ReturnsUser()
        {
            // Arrange
            User testUser = new User
            {
                UserId = 1, FullName = "user_fullName", Email = "user@email.com", Password = "user_password",
                RegistrationDate = DateTime.Now
            };

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(r => r.GetByEmail(testUser.Email))
            .Returns(testUser);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            // Act
            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object);
            User user = target.Authenticate(testUser.Email, testUser.Password);

            // Assert
            Assert.AreSame(user, testUser);
            userRepositoryMock.Verify(r => r.GetByEmail(testUser.Email), Times.Once);
        }

        [Test]
        public void Authenticate_EmailOrPasswordIsEmpty_ThrowsArgumentException()
        {
            // Arrange
            IUserService target = new UserService(new Mock<IUnitOfWork>().Object, this._userValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentException>(() => target.Authenticate("", "password"));
            Assert.Throws<ArgumentException>(() => target.Authenticate("user@email.com", ""));
        }

        [Test]
        public void Authenticate_EmailOrPasswordIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IUserService target = new UserService(new Mock<IUnitOfWork>().Object, this._userValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.Authenticate(null, null));
            Assert.Throws<ArgumentNullException>(() => target.Authenticate("user@email.com", null));
            Assert.Throws<ArgumentNullException>(() => target.Authenticate(null, "password"));
        }

        [Test]
        public void Authenticate_InvalidEmailFormat_ThrowsInvalidEmailFormatException()
        {
            // Arrange
            string email = "invalid@email.com";
            string password = "password";

            this._userValidationMock.Setup(v => v.IsValidEmail(email))
            .Returns(false);

            IUserService target = new UserService(new Mock<IUnitOfWork>().Object, this._userValidationMock.Object);

            // Act and Assert
            Assert.Throws<InvalidEmailFormatException>(() => target.Authenticate(email, password));
        }

        [Test]
        public void Authenticate_NonexistentEmail_ThrowsNonexistentEmailException()
        {
            // Arrange
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

            // Act
            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object);

            Assert.Throws<NonexistentEmailException>(() => target.Authenticate(email, password));

            // Assert
            userRepositoryMock.Verify(r => r.GetByEmail(email), Times.Once);
        }

        [Test]
        public void Authenticate_WrongPassword_ThrowsWrongPasswordException()
        {
            // Arrange
            string password = "wrong_password";

            User testUser = new User
            {
                UserId = 1, FullName = "user_fullName", Email = "user@email.com", Password = "user_password",
                RegistrationDate = DateTime.Now
            };

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(r => r.GetByEmail(testUser.Email))
            .Returns(testUser);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            // Act
            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object);
            Assert.Throws<WrongPasswordException>(() => target.Authenticate(testUser.Email, password));

            // Assert
            userRepositoryMock.Verify(r => r.GetByEmail(testUser.Email), Times.Once);
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

            // Act
            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object);
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

            // Act
            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object);
            User user = target.GetById(testUser.UserId);

            // Assert
            Assert.AreSame(user, testUser);
            userRepositoryMock.Verify(r => r.GetById(testUser.UserId), Times.Once);
        }

        [SetUp]
        public void Init()
        {
            this.MockUserValidation();
        }

        private void MockUserValidation()
        {
            this._userValidationMock = new Mock<IUserValidation>();

            this._userValidationMock.Setup(v => v.IsValidEmail(It.IsAny<string>()))
            .Returns(true);
        }
    }
}