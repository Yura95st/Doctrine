namespace Doctrine.Domain.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("CommentEdit")]
    public class CommentEdit
    {
        public virtual Comment Comment
        {
            get;
            set;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CommentId
        {
            get;
            set;
        }

        [Column(TypeName = "datetime2")]
        public DateTime EditDate
        {
            get;
            set;
        }
    }
}