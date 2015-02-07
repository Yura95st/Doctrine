namespace Doctrine.Domain.Dal
{
    using System;

    using Doctrine.Domain.Dal.Repositories.Abstract;

    public interface IUnitOfWork : IDisposable
    {
        IArticleRepository ArticleRepository
        {
            get;
        }

        ICommentRepository CommentRepository
        {
            get;
        }

        ITagRepository TagRepository
        {
            get;
        }

        ITopicRepository TopicRepository
        {
            get;
        }

        IUserRepository UserRepository
        {
            get;
        }

        IVisitorRepository VisitorRepository
        {
            get;
        }

        void Save();
    }
}