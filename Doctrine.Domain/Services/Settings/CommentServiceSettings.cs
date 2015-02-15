namespace Doctrine.Domain.Services.Settings
{
    using Doctrine.Domain.Utils;

    public class CommentServiceSettings
    {
        private readonly int _maxCommentTreeLevel;

        private readonly int _permittedPeriodForDeleting;

        private readonly int _permittedPeriodForEditing;

        public CommentServiceSettings(int permittedPeriodForDeleting, int permittedPeriodForEditing, int maxCommentTreeLevel)
        {
            Guard.IntMoreOrEqualToZero(permittedPeriodForDeleting, "permittedPeriodForDeleting");
            Guard.IntMoreOrEqualToZero(permittedPeriodForEditing, "permittedPeriodForEditing");
            Guard.IntMoreOrEqualToZero(maxCommentTreeLevel, "maxCommentTreeLevel");

            this._permittedPeriodForDeleting = permittedPeriodForDeleting;
            this._permittedPeriodForEditing = permittedPeriodForEditing;

            this._maxCommentTreeLevel = maxCommentTreeLevel;
        }

        /// <summary>Gets the maximum comment's tree level.</summary>
        /// <value>The maximum comment's tree level.</value>
        public int MaxCommentTreeLevel
        {
            get
            {
                return this._maxCommentTreeLevel;
            }
        }

        /// <summary>Gets the permitted period for deleting (in seconds).</summary>
        /// <value>The permitted period for deleting (in seconds).</value>
        public int PermittedPeriodForDeleting
        {
            get
            {
                return this._permittedPeriodForDeleting;
            }
        }

        /// <summary>Gets the permitted period for editing (in seconds).</summary>
        /// <value>The permitted period for editing (in seconds).</value>
        public int PermittedPeriodForEditing
        {
            get
            {
                return this._permittedPeriodForEditing;
            }
        }
    }
}