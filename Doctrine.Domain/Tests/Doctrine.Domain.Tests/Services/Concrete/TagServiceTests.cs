namespace Doctrine.Domain.Tests.Services.Concrete
{
    using System;

    using Doctrine.Domain.Dal;
    using Doctrine.Domain.Dal.Repositories.Abstract;
    using Doctrine.Domain.Exceptions.InvalidFormat;
    using Doctrine.Domain.Models;
    using Doctrine.Domain.Services.Abstract;
    using Doctrine.Domain.Services.Concrete;
    using Doctrine.Domain.Validation.Abstract;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class TagServiceTests
    {
        private Mock<ITagValidation> _tagValidationMock;

        [Test]
        public void Create_TagNameFormatIsInvalid_ThrowsInvalidTagNameFormatException()
        {
            // Arrange
            string tagName = "invalid_tag_name";

            this._tagValidationMock.Setup(v => v.IsValidName(tagName))
            .Returns(false);

            ITagService target = new TagService(new Mock<IUnitOfWork>().Object, this._tagValidationMock.Object);

            // Act and Assert
            Assert.Throws<InvalidTagNameFormatException>(() => target.Create(tagName));

            this._tagValidationMock.Verify(v => v.IsValidName(tagName), Times.Once);
        }

        [Test]
        public void Create_TagNameIsEmpty_ThrowsArgumentException()
        {
            // Arrange
            ITagService target = new TagService(new Mock<IUnitOfWork>().Object, this._tagValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentException>(() => target.Create(""));
        }

        [Test]
        public void Create_TagNameIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            ITagService target = new TagService(new Mock<IUnitOfWork>().Object, this._tagValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.Create(null));
        }

        [Test]
        public void Create_TagNameIsValid_CreatesTag()
        {
            // Arrange
            string tagName = "tag_name";

            // Arrange - mock tagRepository
            Mock<ITagRepository> tagRepositoryMock = new Mock<ITagRepository>();

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.TagRepository)
            .Returns(tagRepositoryMock.Object);

            // Arrange - create target
            ITagService target = new TagService(unitOfWorkMock.Object, this._tagValidationMock.Object);

            // Act
            Tag createdTag = target.Create(tagName);

            // Assert
            Assert.IsNotNull(createdTag);
            Assert.AreEqual(tagName, createdTag.Name);

            tagRepositoryMock.Verify(r => r.Insert(It.Is<Tag>(t => t.Name == tagName)), Times.Once);

            unitOfWorkMock.Verify(r => r.Save(), Times.Once);
        }

        [Test]
        public void GetByName_NonexistentTagName_ReturnsNull()
        {
            // Arrange
            string tagName = "nonexistent_tag_name";

            // Arrange - mock tagRepository
            Mock<ITagRepository> tagRepositoryMock = new Mock<ITagRepository>();

            tagRepositoryMock.Setup(r => r.GetByName(tagName))
            .Returns((Tag)null);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.TagRepository)
            .Returns(tagRepositoryMock.Object);

            // Arrange - create target
            ITagService target = new TagService(unitOfWorkMock.Object, this._tagValidationMock.Object);

            // Act
            Tag tag = target.GetByName(tagName);

            // Assert
            Assert.IsNull(tag);
        }

        [Test]
        public void GetByName_TagNameIsEmpty_ThrowsArgumentException()
        {
            // Arrange
            ITagService target = new TagService(new Mock<IUnitOfWork>().Object, this._tagValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentException>(() => target.GetByName(""));
        }

        [Test]
        public void GetByName_TagNameIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            ITagService target = new TagService(new Mock<IUnitOfWork>().Object, this._tagValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.GetByName(null));
        }

        [Test]
        public void GetByName_TagNameIsValid_ReturnsTag()
        {
            // Arrange
            Tag testTag = new Tag { TagId = 1, Name = "tag_name" };

            // Arrange - mock tagRepository
            Mock<ITagRepository> tagRepositoryMock = new Mock<ITagRepository>();

            tagRepositoryMock.Setup(r => r.GetByName(testTag.Name))
            .Returns(testTag);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.TagRepository)
            .Returns(tagRepositoryMock.Object);

            // Arrange - create target
            ITagService target = new TagService(unitOfWorkMock.Object, this._tagValidationMock.Object);

            // Act
            Tag tag = target.GetByName(testTag.Name);

            // Assert
            Assert.AreSame(testTag, tag);
        }

        [SetUp]
        public void Init()
        {
            this.MockTagValidation();
        }

        private void MockTagValidation()
        {
            this._tagValidationMock = new Mock<ITagValidation>();

            this._tagValidationMock.Setup(v => v.IsValidName(It.IsAny<string>()))
            .Returns(true);
        }
    }
}