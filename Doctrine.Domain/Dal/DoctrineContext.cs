namespace Doctrine.Domain.Dal
{
    using System.Data.Entity;

    using Doctrine.Domain.Models;

    public class DoctrineContext : DbContext
    {
        public DoctrineContext()
        : base("name=DoctrineContext")
        {
        }

        public virtual DbSet<Article> Articles
        {
            get;
            set;
        }

        public virtual DbSet<ArticleVisitor> ArticleVisitors
        {
            get;
            set;
        }

        public virtual DbSet<CommentEdit> CommentEdits
        {
            get;
            set;
        }

        public virtual DbSet<Comment> Comments
        {
            get;
            set;
        }

        public virtual DbSet<CommentVote> CommentVotes
        {
            get;
            set;
        }

        public virtual DbSet<Tag> Tags
        {
            get;
            set;
        }

        public virtual DbSet<Topic> Topics
        {
            get;
            set;
        }

        public virtual DbSet<UserActivity> UserActivities
        {
            get;
            set;
        }

        public virtual DbSet<UserFavorite> UserFavorites
        {
            get;
            set;
        }

        public virtual DbSet<UserReadHistory> UserReadHistories
        {
            get;
            set;
        }

        public virtual DbSet<User> Users
        {
            get;
            set;
        }

        public virtual DbSet<Visitor> Visitors
        {
            get;
            set;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>()
            .HasMany(e => e.ArticleVisitors)
            .WithRequired(e => e.Article)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<Article>()
            .HasMany(e => e.Comments)
            .WithRequired(e => e.Article)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<Article>()
            .HasMany(e => e.UserFavorites)
            .WithRequired(e => e.Article)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<Article>()
            .HasMany(e => e.UserReadHistories)
            .WithRequired(e => e.Article)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<Article>()
            .HasMany(e => e.Tags)
            .WithMany(e => e.Articles)
            .Map(m => m.ToTable("ArticleTag")
            .MapLeftKey("ArticleId")
            .MapRightKey("TagId"));

            modelBuilder.Entity<Comment>()
            .HasOptional(e => e.CommentEdit)
            .WithRequired(e => e.Comment);

            modelBuilder.Entity<Comment>()
            .HasMany(e => e.CommentVotes)
            .WithRequired(e => e.Comment)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<Comment>()
            .HasMany(e => e.Comment1)
            .WithMany(e => e.Comments)
            .Map(m => m.ToTable("CommentReply")
            .MapLeftKey("CommentId")
            .MapRightKey("ReplyCommentId"));

            modelBuilder.Entity<User>()
            .Property(e => e.Password)
            .IsFixedLength()
            .IsUnicode(false);

            modelBuilder.Entity<User>()
            .Property(e => e.Salt)
            .IsFixedLength()
            .IsUnicode(false);

            modelBuilder.Entity<User>()
            .HasMany(e => e.Articles)
            .WithRequired(e => e.User)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
            .HasMany(e => e.Comments)
            .WithRequired(e => e.User)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
            .HasMany(e => e.CommentVotes)
            .WithRequired(e => e.User)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
            .HasMany(e => e.UserActivities)
            .WithRequired(e => e.User)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
            .HasMany(e => e.UserFavorites)
            .WithRequired(e => e.User)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
            .HasMany(e => e.UserReadHistories)
            .WithRequired(e => e.User)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<Visitor>()
            .Property(e => e.IpAddress)
            .IsFixedLength()
            .IsUnicode(false);

            modelBuilder.Entity<Visitor>()
            .HasMany(e => e.ArticleVisitors)
            .WithRequired(e => e.Visitor)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<Visitor>()
            .HasMany(e => e.UserActivities)
            .WithRequired(e => e.Visitor)
            .WillCascadeOnDelete(false);
        }
    }
}