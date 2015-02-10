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
    public class TopicServiceTests
    {
        private Mock<ITopicValidation> _topicValidationMock;

        [Test]
        public void Create_AllCredentialsAreValid_CreatesTheTopic()
        {
            // Arrange
            string topicName = "topicName";
            int newTopicId = 1;

            // Arrange - mock visitorRepository
            Mock<ITopicRepository> topicRepositoryMock = new Mock<ITopicRepository>();

            Topic newTopic = null;

            topicRepositoryMock.Setup(r => r.Insert(It.Is<Topic>(t => t.Name == topicName)))
            .Callback((Topic t) => newTopic = t);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(u => u.TopicRepository)
            .Returns(topicRepositoryMock.Object);

            unitOfWorkMock.Setup(u => u.Save())
            .Callback(() => newTopic.TopicId = newTopicId);

            ITopicService target = new TopicService(unitOfWorkMock.Object, this._topicValidationMock.Object);

            // Act
            Topic createdTopic = target.Create(topicName);

            // Assert
            Assert.IsNotNull(createdTopic);
            Assert.AreEqual(topicName, createdTopic.Name);
            Assert.AreEqual(newTopicId, createdTopic.TopicId);

            topicRepositoryMock.Verify(r => r.Insert(It.Is<Topic>(t => t.Name == topicName)), Times.Once);

            unitOfWorkMock.Verify(r => r.Save(), Times.Once);
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

        [SetUp]
        public void Init()
        {
            this.MockVisitorValidation();
        }

        private void MockVisitorValidation()
        {
            this._topicValidationMock = new Mock<ITopicValidation>();

            this._topicValidationMock.Setup(v => v.IsValidName(It.IsAny<string>()))
            .Returns(true);
        }
    }
}