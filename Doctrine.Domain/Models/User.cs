namespace Doctrine.Domain.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("User")]
    public class User
    {
        public User()
        {
            this.Comments = new HashSet<Comment>();
            this.CommentVotes = new HashSet<CommentVote>();
            this.UserActivities = new HashSet<UserActivity>();
            this.UserFavorites = new HashSet<UserFavorite>();
            this.UserReadHistories = new HashSet<UserReadHistory>();
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

        [Required]
        [StringLength(150)]
        public string Email
        {
            get;
            set;
        }

        [Required]
        [StringLength(50)]
        public string FirstName
        {
            get;
            set;
        }

        [Required]
        [StringLength(50)]
        public string LastName
        {
            get;
            set;
        }

        [Required]
        [StringLength(44)]
        public string Password
        {
            get;
            set;
        }

        [Column(TypeName = "datetime2")]
        public DateTime RegistrationDate
        {
            get;
            set;
        }

        [Required]
        [StringLength(44)]
        public string Salt
        {
            get;
            set;
        }

        public virtual ICollection<UserActivity> UserActivities
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