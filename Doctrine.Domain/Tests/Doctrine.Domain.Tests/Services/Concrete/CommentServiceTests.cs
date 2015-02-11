namespace Doctrine.Domain.Tests.Services.Concrete
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Doctrine.Domain.Dal;
    using Doctrine.Domain.Dal.Repositories.Abstract;
    using Doctrine.Domain.Exceptions.NotFound;
    using Doctrine.Domain.Models;
    using Doctrine.Domain.Services.Abstract;
    using Doctrine.Domain.Services.Concrete;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class CommentServiceTests
    {
        [Test]
        public void AddVote_CommentIdIsLessOrEqualToZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            int userId = 1;
            bool voteIsPositive = true;

            ICommentService target = new CommentService(new Mock<IUnitOfWork>().Object);

            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => target.AddVote(-1, userId, voteIsPositive));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.AddVote(0, userId, voteIsPositive));
        }

        [Test]
        public void AddVote_NonexistentCommentId_ThrowsCommentNotFoundException()
        {
            // Arrange
            int commentId = 1;
            int userId = 1;
            bool voteIsPositive = true;

            // Arrange - mock commentRepository
            Mock<ICommentRepository> commentRepositoryMock = new Mock<ICommentRepository>();
            commentRepositoryMock.Setup(
            r => r.Get(It.IsAny<Expression<Func<Comment, bool>>>(), null, It.IsAny<Expression<Func<Comment, object>>[]>()))
            .Returns(new Comment[] { });

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(u => u.CommentRepository)
            .Returns(commentRepositoryMock.Object);

            ICommentService target = new CommentService(unitOfWorkMock.Object);

            // Act and Assert
            Assert.Throws<CommentNotFoundException>(() => target.AddVote(commentId, userId, voteIsPositive));

            commentRepositoryMock.Verify(
            r => r.Get(It.IsAny<Expression<Func<Comment, bool>>>(), null, It.IsAny<Expression<Func<Comment, object>>[]>()),
            Times.Once);
        }

        [Test]
        public void AddVote_UserHasNotVotedYet_AddsVote()
        {
            // Arrange
            int commentId = 1;
            int userId = 1;
            bool voteIsPositive = true;

            Comment comment = new Comment { CommentId = commentId, CommentVotes = new List<CommentVote>() };

            // Arrange - mock commentRepository
            Mock<ICommentRepository> commentRepositoryMock = new Mock<ICommentRepository>();
            commentRepositoryMock.Setup(
            r => r.Get(It.IsAny<Expression<Func<Comment, bool>>>(), null, It.IsAny<Expression<Func<Comment, object>>[]>()))
            .Returns(new[] { comment });

            IEnumerable<CommentVote> newCommentVotes = null;

            commentRepositoryMock.Setup(r => r.Update(It.Is<Comment>(c => c.CommentId == commentId)))
            .Callback((Comment c) => newCommentVotes = c.CommentVotes);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(u => u.CommentRepository)
            .Returns(commentRepositoryMock.Object);

            unitOfWorkMock.Setup(u => u.Save())
            .Callback(() => newCommentVotes.FirstOrDefault().CommentId = commentId);

            ICommentService target = new CommentService(unitOfWorkMock.Object);

            // Act
            target.AddVote(commentId, userId, voteIsPositive);

            // Assert
            Assert.AreEqual(1,
            newCommentVotes.Count(v => v.CommentId == commentId && v.UserId == userId && v.IsPositive == voteIsPositive));

            commentRepositoryMock.Verify(
            r => r.Get(It.IsAny<Expression<Func<Comment, bool>>>(), null, It.IsAny<Expression<Func<Comment, object>>[]>()),
            Times.Once);
            commentRepositoryMock.Verify(r => r.Update(It.Is<Comment>(c => c.CommentId == commentId)), Times.Once);

            unitOfWorkMock.Verify(r => r.Save(), Times.Once);
        }

        [Test]
        public void AddVote_UserHasVotedWithAnotherVoteType_EditsVote()
        {
            // Arrange
            int commentId = 1;
            int userId = 1;
            bool voteIsPositive = true;

            CommentVote[] commentVotes =
            {
                new CommentVote { CommentId = commentId, UserId = userId, IsPositive = !voteIsPositive },
                new CommentVote { CommentId = commentId, UserId = userId + 1 },
                new CommentVote { CommentId = commentId, UserId = userId + 2 }
            };

            Comment comment = new Comment { CommentId = commentId, CommentVotes = commentVotes.ToList() };

            // Arrange - mock commentRepository
            Mock<ICommentRepository> commentRepositoryMock = new Mock<ICommentRepository>();
            commentRepositoryMock.Setup(
            r => r.Get(It.IsAny<Expression<Func<Comment, bool>>>(), null, It.IsAny<Expression<Func<Comment, object>>[]>()))
            .Returns(new[] { comment });

            IEnumerable<CommentVote> newCommentVotes = null;

            commentRepositoryMock.Setup(r => r.Update(It.Is<Comment>(c => c.CommentId == commentId)))
            .Callback((Comment c) => newCommentVotes = c.CommentVotes);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(u => u.CommentRepository)
            .Returns(commentRepositoryMock.Object);

            ICommentService target = new CommentService(unitOfWorkMock.Object);

            // Act
            target.AddVote(commentId, userId, voteIsPositive);

            // Assert
            Assert.AreEqual(1,
            newCommentVotes.Count(v => v.CommentId == commentId && v.UserId == userId && v.IsPositive == voteIsPositive));
            Assert.AreEqual(commentVotes.Count(), newCommentVotes.Count());

            commentRepositoryMock.Verify(
            r => r.Get(It.IsAny<Expression<Func<Comment, bool>>>(), null, It.IsAny<Expression<Func<Comment, object>>[]>()),
            Times.Once);
            commentRepositoryMock.Verify(r => r.Update(It.Is<Comment>(c => c.CommentId == commentId)), Times.Once);

            unitOfWorkMock.Verify(r => r.Save(), Times.Once);
        }

        [Test]
        public void AddVote_UserHasVotedWithTheSameVoteType_DoNotUpdateTheVote()
        {
            // Arrange
            int commentId = 1;
            int userId = 1;
            bool voteIsPositive = true;

            Comment comment = new Comment
            {
                CommentId = commentId,
                CommentVotes = { new CommentVote { CommentId = commentId, UserId = userId, IsPositive = voteIsPositive } }
            };

            // Arrange - mock commentRepository
            Mock<ICommentRepository> commentRepositoryMock = new Mock<ICommentRepository>();
            commentRepositoryMock.Setup(
            r => r.Get(It.IsAny<Expression<Func<Comment, bool>>>(), null, It.IsAny<Expression<Func<Comment, object>>[]>()))
            .Returns(new[] { comment });

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(u => u.CommentRepository)
            .Returns(commentRepositoryMock.Object);

            ICommentService target = new CommentService(unitOfWorkMock.Object);

            // Act
            target.AddVote(commentId, userId, voteIsPositive);

            // Assert
            commentRepositoryMock.Verify(
            r => r.Get(It.IsAny<Expression<Func<Comment, bool>>>(), null, It.IsAny<Expression<Func<Comment, object>>[]>()),
            Times.Once);
            commentRepositoryMock.Verify(r => r.Update(It.Is<Comment>(c => c.CommentId == commentId)), Times.Never);

            unitOfWorkMock.Verify(r => r.Save(), Times.Never);
        }

        [Test]
        public void AddVote_UserIdIsLessOrEqualToZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            int commentId = 1;
            bool voteIsPositive = true;

            ICommentService target = new CommentService(new Mock<IUnitOfWork>().Object);

            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => target.AddVote(commentId, -1, voteIsPositive));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.AddVote(commentId, 0, voteIsPositive));
        }

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

        [Test]
        public void DeleteVote_AllCredentialsAreValid_DeletesCommentVote()
        {
            // Arrange
            int commentId = 1;
            int userId = 2;

            CommentVote[] commentVotes =
            {
                new CommentVote { CommentId = commentId, UserId = userId },
                new CommentVote { CommentId = commentId, UserId = userId + 1 },
                new CommentVote { CommentId = commentId, UserId = userId + 2 }
            };

            Comment comment = new Comment { CommentId = commentId, CommentVotes = commentVotes.ToList() };

            // Arrange - mock commentRepository
            Mock<ICommentRepository> commentRepositoryMock = new Mock<ICommentRepository>();
            commentRepositoryMock.Setup(
            r => r.Get(It.IsAny<Expression<Func<Comment, bool>>>(), null, It.IsAny<Expression<Func<Comment, object>>[]>()))
            .Returns(new[] { comment });

            IEnumerable<CommentVote> newCommentVotes = null;

            commentRepositoryMock.Setup(r => r.Update(It.Is<Comment>(c => c.CommentId == comment.CommentId)))
            .Callback((Comment c) => newCommentVotes = c.CommentVotes);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(u => u.CommentRepository)
            .Returns(commentRepositoryMock.Object);

            ICommentService target = new CommentService(unitOfWorkMock.Object);

            // Act
            target.DeleteVote(comment.CommentId, userId);

            // Assert
            Assert.AreEqual(0, newCommentVotes.Count(v => v.CommentId == commentId && v.UserId == userId));
            Assert.AreEqual(commentVotes.Count() - 1, newCommentVotes.Count());

            commentRepositoryMock.Verify(
            r => r.Get(It.IsAny<Expression<Func<Comment, bool>>>(), null, It.IsAny<Expression<Func<Comment, object>>[]>()),
            Times.Once);
            commentRepositoryMock.Verify(r => r.Update(It.Is<Comment>(c => c.CommentId == comment.CommentId)), Times.Once);

            unitOfWorkMock.Verify(r => r.Save(), Times.Once);
        }

        [Test]
        public void DeleteVote_CommentIdIsLessOrEqualToZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            int userId = 1;

            ICommentService target = new CommentService(new Mock<IUnitOfWork>().Object);

            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => target.DeleteVote(-1, userId));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.DeleteVote(0, userId));
        }

        [Test]
        public void DeleteVote_NonexistentCommentId_ThrowsCommentNotFoundException()
        {
            // Arrange
            int commentId = 1;
            int userId = 1;

            // Arrange - mock commentRepository
            Mock<ICommentRepository> commentRepositoryMock = new Mock<ICommentRepository>();
            commentRepositoryMock.Setup(
            r => r.Get(It.IsAny<Expression<Func<Comment, bool>>>(), null, It.IsAny<Expression<Func<Comment, object>>[]>()))
            .Returns(new Comment[] { });

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(u => u.CommentRepository)
            .Returns(commentRepositoryMock.Object);

            ICommentService target = new CommentService(unitOfWorkMock.Object);

            // Act and Assert
            Assert.Throws<CommentNotFoundException>(() => target.DeleteVote(commentId, userId));

            commentRepositoryMock.Verify(
            r => r.Get(It.IsAny<Expression<Func<Comment, bool>>>(), null, It.IsAny<Expression<Func<Comment, object>>[]>()),
            Times.Once);
        }

        [Test]
        public void DeleteVote_NonexistentCommentVote_ThrowsCommentVoteNotFoundException()
        {
            // Arrange
            Comment comment = new Comment { CommentId = 1, CommentVotes = new CommentVote[] { } };

            int userId = 1;

            // Arrange - mock commentRepository
            Mock<ICommentRepository> commentRepositoryMock = new Mock<ICommentRepository>();
            commentRepositoryMock.Setup(
            r => r.Get(It.IsAny<Expression<Func<Comment, bool>>>(), null, It.IsAny<Expression<Func<Comment, object>>[]>()))
            .Returns(new[] { comment });

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(u => u.CommentRepository)
            .Returns(commentRepositoryMock.Object);

            ICommentService target = new CommentService(unitOfWorkMock.Object);

            // Act and Assert
            Assert.Throws<CommentVoteNotFoundException>(() => target.DeleteVote(comment.CommentId, userId));

            commentRepositoryMock.Verify(
            r => r.Get(It.IsAny<Expression<Func<Comment, bool>>>(), null, It.IsAny<Expression<Func<Comment, object>>[]>()),
            Times.Once);
            commentRepositoryMock.Verify(r => r.Update(It.Is<Comment>(c => c.CommentId == comment.CommentId)), Times.Never);

            unitOfWorkMock.Verify(r => r.Save(), Times.Never);
        }

        [Test]
        public void DeleteVote_UserIdIsLessOrEqualToZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            int commentId = 1;

            ICommentService target = new CommentService(new Mock<IUnitOfWork>().Object);

            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => target.DeleteVote(commentId, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.DeleteVote(commentId, 0));
        }
    }
}