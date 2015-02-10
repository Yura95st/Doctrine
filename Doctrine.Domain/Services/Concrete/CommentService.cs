namespace Doctrine.Domain.Services.Concrete
{
    using System;

    using Doctrine.Domain.Dal;
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

        public bool CanEdit(int userId, Comment comment)
        {
            Guard.IntMoreThanZero(userId, "userId");
            Guard.NotNull(comment, "comment");

            if (userId != comment.UserId)
            {
                return false;
            }

            TimeSpan timeSpan = DateTime.Now.AddSeconds(this._permittedPeriodForEditing).Subtract(comment.Date);

            return timeSpan.TotalMinutes >= 0;
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