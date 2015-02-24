namespace Doctrine.Domain.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Article")]
    public class Article
    {
        public Article()
        {
            this.ArticleVisitors = new HashSet<ArticleVisitor>();
            this.Comments = new HashSet<Comment>();
            this.UserFavorites = new HashSet<UserFavorite>();
            this.UserReadHistories = new HashSet<UserReadHistory>();
            this.Tags = new HashSet<Tag>();
        }

        public int ArticleId
        {
            get;
            set;
        }

        public virtual ICollection<ArticleVisitor> ArticleVisitors
        {
            get;
            set;
        }

        public virtual ICollection<Comment> Comments
        {
            get;
            set;
        }

        [Column(TypeName = "datetime2")]
        public DateTime PublicationDate
        {
            get;
            set;
        }

        public virtual ICollection<Tag> Tags
        {
            get;
            set;
        }

        [Required]
        public string Text
        {
            get;
            set;
        }

        [Required]
        [StringLength(150)]
        public string Title
        {
            get;
            set;
        }

        public virtual Topic Topic
        {
            get;
            set;
        }

        public int TopicId
        {
            get;
            set;
        }

        public virtual User User
        {
            get;
            set;
        }

        public virtual ICollection<UserFavorite> UserFavorites
        {
            get;
            set;
        }

        public int UserId
        {
            get;
            set;
        }

        public virtual ICollection<UserReadHistory> UserReadHistories
        {
            get;
            set;
        }
    }
}