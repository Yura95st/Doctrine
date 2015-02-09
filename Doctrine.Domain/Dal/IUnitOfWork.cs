namespace Doctrine.Domain.Dal
{
    using System;

    using Doctrine.Domain.Dal.Repositories.Abstract;

    public interface IUnitOfWork : IDisposable
    {
        /// <summary>Gets the article repository.</summary>
        /// <value>The article repository.</value>
        IArticleRepository ArticleRepository
        {
            get;
        }

        /// <summary>Gets the comment repository.</summary>
        /// <value>The comment repository.</value>
        ICommentRepository CommentRepository
        {
            get;
        }

        /// <summary>Gets the tag repository.</summary>
        /// <value>The tag repository.</value>
        ITagRepository TagRepository
        {
            get;
        }

        /// <summary>Gets the topic repository.</summary>
        /// <value>The topic repository.</value>
        ITopicRepository TopicRepository
        {
            get;
        }

        /// <summary>Gets the user repository.</summary>
        /// <value>The user repository.</value>
        IUserRepository UserRepository
        {
            get;
        }

        /// <summary>Gets the visitor repository.</summary>
        /// <value>The visitor repository.</value>
        IVisitorRepository VisitorRepository
        {
            get;
        }

        /// <summary>Saves changes in all repositories.</summary>
        void Save();
    }
}