namespace Doctrine.Domain.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Topic")]
    public class Topic
    {
        public Topic()
        {
            this.Articles = new HashSet<Article>();
        }

        public virtual ICollection<Article> Articles
        {
            get;
            set;
        }

        [Required]
        [StringLength(100)]
        public string Name
        {
            get;
            set;
        }

        public int TopicId
        {
            get;
            set;
        }
    }
}