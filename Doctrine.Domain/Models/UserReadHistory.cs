namespace Doctrine.Domain.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("UserReadHistory")]
    public class UserReadHistory
    {
        public virtual Article Article
        {
            get;
            set;
        }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ArticleId
        {
            get;
            set;
        }

        public DateTime ReadDate
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
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserId
        {
            get;
            set;
        }
    }
}