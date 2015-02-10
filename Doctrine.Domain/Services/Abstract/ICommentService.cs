namespace Doctrine.Domain.Services.Abstract
{
    using Doctrine.Domain.Models;

    public interface ICommentService
    {
        /// <summary>
        /// Checks whether user with specified identifier can edit the specified comment.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="comment">The comment.</param>
        /// <returns>True if user can edit comment, false - otherwise</returns>
        bool CanEdit(int userId, Comment comment);

        /// <summary>Gets the permitted period for editing (in seconds).</summary>
        /// <value>The permitted period for editing (in seconds).</value>
        int PermittedPeriodForEditing
        {
            get;
        }
    }
}