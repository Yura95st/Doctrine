namespace Doctrine.Domain.Services.Concrete
{
    using System;

    using Doctrine.Domain.Dal;
    using Doctrine.Domain.Exceptions.AlreadyExists;
    using Doctrine.Domain.Exceptions.InvalidFormat;
    using Doctrine.Domain.Exceptions.NotFound;
    using Doctrine.Domain.Models;
    using Doctrine.Domain.Services.Abstract;
    using Doctrine.Domain.Services.Common;
    using Doctrine.Domain.Utils;
    using Doctrine.Domain.Validation.Abstract;

    public class TopicService : ServiceBase, ITopicService
    {
        private readonly ITopicValidation _topicValidation;

        public TopicService(IUnitOfWork unitOfWork, ITopicValidation topicValidation)
        : base(unitOfWork)
        {
            Guard.NotNull(topicValidation, "topicValidation");

            this._topicValidation = topicValidation;
        }

        #region ITopicService Members

        public Topic Create(string topicName)
        {
            Guard.NotNullOrEmpty(topicName, "topicName");

            if (!this._topicValidation.IsValidName(topicName))
            {
                throw new InvalidTopicNameFormatException(String.Format("Topic's name '{0}' has invalid format.", topicName));
            }

            Topic topic = this._unitOfWork.TopicRepository.GetByName(topicName);

            if (topic != null)
            {
                throw new TopicNameAlreadyExistsException(String.Format("Topic with name '{0}' already exists.", topicName));
            }

            topic = new Topic { Name = topicName };

            this._unitOfWork.TopicRepository.Insert(topic);

            this._unitOfWork.Save();

            return topic;
        }

        public void Edit(int topicId, string newTopicName)
        {
            Guard.IntMoreThanZero(topicId, "topicId");
            Guard.NotNullOrEmpty(newTopicName, "newTopicName");

            if (!this._topicValidation.IsValidName(newTopicName))
            {
                throw new InvalidTopicNameFormatException(String.Format("Topic's new name '{0}' has invalid format.",
                newTopicName));
            }

            Topic topic = this._unitOfWork.TopicRepository.GetByName(newTopicName);

            if (topic != null)
            {
                throw new TopicNameAlreadyExistsException(String.Format("Topic with name '{0}' already exists.", newTopicName));
            }

            topic = this._unitOfWork.TopicRepository.GetById(topicId);

            if (topic == null)
            {
                throw new TopicNotFoundException(String.Format("Topic with ID '{0}' was not found.", topicId));
            }

            topic.Name = newTopicName;

            this._unitOfWork.TopicRepository.Update(topic);
            this._unitOfWork.Save();
        }

        public Topic GetByName(string topicName)
        {
            Guard.NotNullOrEmpty(topicName, "topicName");

            return this._unitOfWork.TopicRepository.GetByName(topicName);
        }

        #endregion
    }
}