namespace Doctrine.Domain.Services.Abstract
{
    using Doctrine.Domain.Models;

    public interface ITopicService
    {
        /// <summary>Creates the topic with specified name.</summary>
        /// <param name="topicName">The topic's name.</param>
        /// <returns>The topic.</returns>
        Topic Create(string topicName);
    }
}