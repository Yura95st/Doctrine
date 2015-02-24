namespace Doctrine.App.ViewModels
{
    using System.Collections.Generic;

    using Doctrine.App.Models;
    using Doctrine.Domain.Models;

    public class CommentViewModel
    {
        public CommentViewModel()
        {
            this.Replies = new List<CommentViewModel>();
            this.VotingInfo = new VotingInfo();
        }

        public Comment Comment
        {
            get;
            set;
        }

        public bool IsFromArticleAuthor
        {
            get;
            set;
        }

        public int ParentCommentId
        {
            get;
            set;
        }

        public IEnumerable<CommentViewModel> Replies
        {
            get;
            set;
        }

        public VotingInfo VotingInfo
        {
            get;
            set;
        }
    }
}