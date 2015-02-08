namespace Doctrine.Domain.Dal.Repositories.Abstract
{
    using Doctrine.Domain.Dal.Repositories.Common;
    using Doctrine.Domain.Models;

    public interface IUserRepository : IRepository<User>
    {
        /// <summary>Gets the user by email.</summary>
        /// <param name="email">The email.</param>
        /// <returns>The user.</returns>
        User GetByEmail(string email);
    }
}