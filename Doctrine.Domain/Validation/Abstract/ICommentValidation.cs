namespace Doctrine.Domain.Validation.Abstract
{
    public interface ICommentValidation
    {
        /// <summary>Validates the comment's text.</summary>
        /// <param name="commentText">The comment's text.</param>
        /// <returns>The validated comment's text.</returns>
        string ValidateCommentText(string commentText);
    }
}