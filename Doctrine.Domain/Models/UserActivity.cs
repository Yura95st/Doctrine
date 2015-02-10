namespace Doctrine.Domain.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("UserActivity")]
    public class UserActivity
    {
        [Key]
        public int ActivityId
        {
            get;
            set;
        }

        [Column(TypeName = "datetime2")]
        public DateTime LogOnDate
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

        public virtual Visitor Visitor
        {
            get;
            set;
        }

        public int VisitorId
        {
            get;
            set;
        }
    }
}