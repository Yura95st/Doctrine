namespace Doctrine.App.ViewModels
{
    using System.Collections.Generic;

    using Doctrine.App.Models;
    using Doctrine.Domain.Models;

    public class ArticleViewModel
    {
        public ArticleViewModel()
        {
            this.Comments = new List<CommentViewModel>();
            this.VotingInfo = new VotingInfo();
        }

        public Article Article
        {
            get;
            set;
        }

        public IEnumerable<CommentViewModel> Comments
        {
            get;
            set;
        }

        public int CommentsCount
        {
            get;
            set;
        }

        public int FavsCount
        {
            get;
            set;
        }

        public bool IsPreviewMode
        {
            get;
            set;
        }

        public int ViewsCount
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