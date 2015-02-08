namespace Doctrine.Domain.Services.Abstract
{
    using Doctrine.Domain.Models;

    public interface IUserService
    {
        /// <summary>Authenticates the user.</summary>
        /// <param name="email">The email.</param>
        /// <param name="password">The password.</param>
        /// <returns>Authenticated user.</returns>
        User Authenticate(string email, string password);

        /// <summary>Creates new user.</summary>
        /// <param name="email">The email.</param>
        /// <param name="fullName">The full name.</param>
        /// <param name="password">The password.</param>
        /// <returns>Created user.</returns>
        User Create(string email, string fullName, string password);

        /// <summary>Deletes the user.</summary>
        /// <param name="userId">The user identifier.</param>
        void Delete(int userId);

        /// <summary>Gets the by identifier.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>User.</returns>
        User GetById(int userId);
    }
}