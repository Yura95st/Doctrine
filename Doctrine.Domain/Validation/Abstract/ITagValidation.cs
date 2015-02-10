namespace Doctrine.Domain.Validation.Abstract
{
    public interface ITagValidation
    {
        /// <summary>
        ///     Checks whether string value represents valid tag's name.
        /// </summary>
        /// <param name="tagName">The tag's name.</param>
        /// <returns>True if tag's name is valid, false - otherwise</returns>
        bool IsValidName(string tagName);
    }
}