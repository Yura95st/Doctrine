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
    using Doctrine.Domain.Tests.TestHelpers;
    using Doctrine.Domain.Validation.Abstract;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class ArticleServiceTests
    {
        private Mock<IArticleValidation> _articleValidationMock;

        [Test]
        public void Create_AllCredentialsAreValid_CreatesArticle()
        {
            // Arrange
            User user = new User { UserId = 1 };

            string articleTitle = "article_title";
            string articleText = "article_text";
            string validatedArticleTitle = "validated_article_title";
            string validatedArticleText = "validated_article_text";

            Topic topic = new Topic { TopicId = 2 };
            int[] tagIds = { 1, 2, 3, 4 };

            IList<Tag> tags = new List<Tag>();

            foreach (int tagId in tagIds)
            {
                tags.Add(new Tag { TagId = tagId });
            }

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(r => r.GetById(user.UserId))
            .Returns(user);

            // Arrange - mock topicRepository
            Mock<ITopicRepository> topicRepositoryMock = new Mock<ITopicRepository>();

            topicRepositoryMock.Setup(r => r.GetById(topic.TopicId))
            .Returns(topic);

            // Arrange - mock tagRepository
            Mock<ITagRepository> tagRepositoryMock = new Mock<ITagRepository>();

            tagRepositoryMock.Setup(
            r => r.Get(It.IsAny<Expression<Func<Tag, bool>>>(), null, It.Is<Expression<Func<Tag, object>>[]>(e => !e.Any())))
            .Returns(
            (Expression<Func<Tag, bool>> filter, Func<IQueryable<Tag>, IOrderedQueryable<Tag>> orderBy,
             Expression<Func<Tag, object>>[] selector) => CollectionHelper.FilterCollection(tags, filter, orderBy));

            // Arrange - mock articleRepository
            Mock<IArticleRepository> articleRepositoryMock = new Mock<IArticleRepository>();

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            unitOfWorkMock.SetupGet(u => u.TopicRepository)
            .Returns(topicRepositoryMock.Object);

            unitOfWorkMock.SetupGet(u => u.TagRepository)
            .Returns(tagRepositoryMock.Object);

            unitOfWorkMock.SetupGet(u => u.ArticleRepository)
            .Returns(articleRepositoryMock.Object);

            // Arrange - mock articleValidation
            this._articleValidationMock.Setup(v => v.ValidateTitle(articleTitle))
            .Returns(validatedArticleTitle);

            this._articleValidationMock.Setup(v => v.ValidateArticleText(articleText))
            .Returns(validatedArticleText);

            // Arrange - create target
            IArticleService target = new ArticleService(unitOfWorkMock.Object, this._articleValidationMock.Object);

            // Act
            Article article = target.Create(user.UserId, articleTitle, articleText, topic.TopicId, tagIds);

            // Assert
            Assert.IsNotNull(article);
            Assert.AreEqual(user.UserId, article.UserId);
            Assert.AreEqual(topic.TopicId, article.TopicId);
            Assert.AreEqual(validatedArticleTitle, article.Title);
            Assert.AreEqual(validatedArticleText, article.Text);
            Assert.AreEqual(tags, article.Tags);
            Assert.IsTrue(new DateTime() != article.PublicationDate);

            userRepositoryMock.Verify(r => r.GetById(user.UserId), Times.Once);

            topicRepositoryMock.Verify(r => r.GetById(topic.TopicId), Times.Once);

            tagRepositoryMock.Verify(
            r => r.Get(It.IsAny<Expression<Func<Tag, bool>>>(), null, It.Is<Expression<Func<Tag, object>>[]>(e => !e.Any())),
            Times.Once);

            articleRepositoryMock.Verify(
            r =>
            r.Insert(
            It.Is<Article>(a => a.UserId == user.UserId && a.Title == validatedArticleTitle && a.Text == validatedArticleText)),
            Times.Once);

            unitOfWorkMock.Verify(u => u.Save(), Times.Once);

            this._articleValidationMock.Verify(v => v.ValidateTitle(articleTitle), Times.Once);
            this._articleValidationMock.Verify(v => v.ValidateArticleText(articleText), Times.Once);
        }

        [Test]
        public void Create_NonexistentTopicId_ThrowsTopicNotFoundException()
        {
            // Arrange
            User user = new User { UserId = 1 };
            string articleTitle = "article_title";
            string articleText = "article_text";
            int topicId = 2;
            int[] tagIds = { 1, 2, 3, 4 };

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(r => r.GetById(user.UserId))
            .Returns(user);

            // Arrange - mock topicRepository
            Mock<ITopicRepository> topicRepositoryMock = new Mock<ITopicRepository>();

            topicRepositoryMock.Setup(r => r.GetById(topicId))
            .Returns((Topic)null);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            unitOfWorkMock.SetupGet(u => u.TopicRepository)
            .Returns(topicRepositoryMock.Object);

            // Arrange - create target
            IArticleService target = new ArticleService(unitOfWorkMock.Object, this._articleValidationMock.Object);

            // Act and Assert
            Assert.Throws<TopicNotFoundException>(
            () => target.Create(user.UserId, articleTitle, articleText, topicId, tagIds));

            userRepositoryMock.Verify(r => r.GetById(user.UserId), Times.Once);

            topicRepositoryMock.Verify(r => r.GetById(topicId), Times.Once);
        }

        [Test]
        public void Create_NonexistentUserId_ThrowsUserNotFoundException()
        {
            // Arrange
            int userId = 1;
            string articleTitle = "article_title";
            string articleText = "article_text";
            int topicId = 2;
            int[] tagIds = { 1, 2, 3, 4 };

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(r => r.GetById(userId))
            .Returns((User)null);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            // Arrange - create target
            IArticleService target = new ArticleService(unitOfWorkMock.Object, this._articleValidationMock.Object);

            // Act and Assert
            Assert.Throws<UserNotFoundException>(() => target.Create(userId, articleTitle, articleText, topicId, tagIds));

            userRepositoryMock.Verify(r => r.GetById(userId), Times.Once);
        }

        [Test]
        public void Create_OneOfTagIdsIsNonexistent_DoNotAddThisTagToArticle()
        {
            // Arrange
            User user = new User { UserId = 1 };

            string articleTitle = "article_title";
            string articleText = "article_text";
            string validatedArticleTitle = "validated_article_title";
            string validatedArticleText = "validated_article_text";

            Topic topic = new Topic { TopicId = 2 };

            int nonexistentTagId = 3;
            int[] tagIds = { 1, 2, nonexistentTagId, 4 };

            IList<Tag> tags = new List<Tag>();

            foreach (int tagId in tagIds)
            {
                if (tagId != nonexistentTagId)
                {
                    tags.Add(new Tag { TagId = tagId });
                }
            }

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(r => r.GetById(user.UserId))
            .Returns(user);

            // Arrange - mock topicRepository
            Mock<ITopicRepository> topicRepositoryMock = new Mock<ITopicRepository>();

            topicRepositoryMock.Setup(r => r.GetById(topic.TopicId))
            .Returns(topic);

            // Arrange - mock tagRepository
            Mock<ITagRepository> tagRepositoryMock = new Mock<ITagRepository>();

            tagRepositoryMock.Setup(
            r => r.Get(It.IsAny<Expression<Func<Tag, bool>>>(), null, It.Is<Expression<Func<Tag, object>>[]>(e => !e.Any())))
            .Returns(
            (Expression<Func<Tag, bool>> filter, Func<IQueryable<Tag>, IOrderedQueryable<Tag>> orderBy,
             Expression<Func<Tag, object>>[] selector) => CollectionHelper.FilterCollection(tags, filter, orderBy));

            // Arrange - mock articleRepository
            Mock<IArticleRepository> articleRepositoryMock = new Mock<IArticleRepository>();

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            unitOfWorkMock.SetupGet(u => u.TopicRepository)
            .Returns(topicRepositoryMock.Object);

            unitOfWorkMock.SetupGet(u => u.TagRepository)
            .Returns(tagRepositoryMock.Object);

            unitOfWorkMock.SetupGet(u => u.ArticleRepository)
            .Returns(articleRepositoryMock.Object);

            // Arrange - mock articleValidation
            this._articleValidationMock.Setup(v => v.ValidateTitle(articleTitle))
            .Returns(validatedArticleTitle);

            this._articleValidationMock.Setup(v => v.ValidateArticleText(articleText))
            .Returns(validatedArticleText);

            // Arrange - create target
            IArticleService target = new ArticleService(unitOfWorkMock.Object, this._articleValidationMock.Object);

            // Act
            Article article = target.Create(user.UserId, articleTitle, articleText, topic.TopicId, tagIds);

            // Assert
            Assert.IsNotNull(article);
            Assert.AreEqual(user.UserId, article.UserId);
            Assert.AreEqual(topic.TopicId, article.TopicId);
            Assert.AreEqual(validatedArticleTitle, article.Title);
            Assert.AreEqual(validatedArticleText, article.Text);
            Assert.AreEqual(tags, article.Tags);
            Assert.IsTrue(new DateTime() != article.PublicationDate);

            userRepositoryMock.Verify(r => r.GetById(user.UserId), Times.Once);

            topicRepositoryMock.Verify(r => r.GetById(topic.TopicId), Times.Once);

            tagRepositoryMock.Verify(
            r => r.Get(It.IsAny<Expression<Func<Tag, bool>>>(), null, It.Is<Expression<Func<Tag, object>>[]>(e => !e.Any())),
            Times.Once);

            articleRepositoryMock.Verify(
            r =>
            r.Insert(
            It.Is<Article>(a => a.UserId == user.UserId && a.Title == validatedArticleTitle && a.Text == validatedArticleText)),
            Times.Once);

            unitOfWorkMock.Verify(u => u.Save(), Times.Once);

            this._articleValidationMock.Verify(v => v.ValidateTitle(articleTitle), Times.Once);
            this._articleValidationMock.Verify(v => v.ValidateArticleText(articleText), Times.Once);
        }

        [Test]
        public void Create_OneOfTheTagsIdIsLessOrEqualToZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            int userId = 1;
            string articleTitle = "article_title";
            string articleText = "article_text";
            int topicId = 2;

            IArticleService target = new ArticleService(new Mock<IUnitOfWork>().Object, this._articleValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(
            () => target.Create(userId, articleTitle, articleText, topicId, new[] { 0, 1, 2 }));
            Assert.Throws<ArgumentOutOfRangeException>(
            () => target.Create(userId, articleTitle, articleText, topicId, new[] { -1, 2, 3 }));
        }

        [Test]
        public void Create_TagIdsArrayIsEmpty_DoNotGetTagsFromRepository()
        {
            // Arrange
            User user = new User { UserId = 1 };

            string articleTitle = "article_title";
            string articleText = "article_text";
            string validatedArticleTitle = "validated_article_title";
            string validatedArticleText = "validated_article_text";

            Topic topic = new Topic { TopicId = 2 };
            int[] tagIds = { };

            // Arrange - mock userRepository
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(r => r.GetById(user.UserId))
            .Returns(user);

            // Arrange - mock topicRepository
            Mock<ITopicRepository> topicRepositoryMock = new Mock<ITopicRepository>();

            topicRepositoryMock.Setup(r => r.GetById(topic.TopicId))
            .Returns(topic);

            // Arrange - mock tagRepository
            Mock<ITagRepository> tagRepositoryMock = new Mock<ITagRepository>();

            // Arrange - mock articleRepository
            Mock<IArticleRepository> articleRepositoryMock = new Mock<IArticleRepository>();

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.UserRepository)
            .Returns(userRepositoryMock.Object);

            unitOfWorkMock.SetupGet(u => u.TopicRepository)
            .Returns(topicRepositoryMock.Object);

            unitOfWorkMock.SetupGet(u => u.TagRepository)
            .Returns(tagRepositoryMock.Object);

            unitOfWorkMock.SetupGet(u => u.ArticleRepository)
            .Returns(articleRepositoryMock.Object);

            // Arrange - mock articleValidation
            this._articleValidationMock.Setup(v => v.ValidateTitle(articleTitle))
            .Returns(validatedArticleTitle);

            this._articleValidationMock.Setup(v => v.ValidateArticleText(articleText))
            .Returns(validatedArticleText);

            // Arrange - create target
            IArticleService target = new ArticleService(unitOfWorkMock.Object, this._articleValidationMock.Object);

            // Act
            Article article = target.Create(user.UserId, articleTitle, articleText, topic.TopicId, tagIds);

            // Assert
            Assert.IsNotNull(article);
            Assert.AreEqual(user.UserId, article.UserId);
            Assert.AreEqual(topic.TopicId, article.TopicId);
            Assert.AreEqual(validatedArticleTitle, article.Title);
            Assert.AreEqual(validatedArticleText, article.Text);
            Assert.IsFalse(article.Tags.Any());
            Assert.IsTrue(new DateTime() != article.PublicationDate);

            userRepositoryMock.Verify(r => r.GetById(user.UserId), Times.Once);

            topicRepositoryMock.Verify(r => r.GetById(topic.TopicId), Times.Once);

            tagRepositoryMock.Verify(
            r =>
            r.Get(It.IsAny<Expression<Func<Tag, bool>>>(), It.IsAny<Func<IQueryable<Tag>, IOrderedQueryable<Tag>>>(),
            It.IsAny<Expression<Func<Tag, object>>[]>()), Times.Never);

            articleRepositoryMock.Verify(
            r =>
            r.Insert(
            It.Is<Article>(a => a.UserId == user.UserId && a.Title == validatedArticleTitle && a.Text == validatedArticleText)),
            Times.Once);

            unitOfWorkMock.Verify(u => u.Save(), Times.Once);

            this._articleValidationMock.Verify(v => v.ValidateTitle(articleTitle), Times.Once);
            this._articleValidationMock.Verify(v => v.ValidateArticleText(articleText), Times.Once);
        }

        [Test]
        public void Create_TitleOrTextIsEmpty_ThrowsArgumentException()
        {
            // Arrange
            int userId = 1;
            string articleTitle = "article_title";
            string articleText = "article_text";
            int topicId = 2;
            int[] tagIds = { 1, 2, 3, 4 };

            IArticleService target = new ArticleService(new Mock<IUnitOfWork>().Object, this._articleValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentException>(() => target.Create(userId, "", articleText, topicId, tagIds));
            Assert.Throws<ArgumentException>(() => target.Create(userId, articleTitle, "", topicId, tagIds));
        }

        [Test]
        public void Create_TitleOrTextOrTagIdsIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            int userId = 1;
            string articleTitle = "article_title";
            string articleText = "article_text";
            int topicId = 2;
            int[] tagIds = { 1, 2, 3, 4 };

            IArticleService target = new ArticleService(new Mock<IUnitOfWork>().Object, this._articleValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.Create(userId, null, articleText, topicId, tagIds));
            Assert.Throws<ArgumentNullException>(() => target.Create(userId, articleTitle, null, topicId, tagIds));
            Assert.Throws<ArgumentNullException>(() => target.Create(userId, articleTitle, articleText, topicId, null));
        }

        [Test]
        public void Create_TopicIdIsLessOrEqualToZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            int userId = 1;
            string articleTitle = "article_title";
            string articleText = "article_text";
            int[] tagIds = { 1, 2, 3, 4 };

            IArticleService target = new ArticleService(new Mock<IUnitOfWork>().Object, this._articleValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Create(userId, articleTitle, articleText, -1, tagIds));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Create(userId, articleTitle, articleText, 0, tagIds));
        }

        [Test]
        public void Create_UserIdIsLessOrEqualToZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            string articleTitle = "article_title";
            string articleText = "article_text";
            int topicId = 2;
            int[] tagIds = { 1, 2, 3, 4 };

            IArticleService target = new ArticleService(new Mock<IUnitOfWork>().Object, this._articleValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Create(-1, articleTitle, articleText, topicId, tagIds));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Create(0, articleTitle, articleText, topicId, tagIds));
        }

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

            this._articleValidationMock.Setup(v => v.ValidateArticleText(It.IsAny<string>()))
            .Returns((string text) => text);

            this._articleValidationMock.Setup(v => v.ValidateTitle(It.IsAny<string>()))
            .Returns((string title) => title);
        }
    }
}