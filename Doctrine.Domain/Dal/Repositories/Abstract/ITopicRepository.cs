namespace Doctrine.Domain.Dal.Repositories.Abstract
{
    using Doctrine.Domain.Dal.Repositories.Common;
    using Doctrine.Domain.Models;

    public interface ITopicRepository : IRepository<Topic>
    {
        /// <summary>Gets the topic by specified name.</summary>
        /// <param name="topicName">The topic's name.</param>
        /// <returns>The topic.</returns>
        Topic GetByName(string topicName);
    }
}