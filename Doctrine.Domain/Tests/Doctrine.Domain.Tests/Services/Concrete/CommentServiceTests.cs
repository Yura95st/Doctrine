namespace Doctrine.Domain.Tests.Services.Concrete
{
    using System;

    using Doctrine.Domain.Dal;
    using Doctrine.Domain.Models;
    using Doctrine.Domain.Services.Abstract;
    using Doctrine.Domain.Services.Concrete;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class CommentServiceTests
    {
        [Test]
        public void CanEdit_CommentIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            int userId = 1;

            ICommentService target = new CommentService(new Mock<IUnitOfWork>().Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.CanEdit(userId, null));
        }

        [Test]
        public void CanEdit_PermittedPeriodForEditingExpired_ReturnsFalse()
        {
            // Arrange
            int userId = 1;
            int permittedPeriodForEditing = 300;

            Comment comment = new Comment
            { UserId = userId + 1, Date = DateTime.Now.AddSeconds(permittedPeriodForEditing + 1) };

            ICommentService target = new CommentService(new Mock<IUnitOfWork>().Object, permittedPeriodForEditing);

            // Act
            bool result = target.CanEdit(userId, comment);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void CanEdit_UserCanEditTheComment_ReturnsTrue()
        {
            // Arrange
            int permittedPeriodForEditing = 300;

            Comment comment = new Comment { UserId = 1, Date = DateTime.Now };

            ICommentService target = new CommentService(new Mock<IUnitOfWork>().Object, permittedPeriodForEditing);

            // Act
            bool result = target.CanEdit(comment.UserId, comment);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void CanEdit_UserIdIsLessOrEqualToZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            Comment comment = new Comment();

            ICommentService target = new CommentService(new Mock<IUnitOfWork>().Object);

            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => target.CanEdit(-1, comment));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.CanEdit(0, comment));
        }

        [Test]
        public void CanEdit_UserIsNotAuthorOfTheComment_ReturnsFalse()
        {
            // Arrange
            int userId = 1;
            Comment comment = new Comment { UserId = userId + 1 };

            ICommentService target = new CommentService(new Mock<IUnitOfWork>().Object);

            // Act
            bool result = target.CanEdit(userId, comment);

            //Assert
            Assert.IsFalse(result);
        }
    }
}