namespace Doctrine.Domain.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ArticleVisitor")]
    public class ArticleVisitor
    {
        public virtual Article Article
        {
            get;
            set;
        }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ArticleId
        {
            get;
            set;
        }

        public DateTime LastViewDate
        {
            get;
            set;
        }

        public virtual Visitor Visitor
        {
            get;
            set;
        }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int VisitorId
        {
            get;
            set;
        }
    }
}