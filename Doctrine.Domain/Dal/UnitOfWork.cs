namespace Doctrine.Domain.Dal
{
    using System;

    using Doctrine.Domain.Dal.Repositories.Common;
    using Doctrine.Domain.Models;

    public class UnitOfWork : IUnitOfWork
    {
        private readonly DoctrineContext _context = new DoctrineContext();

        private Repository<User> _courseRepository;

        private bool _disposed;

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

        public IRepository<User> UserRepository
        {
            get
            {
                if (this._courseRepository == null)
                {
                    this._courseRepository = new Repository<User>(this._context);
                }
                return this._courseRepository;
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