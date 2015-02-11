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

    public class CommentService : ServiceBase, ICommentService
    {
        private readonly int _permittedPeriodForEditing;

        public CommentService(IUnitOfWork unitOfWork, int permittedPeriodForEditing = 0)
        : base(unitOfWork)
        {
            Guard.IntMoreOrEqualToZero(permittedPeriodForEditing, "permittedPeriodForEditing");

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
                vote = new CommentVote { UserId = userId };

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