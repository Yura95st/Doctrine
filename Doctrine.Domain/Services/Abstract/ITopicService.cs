namespace Doctrine.Domain.Services.Abstract
{
    using Doctrine.Domain.Models;

    public interface ITopicService
    {
        /// <summary>Creates the topic with specified name.</summary>
        /// <param name="topicName">The topic's name.</param>
        /// <returns>The topic.</returns>
        Topic Create(string topicName);

        /// <summary>Edits the topic.</summary>
        /// <param name="topicId">The topic's identifier.</param>
        /// <param name="newTopicName">The topic's new name.</param>
        void Edit(int topicId, string newTopicName);

        /// <summary>Gets the topic by specified name.</summary>
        /// <param name="topicName">The topic's name.</param>
        /// <returns>The topic.</returns>
        Topic GetByName(string topicName);
    }
}