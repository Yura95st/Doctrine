namespace Doctrine.Domain.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("CommentVote")]
    public class CommentVote
    {
        public virtual Comment Comment
        {
            get;
            set;
        }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CommentId
        {
            get;
            set;
        }

        public bool IsPositive
        {
            get;
            set;
        }

        public virtual User User
        {
            get;
            set;
        }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserId
        {
            get;
            set;
        }
    }
}