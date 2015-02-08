namespace Doctrine.Domain.Tests.Services.Concrete
{
    using System;

    using Doctrine.Domain.Dal;
    using Doctrine.Domain.Dal.Repositories.Abstract;
    using Doctrine.Domain.Exceptions;
    using Doctrine.Domain.Exceptions.InvalidFormat;
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

            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object);

            // Act
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

            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object);

            // Act and Assert
            Assert.Throws<NonexistentEmailException>(() => target.Authenticate(email, password));

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

            IUserService target = new UserService(unitOfWorkMock.Object, this._userValidationMock.Object);

            // Act
            Assert.Throws<WrongPasswordException>(() => target.Authenticate(testUser.Email, password));

            // Assert
            userRepositoryMock.Verify(r => r.GetByEmail(testUser.Email), Times.Once);
        }

        [Test]
        public void Create_AllCredentialsAreValid_CreatesAndReturnsNewUser()
        {
            // Arrange
            string fullName = "user_fullName";
            string email = "user@email.com";
            string password = "user_password";

            int userId = 1;
            DateTime registrationDate = DateTime.Now;

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(r => r.GetByEmail(email))
            .Returns((User)null);

            User newUser = null;

            userRepositoryMock.Setup(r => r.Insert(It.IsAny<User>()))
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
            Assert.AreNotEqual(new DateTime(), user.RegistrationDate);

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

            this._userValidationMock.Setup(v => v.IsValidFullName(It.IsAny<string>()))
            .Returns(true);

            this._userValidationMock.Setup(v => v.IsValidPassword(It.IsAny<string>()))
            .Returns(true);
        }
    }
}