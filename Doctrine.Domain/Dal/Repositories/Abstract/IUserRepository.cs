namespace Doctrine.Domain.Dal.Repositories.Abstract
{
    using System;
    using System.Linq.Expressions;

    using Doctrine.Domain.Dal.Repositories.Common;
    using Doctrine.Domain.Models;

    public interface IUserRepository : IRepository<User>
    {
        /// <summary>Gets the user by email.</summary>
        /// <param name="email">The email.</param>
        /// <returns>The user.</returns>
        User GetByEmail(string email);

        /// <summary>Gets the user by identifier.</summary>
        /// <param name="userId">The user's identifier.</param>
        /// <param name="selector">The selector.</param>
        /// <returns>The user.</returns>
        User GetById(int userId, params Expression<Func<User, object>>[] selector);
    }
}