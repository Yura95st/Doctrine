namespace Doctrine.Domain.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Tag")]
    public class Tag
    {
        public Tag()
        {
            this.Articles = new HashSet<Article>();
        }

        public virtual ICollection<Article> Articles
        {
            get;
            set;
        }

        [Required]
        [StringLength(50)]
        public string Name
        {
            get;
            set;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TagId
        {
            get;
            set;
        }
    }
}