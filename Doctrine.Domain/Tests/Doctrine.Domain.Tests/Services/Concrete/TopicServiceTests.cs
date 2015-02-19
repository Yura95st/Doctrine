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
    public class TopicServiceTests
    {
        private Mock<ITopicValidation> _topicValidationMock;

        [Test]
        public void Create_TopicNameAlreadyExists_ThrowsTopicNameAlreadyExistsException()
        {
            // Arrange
            string topicName = "topic_name";

            Topic topic = new Topic { TopicId = 1, Name = topicName };

            // Arrange - mock topicRepository
            Mock<ITopicRepository> topicRepositoryMock = new Mock<ITopicRepository>();

            topicRepositoryMock.Setup(r => r.GetByName(topicName))
            .Returns(topic);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.TopicRepository)
            .Returns(topicRepositoryMock.Object);

            // Arrange - create target
            ITopicService target = new TopicService(unitOfWorkMock.Object, this._topicValidationMock.Object);

            // Act and Assert
            Assert.Throws<TopicNameAlreadyExistsException>(() => target.Create(topicName));

            topicRepositoryMock.Verify(r => r.GetByName(topicName), Times.Once);
            topicRepositoryMock.Verify(r => r.Insert(It.Is<Topic>(t => t.Name == topicName)), Times.Never);

            unitOfWorkMock.Verify(r => r.Save(), Times.Never);
        }

        [Test]
        public void Create_TopicNameFormatIsInvalid_ThrowsInvalidTopicNameFormatException()
        {
            // Arrange
            string topicName = "invalid_topic_name";

            this._topicValidationMock.Setup(v => v.IsValidName(topicName))
            .Returns(false);

            ITopicService target = new TopicService(new Mock<IUnitOfWork>().Object, this._topicValidationMock.Object);

            // Act and Assert
            Assert.Throws<InvalidTopicNameFormatException>(() => target.Create(topicName));

            this._topicValidationMock.Verify(v => v.IsValidName(topicName), Times.Once);
        }

        [Test]
        public void Create_TopicNameIsEmpty_ThrowsArgumentException()
        {
            // Arrange
            ITopicService target = new TopicService(new Mock<IUnitOfWork>().Object, this._topicValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentException>(() => target.Create(""));
        }

        [Test]
        public void Create_TopicNameIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            ITopicService target = new TopicService(new Mock<IUnitOfWork>().Object, this._topicValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.Create(null));
        }

        [Test]
        public void Create_TopicNameIsValid_CreatesTopic()
        {
            // Arrange
            string topicName = "topicName";

            // Arrange - mock topicRepository
            Mock<ITopicRepository> topicRepositoryMock = new Mock<ITopicRepository>();

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.TopicRepository)
            .Returns(topicRepositoryMock.Object);

            // Arrange - create target
            ITopicService target = new TopicService(unitOfWorkMock.Object, this._topicValidationMock.Object);

            // Act
            Topic createdTopic = target.Create(topicName);

            // Assert
            Assert.IsNotNull(createdTopic);
            Assert.AreEqual(topicName, createdTopic.Name);

            topicRepositoryMock.Verify(r => r.Insert(It.Is<Topic>(t => t.Name == topicName)), Times.Once);

            unitOfWorkMock.Verify(r => r.Save(), Times.Once);
        }

        [Test]
        public void Edit_AllCredentialsAreValid_EditsTopic()
        {
            // Arrange
            Topic topic = new Topic { TopicId = 1 };
            string newTopicName = "new_topic_name";

            // Arrange - mock topicRepository
            Mock<ITopicRepository> topicRepositoryMock = new Mock<ITopicRepository>();

            topicRepositoryMock.Setup(r => r.GetById(topic.TopicId))
            .Returns(topic);

            topicRepositoryMock.Setup(r => r.GetByName(newTopicName))
            .Returns((Topic)null);

            Topic editedTopic = null;

            topicRepositoryMock.Setup(r => r.Update(It.Is<Topic>(t => t.TopicId == topic.TopicId)))
            .Callback((Topic t) => editedTopic = t);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.TopicRepository)
            .Returns(topicRepositoryMock.Object);

            // Arrange - create target
            ITopicService target = new TopicService(unitOfWorkMock.Object, this._topicValidationMock.Object);

            // Act
            target.Edit(topic.TopicId, newTopicName);

            // Assert
            Assert.IsNotNull(editedTopic);
            Assert.AreEqual(newTopicName, editedTopic.Name);

            topicRepositoryMock.Verify(r => r.GetById(topic.TopicId), Times.Once);
            topicRepositoryMock.Verify(r => r.GetByName(newTopicName), Times.Once);
            topicRepositoryMock.Verify(r => r.Update(It.Is<Topic>(t => t.TopicId == topic.TopicId)), Times.Once);

            unitOfWorkMock.Verify(r => r.Save(), Times.Once);
        }

        [Test]
        public void Edit_NewTopicNameAlreadyExists_ThrowsTopicNameAlreadyExistsException()
        {
            // Arrange
            int topicId = 1;
            string newTopicName = "new_topic_name";

            Topic topic = new Topic { Name = newTopicName };

            // Arrange - mock topicRepository
            Mock<ITopicRepository> topicRepositoryMock = new Mock<ITopicRepository>();

            topicRepositoryMock.Setup(r => r.GetByName(newTopicName))
            .Returns(topic);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.TopicRepository)
            .Returns(topicRepositoryMock.Object);

            // Arrange - create target
            ITopicService target = new TopicService(unitOfWorkMock.Object, this._topicValidationMock.Object);

            // Act and Assert
            Assert.Throws<TopicNameAlreadyExistsException>(() => target.Edit(topicId, newTopicName));

            topicRepositoryMock.Verify(r => r.GetByName(newTopicName), Times.Once);
            topicRepositoryMock.Verify(r => r.GetById(topicId), Times.Never);
            topicRepositoryMock.Verify(r => r.Update(It.Is<Topic>(t => t.TopicId == topicId)), Times.Never);

            unitOfWorkMock.Verify(r => r.Save(), Times.Never);
        }

        [Test]
        public void Edit_NonexistentTopicId_ThrowsTopicNotFoundException()
        {
            // Arrange
            int topicId = 1;
            string newTopicName = "new_topic_name";

            // Arrange - mock topicRepository
            Mock<ITopicRepository> topicRepositoryMock = new Mock<ITopicRepository>();

            topicRepositoryMock.Setup(r => r.GetByName(newTopicName))
            .Returns((Topic)null);

            topicRepositoryMock.Setup(r => r.GetById(topicId))
            .Returns((Topic)null);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.TopicRepository)
            .Returns(topicRepositoryMock.Object);

            ITopicService target = new TopicService(unitOfWorkMock.Object, this._topicValidationMock.Object);

            // Act and Assert
            Assert.Throws<TopicNotFoundException>(() => target.Edit(topicId, newTopicName));

            topicRepositoryMock.Verify(r => r.GetByName(newTopicName), Times.Once);
            topicRepositoryMock.Verify(r => r.GetById(topicId), Times.Once);
            topicRepositoryMock.Verify(r => r.Update(It.Is<Topic>(t => t.TopicId == topicId)), Times.Never);

            unitOfWorkMock.Verify(r => r.Save(), Times.Never);
        }

        [Test]
        public void Edit_TopicIdIsLessOrEqualToZero_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            string newTopicName = "new_topic_name";

            ITopicService target = new TopicService(new Mock<IUnitOfWork>().Object, this._topicValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Edit(-1, newTopicName));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Edit(0, newTopicName));
        }

        [Test]
        public void Edit_TopicNameFormatIsInvalid_ThrowsInvalidTopicNameFormatException()
        {
            // Arrange
            int topicId = 1;
            string newTopicName = "invalid_new_topic_name";

            // Arrange - mock topicValidation
            this._topicValidationMock.Setup(v => v.IsValidName(newTopicName))
            .Returns(false);

            // Arrange - create target
            ITopicService target = new TopicService(new Mock<IUnitOfWork>().Object, this._topicValidationMock.Object);

            // Act and Assert
            Assert.Throws<InvalidTopicNameFormatException>(() => target.Edit(topicId, newTopicName));

            this._topicValidationMock.Verify(v => v.IsValidName(newTopicName), Times.Once);
        }

        [Test]
        public void Edit_TopicNameIsEmpty_ThrowsArgumentException()
        {
            // Arrange
            int topicId = 1;

            ITopicService target = new TopicService(new Mock<IUnitOfWork>().Object, this._topicValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentException>(() => target.Edit(topicId, ""));
        }

        [Test]
        public void Edit_TopicNameIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            int topicId = 1;

            ITopicService target = new TopicService(new Mock<IUnitOfWork>().Object, this._topicValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.Edit(topicId, null));
        }

        [Test]
        public void GetByName_NonexistentTopicName_ReturnsNull()
        {
            // Arrange
            string topicName = "nonexistent_topic_name";

            // Arrange - mock topicRepository
            Mock<ITopicRepository> topicRepositoryMock = new Mock<ITopicRepository>();

            topicRepositoryMock.Setup(r => r.GetByName(topicName))
            .Returns((Topic)null);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.TopicRepository)
            .Returns(topicRepositoryMock.Object);

            // Arrange - create target
            ITopicService target = new TopicService(unitOfWorkMock.Object, this._topicValidationMock.Object);

            // Act
            Topic topic = target.GetByName(topicName);

            // Assert
            Assert.IsNull(topic);
        }

        [Test]
        public void GetByName_TopicNameIsEmpty_ThrowsArgumentException()
        {
            // Arrange
            ITopicService target = new TopicService(new Mock<IUnitOfWork>().Object, this._topicValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentException>(() => target.GetByName(""));
        }

        [Test]
        public void GetByName_TopicNameIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            ITopicService target = new TopicService(new Mock<IUnitOfWork>().Object, this._topicValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.GetByName(null));
        }

        [Test]
        public void GetByName_TopicNameIsValid_ReturnsTopic()
        {
            // Arrange
            Topic testTopic = new Topic { TopicId = 1, Name = "topic_name" };

            // Arrange - mock topicRepository
            Mock<ITopicRepository> topicRepositoryMock = new Mock<ITopicRepository>();

            topicRepositoryMock.Setup(r => r.GetByName(testTopic.Name))
            .Returns(testTopic);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock.SetupGet(u => u.TopicRepository)
            .Returns(topicRepositoryMock.Object);

            // Arrange - create target
            ITopicService target = new TopicService(unitOfWorkMock.Object, this._topicValidationMock.Object);

            // Act
            Topic topic = target.GetByName(testTopic.Name);

            // Assert
            Assert.AreSame(testTopic, topic);
        }

        [SetUp]
        public void Init()
        {
            this.MockTopicValidation();
        }

        private void MockTopicValidation()
        {
            this._topicValidationMock = new Mock<ITopicValidation>();

            this._topicValidationMock.Setup(v => v.IsValidName(It.IsAny<string>()))
            .Returns(true);
        }
    }
}