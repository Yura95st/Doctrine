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
    using Doctrine.Domain.Validation.Abstract;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class CommentServiceTests
    {
        private Mock<ICommentValidation> _commentValidationMock;

        [Test]
        public void AddVote_CommentIdIsLessOrEqualToZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            int userId = 1;
            bool voteIsPositive = true;

            ICommentService target = new CommentService(new Mock<IUnitOfWork>().Object, this._commentValidationMock.Object);

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

            // Arrange - create target
            ICommentService target = new CommentService(unitOfWorkMock.Object, this._commentValidationMock.Object);

            // Act and Assert
            Assert.Throws<CommentNotFoundException>(() => target.AddVote(commentId, userId, voteIsPositive));

            commentRepositoryMock.Verify(
            r => r.Get(It.IsAny<Expression<Func<Comment, bool>>>(), null, It.IsAny<Expression<Func<Comment, object>>[]>()),
            Times.Once);

            commentRepositoryMock.Verify(r => r.Update(It.Is<Comment>(c => c.CommentId == commentId)), Times.Never);

            unitOfWorkMock.Verify(r => r.Save(), Times.Never);
        }

        [Test]
        public void AddVote_NonexistentUserId_ThrowsUserNotFoundException()
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

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(r => r.GetById(userId))
            .Returns((User)null);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.CommentRepository)
            .Returns(commentRepositoryMock.Object);

            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            // Arrange - create target
            ICommentService target = new CommentService(unitOfWorkMock.Object, this._commentValidationMock.Object);

            // Act and Assert
            Assert.Throws<UserNotFoundException>(() => target.AddVote(commentId, userId, voteIsPositive));

            commentRepositoryMock.Verify(
            r => r.Get(It.IsAny<Expression<Func<Comment, bool>>>(), null, It.IsAny<Expression<Func<Comment, object>>[]>()),
            Times.Once);
            commentRepositoryMock.Verify(r => r.Update(It.Is<Comment>(c => c.CommentId == commentId)), Times.Never);

            userRepositoryMock.Verify(r => r.GetById(userId), Times.Once());

            unitOfWorkMock.Verify(r => r.Save(), Times.Never);
        }

        [Test]
        public void AddVote_UserHasNotVotedYet_AddsVote()
        {
            // Arrange
            int commentId = 1;
            int userId = 1;
            bool voteIsPositive = true;
            CommentVote[] commentVotes =
            {
                new CommentVote { CommentId = commentId, UserId = userId + 1 },
                new CommentVote { CommentId = commentId, UserId = userId + 2 }
            };

            User user = new User { UserId = userId };
            Comment comment = new Comment { CommentId = commentId, CommentVotes = commentVotes.ToList() };

            // Arrange - mock commentRepository
            Mock<ICommentRepository> commentRepositoryMock = new Mock<ICommentRepository>();

            commentRepositoryMock.Setup(
            r => r.Get(It.IsAny<Expression<Func<Comment, bool>>>(), null, It.IsAny<Expression<Func<Comment, object>>[]>()))
            .Returns(new[] { comment });

            IEnumerable<CommentVote> newCommentVotes = null;

            commentRepositoryMock.Setup(r => r.Update(It.Is<Comment>(c => c.CommentId == commentId)))
            .Callback((Comment c) => newCommentVotes = c.CommentVotes);

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(r => r.GetById(userId))
            .Returns(user);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.CommentRepository)
            .Returns(commentRepositoryMock.Object);

            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            // Arrange - create target
            ICommentService target = new CommentService(unitOfWorkMock.Object, this._commentValidationMock.Object);

            // Act
            target.AddVote(commentId, userId, voteIsPositive);

            // Assert
            Assert.IsNotNull(newCommentVotes);
            Assert.AreEqual(commentVotes.Count() + 1, newCommentVotes.Count());

            CommentVote commentVote =
            newCommentVotes.FirstOrDefault(v => v.UserId == userId && v.IsPositive == voteIsPositive);

            Assert.IsNotNull(commentVote);

            commentRepositoryMock.Verify(
            r => r.Get(It.IsAny<Expression<Func<Comment, bool>>>(), null, It.IsAny<Expression<Func<Comment, object>>[]>()),
            Times.Once);
            commentRepositoryMock.Verify(r => r.Update(It.Is<Comment>(c => c.CommentId == commentId)), Times.Once);

            userRepositoryMock.Verify(r => r.GetById(userId), Times.Once());

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

            // Arrange - create target
            ICommentService target = new CommentService(unitOfWorkMock.Object, this._commentValidationMock.Object);

            // Act
            target.AddVote(commentId, userId, voteIsPositive);

            // Assert
            Assert.IsNotNull(newCommentVotes);
            Assert.AreEqual(commentVotes.Count(), newCommentVotes.Count());

            CommentVote commentVote =
            newCommentVotes.FirstOrDefault(
            v => v.CommentId == commentId && v.UserId == userId && v.IsPositive == voteIsPositive);

            Assert.IsNotNull(commentVote);

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

            // Arrange - create target
            ICommentService target = new CommentService(unitOfWorkMock.Object, this._commentValidationMock.Object);

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

            ICommentService target = new CommentService(new Mock<IUnitOfWork>().Object, this._commentValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => target.AddVote(commentId, -1, voteIsPositive));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.AddVote(commentId, 0, voteIsPositive));
        }

        [Test]
        public void CanEdit_CommentIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            int userId = 1;

            ICommentService target = new CommentService(new Mock<IUnitOfWork>().Object, this._commentValidationMock.Object);

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

            // Arrange - create target
            ICommentService target = new CommentService(new Mock<IUnitOfWork>().Object, this._commentValidationMock.Object,
            permittedPeriodForEditing);

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

            ICommentService target = new CommentService(new Mock<IUnitOfWork>().Object, this._commentValidationMock.Object,
            permittedPeriodForEditing);

            // Act
            bool result = target.CanEdit(comment.UserId, comment);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void CanEdit_UserIdIsLessOrEqualToZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            Comment comment = new Comment { CommentId = 1 };

            ICommentService target = new CommentService(new Mock<IUnitOfWork>().Object, this._commentValidationMock.Object);

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

            ICommentService target = new CommentService(new Mock<IUnitOfWork>().Object, this._commentValidationMock.Object);

            // Act
            bool result = target.CanEdit(userId, comment);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Create_AllCredentialsAreValid_CreatesAndReturnsComment()
        {
            // Arrange
            User user = new User { UserId = 1 };
            Article article = new Article { ArticleId = 2 };
            string commentText = "comment_text";
            string validatedCommentText = "validated_comment_text";

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(r => r.GetById(user.UserId))
            .Returns(user);

            // Arrange - mock articleRepository
            Mock<IArticleRepository> articleRepositoryMock = new Mock<IArticleRepository>();

            articleRepositoryMock.Setup(r => r.GetById(article.ArticleId))
            .Returns(article);

            // Arrange - mock commentRepository
            Mock<ICommentRepository> commentRepositoryMock = new Mock<ICommentRepository>();

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            unitOfWorkMock.SetupGet(u => u.ArticleRepository)
            .Returns(articleRepositoryMock.Object);

            unitOfWorkMock.SetupGet(u => u.CommentRepository)
            .Returns(commentRepositoryMock.Object);

            // Arrange - mock commentValidation
            this._commentValidationMock.Setup(v => v.ValidateCommentText(commentText))
            .Returns(validatedCommentText);

            // Arrange - create target
            ICommentService target = new CommentService(unitOfWorkMock.Object, this._commentValidationMock.Object);

            // Act
            Comment comment = target.Create(user.UserId, article.ArticleId, commentText);

            // Assert
            Assert.IsNotNull(comment);
            Assert.AreEqual(user.UserId, comment.UserId);
            Assert.AreEqual(article.ArticleId, comment.ArticleId);
            Assert.AreEqual(validatedCommentText, comment.Text);
            Assert.IsTrue(new DateTime() != comment.Date);

            userRepositoryMock.Verify(r => r.GetById(user.UserId), Times.Once);

            articleRepositoryMock.Verify(r => r.GetById(article.ArticleId), Times.Once);

            commentRepositoryMock.Verify(
            r =>
            r.Insert(
            It.Is<Comment>(c => c.UserId == user.UserId && c.ArticleId == article.ArticleId && c.Text == validatedCommentText)),
            Times.Once);

            unitOfWorkMock.Verify(u => u.Save(), Times.Once);

            this._commentValidationMock.Verify(v => v.ValidateCommentText(commentText), Times.Once);
        }

        [Test]
        public void Create_ArticleIdIsLessOrEqualToZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            int userId = 1;
            string commentText = "comment_text";

            ICommentService target = new CommentService(new Mock<IUnitOfWork>().Object, this._commentValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Create(userId, -1, commentText));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Create(userId, 0, commentText));
        }

        [Test]
        public void Create_CommentTextIsEmpty_ThrowsArgumentException()
        {
            // Arrange
            int userId = 1;
            int articleId = 2;

            ICommentService target = new CommentService(new Mock<IUnitOfWork>().Object, this._commentValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentException>(() => target.Create(userId, articleId, ""));
        }

        [Test]
        public void Create_CommentTextIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            int userId = 1;
            int articleId = 2;

            ICommentService target = new CommentService(new Mock<IUnitOfWork>().Object, this._commentValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.Create(userId, articleId, null));
        }

        [Test]
        public void Create_NonexistentArticleId_ThrowsArticleNotFoundException()
        {
            // Arrange
            User user = new User { UserId = 1 };
            int articleId = 2;
            string commentText = "comment_text";

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(r => r.GetById(user.UserId))
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
            ICommentService target = new CommentService(unitOfWorkMock.Object, this._commentValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArticleNotFoundException>(() => target.Create(user.UserId, articleId, commentText));

            userRepositoryMock.Verify(r => r.GetById(user.UserId), Times.Once);

            articleRepositoryMock.Verify(r => r.GetById(articleId), Times.Once);
        }

        [Test]
        public void Create_NonexistentUserId_ThrowsUserNotFoundException()
        {
            // Arrange
            int userId = 1;
            int articleId = 2;
            string commentText = "comment_text";

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(r => r.GetById(userId))
            .Returns((User)null);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            // Arrange - create target
            ICommentService target = new CommentService(unitOfWorkMock.Object, this._commentValidationMock.Object);

            // Act and Assert
            Assert.Throws<UserNotFoundException>(() => target.Create(userId, articleId, commentText));

            userRepositoryMock.Verify(r => r.GetById(userId), Times.Once);
        }

        [Test]
        public void Create_UserIdIsLessOrEqualToZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            int articleId = 1;
            string commentText = "comment_text";

            ICommentService target = new CommentService(new Mock<IUnitOfWork>().Object, this._commentValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Create(-1, articleId, commentText));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Create(0, articleId, commentText));
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

            // Arrange - create target
            ICommentService target = new CommentService(unitOfWorkMock.Object, this._commentValidationMock.Object);

            // Act
            target.DeleteVote(comment.CommentId, userId);

            // Assert
            Assert.AreEqual(commentVotes.Count() - 1, newCommentVotes.Count());
            Assert.AreEqual(0, newCommentVotes.Count(v => v.CommentId == commentId && v.UserId == userId));

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

            ICommentService target = new CommentService(new Mock<IUnitOfWork>().Object, this._commentValidationMock.Object);

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

            // Arrange - create target
            ICommentService target = new CommentService(unitOfWorkMock.Object, this._commentValidationMock.Object);

            // Act and Assert
            Assert.Throws<CommentNotFoundException>(() => target.DeleteVote(commentId, userId));

            commentRepositoryMock.Verify(
            r => r.Get(It.IsAny<Expression<Func<Comment, bool>>>(), null, It.IsAny<Expression<Func<Comment, object>>[]>()),
            Times.Once);
            commentRepositoryMock.Verify(r => r.Update(It.Is<Comment>(c => c.CommentId == commentId)), Times.Never);

            unitOfWorkMock.Verify(r => r.Save(), Times.Never);
        }

        [Test]
        public void DeleteVote_NonexistentCommentVote_ThrowsCommentVoteNotFoundException()
        {
            // Arrange
            int userId = 1;
            int commentId = 2;

            Comment comment = new Comment
            {
                CommentId = commentId,
                CommentVotes =
                {
                    new CommentVote { CommentId = commentId, UserId = userId + 1 },
                    new CommentVote { CommentId = commentId, UserId = userId + 2 }
                }
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

            // Arrange - create target
            ICommentService target = new CommentService(unitOfWorkMock.Object, this._commentValidationMock.Object);

            // Act and Assert
            Assert.Throws<CommentVoteNotFoundException>(() => target.DeleteVote(commentId, userId));

            commentRepositoryMock.Verify(
            r => r.Get(It.IsAny<Expression<Func<Comment, bool>>>(), null, It.IsAny<Expression<Func<Comment, object>>[]>()),
            Times.Once);
            commentRepositoryMock.Verify(r => r.Update(It.Is<Comment>(c => c.CommentId == commentId)), Times.Never);

            unitOfWorkMock.Verify(r => r.Save(), Times.Never);
        }

        [Test]
        public void DeleteVote_UserIdIsLessOrEqualToZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            int commentId = 1;

            ICommentService target = new CommentService(new Mock<IUnitOfWork>().Object, this._commentValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => target.DeleteVote(commentId, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.DeleteVote(commentId, 0));
        }

        [SetUp]
        public void Init()
        {
            this.MockCommentValidation();
        }

        private void MockCommentValidation()
        {
            this._commentValidationMock = new Mock<ICommentValidation>();

            this._commentValidationMock.Setup(v => v.ValidateCommentText(It.IsAny<string>()))
            .Returns((string commentText) => commentText);
        }
    }
}