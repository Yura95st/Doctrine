namespace Doctrine.Domain.Dal
{
    using System;
    using System.Diagnostics;

    using Doctrine.Domain.Dal.Repositories.Abstract;
    using Doctrine.Domain.Dal.Repositories.Concrete;

    public class UnitOfWork : IUnitOfWork
    {
        private readonly DoctrineContext _context;

        private IArticleRepository _articleRepository;

        private ICommentRepository _commentRepository;

        private bool _disposed;

        private ITagRepository _tagRepository;

        private ITopicRepository _topicRepository;

        private IUserRepository _userRepository;

        private IVisitorRepository _visitorRepository;

        public UnitOfWork()
        {
            this._context = new DoctrineContext();

            // Log all the data from Entity Framework to Output window
            this._context.Database.Log = s => Debug.WriteLine(s);
        }

        #region IUnitOfWork Members

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Save()
        {
            this._context.SaveChanges();
        }

        public IArticleRepository ArticleRepository
        {
            get
            {
                if (this._articleRepository == null)
                {
                    this._articleRepository = new ArticleRepository(this._context);
                }
                return this._articleRepository;
            }
        }

        public ICommentRepository CommentRepository
        {
            get
            {
                if (this._commentRepository == null)
                {
                    this._commentRepository = new CommentRepository(this._context);
                }
                return this._commentRepository;
            }
        }

        public ITagRepository TagRepository
        {
            get
            {
                if (this._tagRepository == null)
                {
                    this._tagRepository = new TagRepository(this._context);
                }
                return this._tagRepository;
            }
        }

        public ITopicRepository TopicRepository
        {
            get
            {
                if (this._topicRepository == null)
                {
                    this._topicRepository = new TopicRepository(this._context);
                }
                return this._topicRepository;
            }
        }

        public IUserRepository UserRepository
        {
            get
            {
                if (this._userRepository == null)
                {
                    this._userRepository = new UserRepository(this._context);
                }
                return this._userRepository;
            }
        }

        public IVisitorRepository VisitorRepository
        {
            get
            {
                if (this._visitorRepository == null)
                {
                    this._visitorRepository = new VisitorRepository(this._context);
                }
                return this._visitorRepository;
            }
        }

        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    this._context.Dispose();
                }
            }
            this._disposed = true;
        }
    }
}