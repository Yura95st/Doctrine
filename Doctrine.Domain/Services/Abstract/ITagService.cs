namespace Doctrine.Domain.Services.Abstract
{
    using Doctrine.Domain.Models;

    public interface ITagService
    {
        /// <summary>Creates the tag.</summary>
        /// <param name="tagName">The tag's name.</param>
        /// <returns>The tag.</returns>
        Tag Create(string tagName);

        /// <summary>Edits the tag.</summary>
        /// <param name="tagId">The tag's identifier.</param>
        /// <param name="newTagName">The tag's new name.</param>
        void Edit(int tagId, string newTagName);

        /// <summary>Gets the tag by specified name.</summary>
        /// <param name="tagName">The tag's name.</param>
        /// <returns>The tag.</returns>
        Tag GetByName(string tagName);
    }
}