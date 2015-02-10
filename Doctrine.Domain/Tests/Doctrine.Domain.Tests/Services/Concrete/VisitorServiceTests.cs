namespace Doctrine.Domain.Tests.Services.Concrete
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Doctrine.Domain.Dal;
    using Doctrine.Domain.Dal.Repositories.Abstract;
    using Doctrine.Domain.Exceptions.InvalidFormat;
    using Doctrine.Domain.Exceptions.NotFound;
    using Doctrine.Domain.Models;
    using Doctrine.Domain.Services.Abstract;
    using Doctrine.Domain.Services.Concrete;
    using Doctrine.Domain.Validation.Abstract;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class VisitorServiceTests
    {
        private Mock<IVisitorValidation> _visitorValidationMock;

        [SetUp]
        public void Init()
        {
            this.MockVisitorValidation();

            //mockRepository.Setup(r => r.Get(It.IsAny<Expression<Func<Visitor, bool>>>(), null, ""))
            //.Returns(
            //(Expression<Func<Visitor, bool>> predicate, Func<IQueryable<Visitor>, IOrderedQueryable<Visitor>> orderBy,
            // string includeProperties) => new[] { visitor }.Where(predicate.Compile()));
        }

        [Test]
        public void RegisterIpAddress_IpAddressFormatIsInvalid_ThrowsInvalidIpAddressFormatException()
        {
            // Arrange
            string ipAddress = "127.0.0.1";

            this._visitorValidationMock.Setup(v => v.IsValidIpAddress(ipAddress))
            .Returns(false);

            IVisitorService target = new VisitorService(new Mock<IUnitOfWork>().Object, this._visitorValidationMock.Object);

            // Act and Assert
            Assert.Throws<InvalidIpAddressFormatException>(() => target.RegisterIpAddress(ipAddress));
        }

        [Test]
        public void RegisterIpAddress_IpAddressIsAlreadyRegistered_GetsVisitorByIpAddress()
        {
            // Arrange
            Visitor visitor = new Visitor { VisitorId = 1, IpAddress = "127.0.0.1" };

            // Arrange - mock visitorRepository
            Mock<IVisitorRepository> visitorRepositoryMock = new Mock<IVisitorRepository>();
            visitorRepositoryMock.Setup(r => r.GetByIpAddress(visitor.IpAddress))
            .Returns(visitor);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(unitOfWork => unitOfWork.VisitorRepository)
            .Returns(visitorRepositoryMock.Object);

            IVisitorService target = new VisitorService(unitOfWorkMock.Object, this._visitorValidationMock.Object);

            // Act
            Visitor registeredVisitor = target.RegisterIpAddress(visitor.IpAddress);

            // Assert
            Assert.AreSame(visitor, registeredVisitor);

            visitorRepositoryMock.Verify(r => r.GetByIpAddress(visitor.IpAddress), Times.Once);
            visitorRepositoryMock.Verify(r => r.Insert(It.Is<Visitor>(v => v.VisitorId == visitor.VisitorId)), Times.Never);

            unitOfWorkMock.Verify(r => r.Save(), Times.Never);
        }

        [Test]
        public void RegisterIpAddress_IpAddressIsNotRegistered_RegistersIpAddressAndGetsVisitorByIpAddress()
        {
            // Arrange
            string ipAddress = "127.0.0.1";
            int newVisitorId = 1;

            // Arrange - mock visitorRepository
            Mock<IVisitorRepository> visitorRepositoryMock = new Mock<IVisitorRepository>();
            visitorRepositoryMock.Setup(r => r.GetByIpAddress(ipAddress))
            .Returns((Visitor)null);

            Visitor newVisitor = null;

            visitorRepositoryMock.Setup(r => r.Insert(It.Is<Visitor>(v => v.IpAddress == ipAddress)))
            .Callback((Visitor v) => newVisitor = v);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(u => u.VisitorRepository)
            .Returns(visitorRepositoryMock.Object);

            unitOfWorkMock.Setup(u => u.Save())
            .Callback(() => newVisitor.VisitorId = newVisitorId);

            IVisitorService target = new VisitorService(unitOfWorkMock.Object, this._visitorValidationMock.Object);

            // Act
            Visitor registeredVisitor = target.RegisterIpAddress(ipAddress);

            // Assert
            Assert.IsNotNull(registeredVisitor);
            Assert.AreEqual(ipAddress, registeredVisitor.IpAddress);
            Assert.AreEqual(newVisitorId, registeredVisitor.VisitorId);

            visitorRepositoryMock.Verify(r => r.GetByIpAddress(ipAddress), Times.Once);
            visitorRepositoryMock.Verify(r => r.Insert(It.Is<Visitor>(v => v.IpAddress == ipAddress)), Times.Once);

            unitOfWorkMock.Verify(r => r.Save(), Times.Once);
        }

        [Test]
        public void RegisterIpAddress_IpAddressIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IVisitorService target = new VisitorService(new Mock<IUnitOfWork>().Object, this._visitorValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.RegisterIpAddress(null));
        }

        [Test]
        public void ViewArticle_AllCredentialsAreValid_MarksArticleAsViewedByVisitor()
        {
            // Arrange
            Visitor visitor = new Visitor { VisitorId = 1 };

            Article article = new Article { ArticleId = 2 };

            // Arrange - mock visitorRepository
            Mock<IVisitorRepository> visitorRepositoryMock = new Mock<IVisitorRepository>();

            visitorRepositoryMock.Setup(
            r => r.Get(It.IsAny<Expression<Func<Visitor, bool>>>(), null, It.IsAny<Expression<Func<Visitor, object>>[]>()))
            .Returns(new[] { visitor });

            ArticleVisitor newArticleVisitor = null;
            visitorRepositoryMock.Setup(r => r.Update(It.Is<Visitor>(v => v.VisitorId == visitor.VisitorId)))
            .Callback((Visitor v) => newArticleVisitor = v.ArticleVisitors.FirstOrDefault());

            // Arrange - mock articleRepository
            Mock<IArticleRepository> articleRepositoryMock = new Mock<IArticleRepository>();
            articleRepositoryMock.Setup(r => r.GetById(article.ArticleId))
            .Returns(article);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(u => u.VisitorRepository)
            .Returns(visitorRepositoryMock.Object);

            unitOfWorkMock.SetupGet(u => u.ArticleRepository)
            .Returns(articleRepositoryMock.Object);

            unitOfWorkMock.Setup(u => u.Save())
            .Callback(() => newArticleVisitor.VisitorId = visitor.VisitorId);

            IVisitorService target = new VisitorService(unitOfWorkMock.Object, this._visitorValidationMock.Object);

            // Act
            target.ViewArticle(visitor.VisitorId, article.ArticleId);

            // Assert
            Assert.AreEqual(visitor.VisitorId, newArticleVisitor.VisitorId);
            Assert.AreEqual(article.ArticleId, newArticleVisitor.ArticleId);
            Assert.IsTrue(new DateTime() != newArticleVisitor.LastViewDate);

            visitorRepositoryMock.Verify(
            r => r.Get(It.IsAny<Expression<Func<Visitor, bool>>>(), null, It.IsAny<Expression<Func<Visitor, object>>[]>()),
            Times.Once);
            visitorRepositoryMock.Verify(r => r.Update(It.Is<Visitor>(v => v.VisitorId == visitor.VisitorId)), Times.Once);

            articleRepositoryMock.Verify(r => r.GetById(article.ArticleId), Times.Once);

            unitOfWorkMock.Verify(u => u.Save(), Times.Once);
        }

        [Test]
        public void ViewArticle_ArticleAlreadyViewed_UpdatesLastViewDate()
        {
            // Arrange
            int visitorId = 1;
            int articleId = 2;
            DateTime lastViewDate = new DateTime();

            ArticleVisitor articleVisitor = new ArticleVisitor
            { ArticleId = articleId, VisitorId = visitorId, LastViewDate = lastViewDate };

            Visitor visitor = new Visitor
            { VisitorId = visitorId, ArticleVisitors = new List<ArticleVisitor> { articleVisitor } };

            // Arrange - mock visitorRepository
            Mock<IVisitorRepository> visitorRepositoryMock = new Mock<IVisitorRepository>();

            visitorRepositoryMock.Setup(
            r => r.Get(It.IsAny<Expression<Func<Visitor, bool>>>(), null, It.IsAny<Expression<Func<Visitor, object>>[]>()))
            .Returns(new[] { visitor });

            ArticleVisitor newArticleVisitor = null;
            visitorRepositoryMock.Setup(r => r.Update(It.Is<Visitor>(v => v.VisitorId == visitorId)))
            .Callback((Visitor v) => newArticleVisitor = v.ArticleVisitors.FirstOrDefault());

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(u => u.VisitorRepository)
            .Returns(visitorRepositoryMock.Object);

            IVisitorService target = new VisitorService(unitOfWorkMock.Object, this._visitorValidationMock.Object);

            // Act
            target.ViewArticle(visitorId, articleId);

            // Assert
            Assert.AreEqual(visitorId, newArticleVisitor.VisitorId);
            Assert.AreEqual(articleId, newArticleVisitor.ArticleId);
            Assert.IsTrue(lastViewDate != newArticleVisitor.LastViewDate);

            visitorRepositoryMock.Verify(
            r => r.Get(It.IsAny<Expression<Func<Visitor, bool>>>(), null, It.IsAny<Expression<Func<Visitor, object>>[]>()),
            Times.Once);
            visitorRepositoryMock.Verify(r => r.Update(It.Is<Visitor>(v => v.VisitorId == visitor.VisitorId)), Times.Once);

            unitOfWorkMock.Verify(u => u.Save(), Times.Once);
        }

        [Test]
        public void ViewArticle_ArticleIdIsLessOrEqualToZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            int visitorId = 1;

            IVisitorService target = new VisitorService(new Mock<IUnitOfWork>().Object, this._visitorValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => target.ViewArticle(visitorId, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.ViewArticle(visitorId, 0));
        }

        [Test]
        public void ViewArticle_NonexistentArticleId_ThrowsArticleNotFoundException()
        {
            // Arrange
            Visitor visitor = new Visitor { VisitorId = 1 };

            int articleId = 2;

            // Arrange - mock visitorRepository
            Mock<IVisitorRepository> visitorRepositoryMock = new Mock<IVisitorRepository>();

            visitorRepositoryMock.Setup(
            r => r.Get(It.IsAny<Expression<Func<Visitor, bool>>>(), null, It.IsAny<Expression<Func<Visitor, object>>[]>()))
            .Returns(new[] { visitor });

            // Arrange - mock articleRepository
            Mock<IArticleRepository> articleRepositoryMock = new Mock<IArticleRepository>();
            articleRepositoryMock.Setup(r => r.GetById(articleId))
            .Returns((Article)null);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(u => u.VisitorRepository)
            .Returns(visitorRepositoryMock.Object);

            unitOfWorkMock.SetupGet(u => u.ArticleRepository)
            .Returns(articleRepositoryMock.Object);

            IVisitorService target = new VisitorService(unitOfWorkMock.Object, this._visitorValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArticleNotFoundException>(() => target.ViewArticle(visitor.VisitorId, articleId));

            visitorRepositoryMock.Verify(
            r => r.Get(It.IsAny<Expression<Func<Visitor, bool>>>(), null, It.IsAny<Expression<Func<Visitor, object>>[]>()),
            Times.Once);
            visitorRepositoryMock.Verify(r => r.Update(It.Is<Visitor>(v => v.VisitorId == visitor.VisitorId)), Times.Never);

            articleRepositoryMock.Verify(r => r.GetById(articleId), Times.Once);

            unitOfWorkMock.Verify(u => u.Save(), Times.Never);
        }

        [Test]
        public void ViewArticle_NonexistentVisitorId_ThrowsVisitorNotFoundException()
        {
            // Arrange
            int visitorId = 1;
            int articleId = 2;

            // Arrange - mock visitorRepository
            Mock<IVisitorRepository> visitorRepositoryMock = new Mock<IVisitorRepository>();
            visitorRepositoryMock.Setup(
            r => r.Get(It.IsAny<Expression<Func<Visitor, bool>>>(), null, It.IsAny<Expression<Func<Visitor, object>>[]>()))
            .Returns(new Visitor[] { });

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(u => u.VisitorRepository)
            .Returns(visitorRepositoryMock.Object);

            IVisitorService target = new VisitorService(unitOfWorkMock.Object, this._visitorValidationMock.Object);

            // Act and Assert
            Assert.Throws<VisitorNotFoundException>(() => target.ViewArticle(visitorId, articleId));

            visitorRepositoryMock.Verify(
            r => r.Get(It.IsAny<Expression<Func<Visitor, bool>>>(), null, It.IsAny<Expression<Func<Visitor, object>>[]>()),
            Times.Once);
            visitorRepositoryMock.Verify(r => r.Update(It.Is<Visitor>(v => v.VisitorId == visitorId)), Times.Never);

            unitOfWorkMock.Verify(u => u.Save(), Times.Never);
        }

        [Test]
        public void ViewArticle_VisitorIdIsLessOrEqualToZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            int articleId = 1;

            IVisitorService target = new VisitorService(new Mock<IUnitOfWork>().Object, this._visitorValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => target.ViewArticle(-1, articleId));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.ViewArticle(0, articleId));
        }

        private void MockVisitorValidation()
        {
            this._visitorValidationMock = new Mock<IVisitorValidation>();

            this._visitorValidationMock.Setup(v => v.IsValidIpAddress(It.IsAny<string>()))
            .Returns(true);
        }
    }
}