namespace Doctrine.Domain.Services.Abstract
{
    using Doctrine.Domain.Models;

    public interface ICommentService
    {
        /// <summary>Gets the permitted period for editing (in seconds).</summary>
        /// <value>The permitted period for editing (in seconds).</value>
        int PermittedPeriodForEditing
        {
            get;
        }

        /// <summary>Adds the user's vote to the comment with specified identifier.</summary>
        /// <param name="commentId">The comment identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="voteIsPositive">Determines whether vote is positive or not.</param>
        void AddVote(int commentId, int userId, bool voteIsPositive);

        /// <summary>
        ///     Checks whether user with specified identifier can edit the specified comment.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="comment">The comment.</param>
        /// <returns>True if user can edit comment, false - otherwise</returns>
        bool CanEdit(int userId, Comment comment);

        /// <summary>Deletes the user's vote.</summary>
        /// <param name="commentId">The comment identifier.</param>
        /// <param name="userId">The user identifier.</param>
        void DeleteVote(int commentId, int userId);
    }
}