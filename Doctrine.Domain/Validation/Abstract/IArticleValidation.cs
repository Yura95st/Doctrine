namespace Doctrine.Domain.Validation.Abstract
{
    public interface IArticleValidation
    {
        /// <summary>Validates the article's text.</summary>
        /// <param name="articleText">The article's text.</param>
        /// <returns>The validated article's text.</returns>
        string ValidateArticleText(string articleText);

        /// <summary>Validates the article's title.</summary>
        /// <param name="articleTitle">The article's title.</param>
        /// <returns>The validated article's title.</returns>
        string ValidateTitle(string articleTitle);
    }
}