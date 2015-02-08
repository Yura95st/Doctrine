namespace Doctrine.Domain.Tests.Services.Concrete
{
    using System;

    using Doctrine.Domain.Dal;
    using Doctrine.Domain.Dal.Repositories.Abstract;
    using Doctrine.Domain.Models;
    using Doctrine.Domain.Services.Abstract;
    using Doctrine.Domain.Services.Concrete;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class UserServiceTests
    {
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
            IUserService target = new UserService(unitOfWorkMock.Object);
            User user = target.GetById(userId);

            // Assert
            Assert.IsNull(user);
            userRepositoryMock.Verify(r => r.GetById(userId), Times.Once);
        }

        [Test]
        public void GetById_ReturnsUser()
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
            IUserService target = new UserService(unitOfWorkMock.Object);
            User user = target.GetById(testUser.UserId);

            // Assert
            Assert.AreSame(user, testUser);
            userRepositoryMock.Verify(r => r.GetById(testUser.UserId), Times.Once);
        }

        [Test]
        public void GetById_UserIdIsLessOrEqualToZero_ThrowsArgumentOutOfRangeException()
        {
            IUserService target = new UserService(new Mock<IUnitOfWork>().Object);

            Assert.Throws<ArgumentOutOfRangeException>(() => target.GetById(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.GetById(0));
        }
    }
}