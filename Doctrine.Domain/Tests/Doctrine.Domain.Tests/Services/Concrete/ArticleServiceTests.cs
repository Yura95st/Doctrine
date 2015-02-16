namespace Doctrine.Domain.Tests.Services.Concrete
{
    using System;

    using Doctrine.Domain.Dal;
    using Doctrine.Domain.Dal.Repositories.Abstract;
    using Doctrine.Domain.Models;
    using Doctrine.Domain.Services.Abstract;
    using Doctrine.Domain.Services.Concrete;
    using Doctrine.Domain.Validation.Abstract;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class ArticleServiceTests
    {
        private Mock<IArticleValidation> _articleValidationMock;

        [Test]
        public void GetById_ArticleIdIsLessOrEqualToZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            IArticleService target = new ArticleService(new Mock<IUnitOfWork>().Object, this._articleValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => target.GetById(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.GetById(0));
        }

        [Test]
        public void GetById_ArticleIdIsValid_ReturnsArticle()
        {
            // Arrange
            Article testArticle = new Article { ArticleId = 1 };

            // Arrange - mock articleRepository
            Mock<IArticleRepository> articleRepositoryMock = new Mock<IArticleRepository>();

            articleRepositoryMock.Setup(r => r.GetById(testArticle.ArticleId))
            .Returns(testArticle);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.ArticleRepository)
            .Returns(articleRepositoryMock.Object);

            // Arrange - create target
            IArticleService target = new ArticleService(unitOfWorkMock.Object, this._articleValidationMock.Object);

            // Act
            Article article = target.GetById(testArticle.ArticleId);

            // Assert
            Assert.AreSame(testArticle, article);

            articleRepositoryMock.Verify(r => r.GetById(testArticle.ArticleId), Times.Once);
        }

        [Test]
        public void GetById_NonexistentArticleId_ReturnsNull()
        {
            // Arrange
            int articleId = 1;

            // Arrange - mock articleRepository
            Mock<IArticleRepository> articleRepositoryMock = new Mock<IArticleRepository>();

            articleRepositoryMock.Setup(r => r.GetById(articleId))
            .Returns((Article)null);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.ArticleRepository)
            .Returns(articleRepositoryMock.Object);

            // Arrange - create target
            IArticleService target = new ArticleService(unitOfWorkMock.Object, this._articleValidationMock.Object);

            // Act
            Article article = target.GetById(articleId);

            // Assert
            Assert.IsNull(article);

            articleRepositoryMock.Verify(r => r.GetById(articleId), Times.Once);
        }

        [SetUp]
        public void Init()
        {
            this.MockArticleValidation();
        }

        private void MockArticleValidation()
        {
            this._articleValidationMock = new Mock<IArticleValidation>();
        }
    }
}