namespace Doctrine.Domain.Services.Concrete
{
    using System;
    using System.Linq;

    using Doctrine.Domain.Dal;
    using Doctrine.Domain.Exceptions;
    using Doctrine.Domain.Exceptions.NotFound;
    using Doctrine.Domain.Models;
    using Doctrine.Domain.Services.Abstract;
    using Doctrine.Domain.Services.Common;
    using Doctrine.Domain.Services.Settings;
    using Doctrine.Domain.Utils;
    using Doctrine.Domain.Validation.Abstract;

    public class CommentService : ServiceBase, ICommentService
    {
        private readonly ICommentValidation _commentValidation;

        private readonly CommentServiceSettings _serviceSettings;

        public CommentService(IUnitOfWork unitOfWork, ICommentValidation commentValidation,
                              CommentServiceSettings serviceSettings)
        : base(unitOfWork)
        {
            Guard.NotNull(commentValidation, "commentValidation");
            Guard.NotNull(serviceSettings, "serviceSettings");

            this._commentValidation = commentValidation;
            this._serviceSettings = serviceSettings;
        }

        #region ICommentService Members

        public void AddVote(int commentId, int userId, bool voteIsPositive)
        {
            Guard.IntMoreThanZero(commentId, "commentId");
            Guard.IntMoreThanZero(userId, "userId");

            Comment comment =
            this._unitOfWork.CommentRepository.Get(c => c.CommentId == commentId, selector: c => c.CommentVotes)
            .FirstOrDefault();

            if (comment == null)
            {
                throw new CommentNotFoundException(String.Format("Comment with ID '{0}' was not found.", commentId));
            }

            CommentVote vote = comment.CommentVotes.FirstOrDefault(v => v.UserId == userId);

            if (vote != null && vote.IsPositive == voteIsPositive)
            {
                // Same comment's vote has already been added.
                return;
            }

            if (vote == null)
            {
                User user = this._unitOfWork.UserRepository.GetById(userId);

                if (user == null)
                {
                    throw new UserNotFoundException(String.Format("User with ID '{0}' was not found.", userId));
                }

                vote = new CommentVote { UserId = user.UserId };

                comment.CommentVotes.Add(vote);
            }

            vote.IsPositive = voteIsPositive;

            this._unitOfWork.CommentRepository.Update(comment);
            this._unitOfWork.Save();
        }

        public bool CanDelete(int userId, Comment comment)
        {
            Guard.IntMoreThanZero(userId, "userId");
            Guard.NotNull(comment, "comment");

            return (!comment.IsDeleted && comment.UserId == userId
                    && !CommentService.IsPermittedPeriodExpired(comment.Date,
                    this._serviceSettings.PermittedPeriodForDeleting));
        }

        public bool CanEdit(int userId, Comment comment)
        {
            Guard.IntMoreThanZero(userId, "userId");
            Guard.NotNull(comment, "comment");

            return (!comment.IsDeleted && comment.UserId == userId
                    && !CommentService.IsPermittedPeriodExpired(comment.Date, this._serviceSettings.PermittedPeriodForEditing));
        }

        public Comment Create(int userId, int articleId, string commentText)
        {
            Guard.IntMoreThanZero(userId, "userId");
            Guard.IntMoreThanZero(articleId, "articleId");
            Guard.NotNullOrEmpty(commentText, "commentText");

            User user = this._unitOfWork.UserRepository.GetById(userId);

            if (user == null)
            {
                throw new UserNotFoundException(String.Format("User with ID '{0}' was not found.", userId));
            }

            Article article = this._unitOfWork.ArticleRepository.GetById(articleId);

            if (article == null)
            {
                throw new ArticleNotFoundException(String.Format("Article with ID '{0}' was not found.", articleId));
            }

            // Validate comment's text
            string validatedCommentText = this._commentValidation.ValidateCommentText(commentText);

            Comment comment = new Comment
            { ArticleId = article.ArticleId, UserId = user.UserId, Text = validatedCommentText, Date = DateTime.Now };

            this._unitOfWork.CommentRepository.Insert(comment);
            this._unitOfWork.Save();

            return comment;
        }

        public void Delete(int commentId, int userId)
        {
            Guard.IntMoreThanZero(commentId, "commentId");
            Guard.IntMoreThanZero(userId, "userId");

            Comment comment = this._unitOfWork.CommentRepository.GetById(commentId);

            if (comment == null)
            {
                throw new CommentNotFoundException(String.Format("Comment with ID '{0}' was not found.", commentId));
            }

            if (comment.IsDeleted)
            {
                throw new DeletingCommentIsForbiddenException(String.Format(
                "Comment with ID '{0}' has already been deleted.", commentId));
            }

            if (comment.UserId != userId)
            {
                throw new DeletingCommentIsForbiddenException(
                String.Format(
                "User with ID '{0}' is not the author of the comment with ID '{0}' and aren't allowed to delete it.",
                commentId));
            }

            if (CommentService.IsPermittedPeriodExpired(comment.Date, this._serviceSettings.PermittedPeriodForDeleting))
            {
                throw new PermittedPeriodForDeletingExpiredException(
                String.Format("Permitted period ('{0}' seconds) for deleting comment with dateTime '{1}' is expired.",
                this._serviceSettings.PermittedPeriodForDeleting, comment.Date));
            }

            comment.IsDeleted = true;

            this._unitOfWork.CommentRepository.Update(comment);
            this._unitOfWork.Save();
        }

