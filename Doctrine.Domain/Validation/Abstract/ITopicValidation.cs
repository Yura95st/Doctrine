namespace Doctrine.Domain.Validation.Abstract
{
    public interface ITopicValidation
    {
        /// <summary>
        ///     Checks whether string value represents valid topic's name.
        /// </summary>
        /// <param name="topicName">The topic's name.</param>
        /// <returns>True if topic's name is valid, false - otherwise</returns>
        bool IsValidName(string topicName);
    }
}