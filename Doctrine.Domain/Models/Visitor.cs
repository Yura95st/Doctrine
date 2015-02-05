namespace Doctrine.Domain.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Visitor")]
    public class Visitor
    {
        public Visitor()
        {
            this.ArticleVisitors = new HashSet<ArticleVisitor>();
            this.UserActivities = new HashSet<UserActivity>();
        }

        public virtual ICollection<ArticleVisitor> ArticleVisitors
        {
            get;
            set;
        }

        [Required]
        [StringLength(15)]
        public string IpAddress
        {
            get;
            set;
        }

        public virtual ICollection<UserActivity> UserActivities
        {
            get;
            set;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int VisitorId
        {
            get;
            set;
        }
    }
}