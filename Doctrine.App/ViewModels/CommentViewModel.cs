namespace Doctrine.App.ViewModels
{
    using System.Collections.Generic;

    using Doctrine.Domain.Models;

    public class CommentViewModel
    {
        public CommentViewModel()
        {
            this.Replies = new List<CommentViewModel>();
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

        public int NegativeVotesCount
        {
            get;
            set;
        }

        public int ParentCommentId
        {
            get;
            set;
        }

        public int PositiveVotesCount
        {
            get;
            set;
        }

        public IEnumerable<CommentViewModel> Replies
        {
            get;
            set;
        }
    }
}