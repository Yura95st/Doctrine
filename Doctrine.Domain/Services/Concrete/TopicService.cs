namespace Doctrine.Domain.Services.Concrete
{
    using System;

    using Doctrine.Domain.Dal;
    using Doctrine.Domain.Exceptions.InvalidFormat;
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

            Topic topic = new Topic { Name = topicName };

            this._unitOfWork.TopicRepository.Insert(topic);

            this._unitOfWork.Save();

            return topic;
        }

        public Topic GetByName(string topicName)
        {
            Guard.NotNullOrEmpty(topicName, "topicName");

            return this._unitOfWork.TopicRepository.GetByName(topicName);
        }

        #endregion
    }
}