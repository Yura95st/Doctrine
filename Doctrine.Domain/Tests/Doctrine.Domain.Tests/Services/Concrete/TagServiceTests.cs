namespace Doctrine.Domain.Tests.Services.Concrete
{
    using System;

    using Doctrine.Domain.Dal;
    using Doctrine.Domain.Dal.Repositories.Abstract;
    using Doctrine.Domain.Exceptions.AlreadyExists;
    using Doctrine.Domain.Exceptions.InvalidFormat;
    using Doctrine.Domain.Exceptions.NotFound;
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
        public void Create_TagNameAlreadyExists_ThrowsTagNameAlreadyExistsException()
        {
            // Arrange
            string tagName = "tag_name";

            Tag tag = new Tag { TagId = 1, Name = tagName };

            // Arrange - mock tagRepository
            Mock<ITagRepository> tagRepositoryMock = new Mock<ITagRepository>();

            tagRepositoryMock.Setup(r => r.GetByName(tagName))
            .Returns(tag);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.TagRepository)
            .Returns(tagRepositoryMock.Object);

            // Arrange - create target
            ITagService target = new TagService(unitOfWorkMock.Object, this._tagValidationMock.Object);

            // Act and Assert
            Assert.Throws<TagNameAlreadyExistsException>(() => target.Create(tagName));

            tagRepositoryMock.Verify(r => r.GetByName(tagName), Times.Once);
            tagRepositoryMock.Verify(r => r.Insert(It.Is<Tag>(t => t.Name == tagName)), Times.Never);

            unitOfWorkMock.Verify(r => r.Save(), Times.Never);
        }

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
        public void Delete_NonexistentTagId_ThrowsTagNotFoundException()
        {
            // Arrange
            int tagId = 1;

            // Arrange - mock tagRepository
            Mock<ITagRepository> tagRepositoryMock = new Mock<ITagRepository>();

            tagRepositoryMock.Setup(r => r.GetById(tagId))
            .Returns((Tag)null);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.TagRepository)
            .Returns(tagRepositoryMock.Object);

            ITagService target = new TagService(unitOfWorkMock.Object, this._tagValidationMock.Object);

            // Act and Assert
            Assert.Throws<TagNotFoundException>(() => target.Delete(tagId));

            tagRepositoryMock.Verify(r => r.GetById(tagId), Times.Once);
            tagRepositoryMock.Verify(r => r.Delete(It.Is<Tag>(t => t.TagId == tagId)), Times.Never);

            unitOfWorkMock.Verify(r => r.Save(), Times.Never);
        }

        [Test]
        public void Delete_TagIdIsLessOrEqualToZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            ITagService target = new TagService(new Mock<IUnitOfWork>().Object, this._tagValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Delete(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Delete(0));
        }

        [Test]
        public void Delete_TagIdIsValid_DeletesTag()
        {
            // Arrange
            Tag tag = new Tag { TagId = 1 };

            // Arrange - mock tagRepository
            Mock<ITagRepository> tagRepositoryMock = new Mock<ITagRepository>();

            tagRepositoryMock.Setup(r => r.GetById(tag.TagId))
            .Returns(tag);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.TagRepository)
            .Returns(tagRepositoryMock.Object);

            ITagService target = new TagService(unitOfWorkMock.Object, this._tagValidationMock.Object);

            // Act
            target.Delete(tag.TagId);

            // Assert
            tagRepositoryMock.Verify(r => r.GetById(tag.TagId), Times.Once);
            tagRepositoryMock.Verify(r => r.Delete(It.Is<Tag>(t => t.TagId == tag.TagId)), Times.Once);

            unitOfWorkMock.Verify(r => r.Save(), Times.Once);
        }

        [Test]
        public void Edit_AllCredentialsAreValid_EditsTag()
        {
            // Arrange
            Tag tag = new Tag { TagId = 1 };
            string newTagName = "new_tag_name";

            // Arrange - mock tagRepository
            Mock<ITagRepository> tagRepositoryMock = new Mock<ITagRepository>();

            tagRepositoryMock.Setup(r => r.GetById(tag.TagId))
            .Returns(tag);

            tagRepositoryMock.Setup(r => r.GetByName(newTagName))
            .Returns((Tag)null);

            Tag editedTag = null;

            tagRepositoryMock.Setup(r => r.Update(It.Is<Tag>(t => t.TagId == tag.TagId)))
            .Callback((Tag t) => editedTag = t);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.TagRepository)
            .Returns(tagRepositoryMock.Object);

            // Arrange - create target
            ITagService target = new TagService(unitOfWorkMock.Object, this._tagValidationMock.Object);

            // Act
            target.Edit(tag.TagId, newTagName);

            // Assert
            Assert.IsNotNull(editedTag);
            Assert.AreEqual(newTagName, editedTag.Name);

            tagRepositoryMock.Verify(r => r.GetById(tag.TagId), Times.Once);
            tagRepositoryMock.Verify(r => r.GetByName(newTagName), Times.Once);
            tagRepositoryMock.Verify(r => r.Update(It.Is<Tag>(t => t.TagId == tag.TagId)), Times.Once);

            unitOfWorkMock.Verify(r => r.Save(), Times.Once);
        }

        [Test]
        public void Edit_NewTagNameAlreadyExists_ThrowsTagNameAlreadyExistsException()
        {
            // Arrange
            int tagId = 1;
            string newTagName = "new_tag_name";

            Tag tag = new Tag { Name = newTagName };

            // Arrange - mock tagRepository
            Mock<ITagRepository> tagRepositoryMock = new Mock<ITagRepository>();

            tagRepositoryMock.Setup(r => r.GetByName(newTagName))
            .Returns(tag);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.TagRepository)
            .Returns(tagRepositoryMock.Object);

            // Arrange - create target
            ITagService target = new TagService(unitOfWorkMock.Object, this._tagValidationMock.Object);

            // Act and Assert
            Assert.Throws<TagNameAlreadyExistsException>(() => target.Edit(tagId, newTagName));

            tagRepositoryMock.Verify(r => r.GetByName(newTagName), Times.Once);
            tagRepositoryMock.Verify(r => r.GetById(tagId), Times.Never);
            tagRepositoryMock.Verify(r => r.Update(It.Is<Tag>(t => t.TagId == tagId)), Times.Never);

            unitOfWorkMock.Verify(r => r.Save(), Times.Never);
        }

        [Test]
        public void Edit_NonexistentTagId_ThrowsTagNotFoundException()
        {
            // Arrange
            int tagId = 1;
            string newTagName = "new_tag_name";

            // Arrange - mock tagRepository
            Mock<ITagRepository> tagRepositoryMock = new Mock<ITagRepository>();

            tagRepositoryMock.Setup(r => r.GetByName(newTagName))
            .Returns((Tag)null);

            tagRepositoryMock.Setup(r => r.GetById(tagId))
            .Returns((Tag)null);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.TagRepository)
            .Returns(tagRepositoryMock.Object);

            ITagService target = new TagService(unitOfWorkMock.Object, this._tagValidationMock.Object);

            // Act and Assert
            Assert.Throws<TagNotFoundException>(() => target.Edit(tagId, newTagName));

            tagRepositoryMock.Verify(r => r.GetByName(newTagName), Times.Once);
            tagRepositoryMock.Verify(r => r.GetById(tagId), Times.Once);
            tagRepositoryMock.Verify(r => r.Update(It.Is<Tag>(t => t.TagId == tagId)), Times.Never);

            unitOfWorkMock.Verify(r => r.Save(), Times.Never);
        }

        [Test]
        public void Edit_TagIdIsLessOrEqualToZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            string newTagName = "new_tag_name";

            ITagService target = new TagService(new Mock<IUnitOfWork>().Object, this._tagValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Edit(-1, newTagName));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Edit(0, newTagName));
        }

        [Test]
        public void Edit_TagNameFormatIsInvalid_ThrowsInvalidTagNameFormatException()
        {
            // Arrange
            int tagId = 1;
            string newTagName = "invalid_new_tag_name";

            // Arrange - mock tagValidation
            this._tagValidationMock.Setup(v => v.IsValidName(newTagName))
            .Returns(false);

            // Arrange - create target
            ITagService target = new TagService(new Mock<IUnitOfWork>().Object, this._tagValidationMock.Object);

            // Act and Assert
            Assert.Throws<InvalidTagNameFormatException>(() => target.Edit(tagId, newTagName));

            this._tagValidationMock.Verify(v => v.IsValidName(newTagName), Times.Once);
        }

        [Test]
        public void Edit_TagNameIsEmpty_ThrowsArgumentException()
        {
            // Arrange
            int tagId = 1;

            ITagService target = new TagService(new Mock<IUnitOfWork>().Object, this._tagValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentException>(() => target.Edit(tagId, ""));
        }

        [Test]
        public void Edit_TagNameIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            int tagId = 1;

            ITagService target = new TagService(new Mock<IUnitOfWork>().Object, this._tagValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.Edit(tagId, null));
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