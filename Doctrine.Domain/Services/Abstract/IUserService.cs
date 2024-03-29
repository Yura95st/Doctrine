﻿namespace Doctrine.Domain.Services.Abstract
{
    using Doctrine.Domain.Models;

    public interface IUserService
    {
        /// <summary>Adds the article to user's favorites.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="articleId">The article identifier.</param>
        void AddArticleToFavorites(int userId, int articleId);

        /// <summary>Authenticates the visitor as user.</summary>
        /// <param name="visitorId">The visitor identifier.</param>
        /// <param name="email">The email.</param>
        /// <param name="password">The password.</param>
        /// <returns>Authenticated user.</returns>
        User Authenticate(int visitorId, string email, string password);

        /// <summary>Creates new user.</summary>
        /// <param name="email">The email.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="password">The password.</param>
        /// <returns>Created user.</returns>
        User Create(string email, string firstName, string lastName, string password);

        /// <summary>Deletes the user.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="password">The user's password.</param>
        void Delete(int userId, string password);

        /// <summary>Gets the by identifier.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>User.</returns>
        User GetById(int userId);

        /// <summary>Marks the article as read by user.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="articleId">The article identifier.</param>
        void ReadArticle(int userId, int articleId);

        /// <summary>Removes the article from user's favorites.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="articleId">The article identifier.</param>
        void RemoveArticleFromFavorites(int userId, int articleId);

        /// <summary>Removes the fact, that article was read by user.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="articleId">The article identifier.</param>
        void UnreadArticle(int userId, int articleId);
    }
}