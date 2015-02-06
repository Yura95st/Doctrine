namespace Doctrine.Domain.Dal
{
    using System;

    using Doctrine.Domain.Dal.Repositories.Common;
    using Doctrine.Domain.Models;

    internal interface IUnitOfWork : IDisposable
    {
        IRepository<User> UserRepository
        {
            get;
        }

        void Save();
    }
}