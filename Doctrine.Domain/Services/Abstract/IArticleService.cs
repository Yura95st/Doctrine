namespace Doctrine.Domain.Services.Abstract
{
    using Doctrine.Domain.Models;

    public interface IArticleService
    {
        /// <summary>Creates the article.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="title">The article's title.</param>
        /// <param name="text">The article's text.</param>
        /// <param name="topicId">The topic identifier.</param>
        /// <param name="tagIds">The tags' identifiers.</param>
        /// <returns>The article.</returns>
        Article Create(int userId, string title, string text, int topicId, int[] tagIds);

        /// <summary>Deletes the article.</summary>
        /// <param name="articleId">The article identifier.</param>
        void Delete(int articleId);

        /// <summary>Gets the article by identifier.</summary>
        /// <param name="articleId">The article identifier.</param>
        /// <returns>The article.</returns>
        Article GetById(int articleId);
    }
}