        public void DeleteVote(int commentId, int userId)
        {
            Guard.IntMoreThanZero(commentId, "commentId");
            Guard.IntMoreThanZero(userId, "userId");

            Comment comment =
            this._unitOfWork.CommentRepository.Get(c => c.CommentId == commentId, selector: c => c.CommentVotes)
            .FirstOrDefault();

            if (comment == null)
            {
                throw new CommentNotFoundException(String.Format("Comment with ID '{0}' was not found.", commentId));
            }

            CommentVote vote = comment.CommentVotes.FirstOrDefault(v => v.UserId == userId);

            if (vote == null)
            {
                throw new CommentVoteNotFoundException(
                String.Format("Comment's vote with commentId '{0}' and userId '{1}' was not found.", commentId, userId));
            }

            comment.CommentVotes.Remove(vote);

            this._unitOfWork.CommentRepository.Update(comment);
            this._unitOfWork.Save();
        }

        public void Edit(int commentId, int userId, string newCommentText)
        {
            Guard.IntMoreThanZero(commentId, "commentId");
            Guard.IntMoreThanZero(userId, "userId");
            Guard.NotNullOrEmpty(newCommentText, "newCommentText");

            Comment comment =
            this._unitOfWork.CommentRepository.Get(c => c.CommentId == commentId, selector: c => c.CommentEdit)
            .FirstOrDefault();

            if (comment == null)
            {
                throw new CommentNotFoundException(String.Format("Comment with ID '{0}' was not found.", commentId));
            }

            if (comment.IsDeleted)
            {
                throw new EditingCommentIsForbiddenException(
                String.Format("Comment with ID '{0}' has been deleted and can't be edited.", commentId));
            }

            if (comment.UserId != userId)
            {
                throw new EditingCommentIsForbiddenException(
                String.Format(
                "User with ID '{0}' is not the author of the comment with ID '{0}' and aren't allowed to edit it.", commentId));
            }

            if (CommentService.IsPermittedPeriodExpired(comment.Date, this._serviceSettings.PermittedPeriodForEditing))
            {
                throw new PermittedPeriodForEditingExpiredException(
                String.Format("Permitted period ('{0}' seconds) for editing comment with dateTime '{1}' is expired.",
                this._serviceSettings.PermittedPeriodForEditing, comment.Date));
            }

            if (comment.CommentEdit == null)
            {
                comment.CommentEdit = new CommentEdit();
            }

            comment.CommentEdit.EditDate = DateTime.Now;

            // Validate comment's text
            string validatedNewCommentText = this._commentValidation.ValidateCommentText(newCommentText);

            comment.Text = validatedNewCommentText;

            this._unitOfWork.CommentRepository.Update(comment);
            this._unitOfWork.Save();
        }

        public Comment Reply(int commentId, int userId, string commentText)
        {
            Guard.IntMoreThanZero(commentId, "commentId");
            Guard.IntMoreThanZero(userId, "userId");
            Guard.NotNullOrEmpty(commentText, "commentText");

            Comment comment =
            this._unitOfWork.CommentRepository.Get(c => c.CommentId == commentId, selector: c => c.Comment1)
            .FirstOrDefault();

            if (comment == null)
            {
                throw new CommentNotFoundException(String.Format("Comment with ID '{0}' was not found.", commentId));
            }

            User user = this._unitOfWork.UserRepository.GetById(userId);

            if (user == null)
            {
                throw new UserNotFoundException(String.Format("User with ID '{0}' was not found.", userId));
            }

            // Validate comment's text
            string validatedCommentText = this._commentValidation.ValidateCommentText(commentText);

            Comment replyComment = new Comment
            { ArticleId = comment.ArticleId, UserId = user.UserId, Text = validatedCommentText, Date = DateTime.Now };

            comment.Comment1.Add(replyComment);

            this._unitOfWork.CommentRepository.Update(comment);
            this._unitOfWork.Save();

            return replyComment;
        }

        public CommentServiceSettings ServiceSettings
        {
            get
            {
                return this._serviceSettings;
            }
        }

        #endregion

        private static bool IsPermittedPeriodExpired(DateTime dateTime, int permittedPeriod)
        {
            TimeSpan timeSpan = dateTime.AddSeconds(permittedPeriod)
            .Subtract(DateTime.Now);

            return timeSpan.TotalMilliseconds < 0;
        }
    }
}