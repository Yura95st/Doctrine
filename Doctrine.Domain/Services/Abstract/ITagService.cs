namespace Doctrine.Domain.Services.Abstract
{
    using Doctrine.Domain.Models;

    public interface ITagService
    {
        /// <summary>Creates the tag.</summary>
        /// <param name="tagName">The tag's name.</param>
        /// <returns>The tag.</returns>
        Tag Create(string tagName);

        /// <summary>Gets the tag by specified name.</summary>
        /// <param name="tagName">The tag's name.</param>
        /// <returns>The tag.</returns>
        Tag GetByName(string tagName);
    }
}