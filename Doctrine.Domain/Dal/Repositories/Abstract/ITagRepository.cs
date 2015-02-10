namespace Doctrine.Domain.Dal.Repositories.Abstract
{
    using Doctrine.Domain.Dal.Repositories.Common;
    using Doctrine.Domain.Models;

    public interface ITagRepository : IRepository<Tag>
    {
        /// <summary>Gets the tag by specified name.</summary>
        /// <param name="tagName">The tag's name.</param>
        /// <returns>The tag.</returns>
        Tag GetByName(string tagName);
    }
}