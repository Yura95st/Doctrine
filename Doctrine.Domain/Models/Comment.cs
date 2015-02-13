namespace Doctrine.Domain.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Comment")]
    public class Comment
    {
        public Comment()
        {
            this.CommentVotes = new HashSet<CommentVote>();
            this.Comment1 = new HashSet<Comment>();
            this.Comments = new HashSet<Comment>();
        }

        public virtual Article Article
        {
            get;
            set;
        }

        public int ArticleId
        {
            get;
            set;
        }

        public virtual ICollection<Comment> Comment1
        {
            get;
            set;
        }

        public int CommentId
        {
            get;
            set;
        }

        public virtual ICollection<Comment> Comments
        {
            get;
            set;
        }

        public virtual ICollection<CommentVote> CommentVotes
        {
            get;
            set;
        }

        [Column(TypeName = "datetime2")]
        public DateTime Date
        {
            get;
            set;
        }

        public bool IsDeleted
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

        public virtual User User
        {
            get;
            set;
        }

        public int UserId
        {
            get;
            set;
        }
    }
}