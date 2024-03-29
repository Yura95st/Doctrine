﻿namespace Doctrine.Domain.Services.Abstract
{
    using Doctrine.Domain.Models;
    using Doctrine.Domain.Services.Settings;

    public interface ICommentService
    {
        /// <summary>Adds the user's vote to the comment with specified identifier.</summary>
        /// <param name="commentId">The comment identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="voteIsPositive">Determines whether vote is positive or not.</param>
        void AddVote(int commentId, int userId, bool voteIsPositive);

        /// <summary>
        ///     Checks whether user with specified identifier can delete the specified comment.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="comment">The comment.</param>
        /// <returns>True if user can delete comment, false - otherwise</returns>
        bool CanDelete(int userId, Comment comment);

        /// <summary>
        ///     Checks whether user with specified identifier can edit the specified comment.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="comment">The comment.</param>
        /// <returns>True if user can edit comment, false - otherwise</returns>
        bool CanEdit(int userId, Comment comment);

        /// <summary>
        ///     Checks whether specified comment can have a reply.
        /// </summary>
        /// <param name="comment">The comment.</param>
        /// <returns>True if comment can have a reply, false - otherwise</returns>
        bool CanHaveReply(Comment comment);

        /// <summary>Creates the comment from user with specified identifier for article with specified identifier.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="articleId">The article identifier.</param>
        /// <param name="commentText">The comment's text.</param>
        /// <returns>Created comment.</returns>
        Comment Create(int userId, int articleId, string commentText);

        /// <summary>Deletes the comment with specified identifier.</summary>
        /// <param name="commentId">The comment identifier.</param>
        /// <param name="userId">The user identifier.</param>
        void Delete(int commentId, int userId);

        /// <summary>Deletes the user's vote.</summary>
        /// <param name="commentId">The comment identifier.</param>
        /// <param name="userId">The user identifier.</param>
        void DeleteVote(int commentId, int userId);

        /// <summary>Edits the comment with specified identifier.</summary>
        /// <param name="commentId">The comment identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="newCommentText">The new comment's text.</param>
        void Edit(int commentId, int userId, string newCommentText);

        /// <summary>Creates the comment as a reply for the comment with specified identifier.</summary>
        /// <param name="commentId">The comment identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="commentText">The comment text.</param>
        /// <returns>Created reply comment.</returns>
        Comment Reply(int commentId, int userId, string commentText);
    }
}