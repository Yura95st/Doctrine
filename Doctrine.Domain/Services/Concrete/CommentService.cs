namespace Doctrine.Domain.Services.Concrete
{
    using System;
    using System.Linq;

    using Doctrine.Domain.Dal;
    using Doctrine.Domain.Exceptions.NotFound;
    using Doctrine.Domain.Models;
    using Doctrine.Domain.Services.Abstract;
    using Doctrine.Domain.Services.Common;
    using Doctrine.Domain.Utils;
    using Doctrine.Domain.Validation.Abstract;

    public class CommentService : ServiceBase, ICommentService
    {
        private readonly ICommentValidation _commentValidation;

        private readonly int _permittedPeriodForEditing;

        public CommentService(IUnitOfWork unitOfWork, ICommentValidation commentValidation, int permittedPeriodForEditing = 0)
        : base(unitOfWork)
        {
            Guard.NotNull(commentValidation, "commentValidation");
            Guard.IntMoreOrEqualToZero(permittedPeriodForEditing, "permittedPeriodForEditing");

            this._commentValidation = commentValidation;
            this._permittedPeriodForEditing = permittedPeriodForEditing;
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

        public bool CanEdit(int userId, Comment comment)
        {
            Guard.IntMoreThanZero(userId, "userId");
            Guard.NotNull(comment, "comment");

            if (userId != comment.UserId)
            {
                return false;
            }

            TimeSpan timeSpan = DateTime.Now.AddSeconds(this._permittedPeriodForEditing)
            .Subtract(comment.Date);

            return timeSpan.TotalMinutes >= 0;
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

        public int PermittedPeriodForEditing
        {
            get
            {
                return this._permittedPeriodForEditing;
            }
        }

        #endregion
    }
}