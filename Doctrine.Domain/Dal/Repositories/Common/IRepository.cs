namespace Doctrine.Domain.Dal.Repositories.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Doctrine.Domain.Dal.Query.Abstract;

    public interface IRepository<TEntity>
    where TEntity : class
    {
        /// <summary>
        ///     Deletes the entity with specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        void Delete(object id);

        /// <summary>Deletes the entity.</summary>
        /// <param name="entityToDelete">The entity.</param>
        void Delete(TEntity entityToDelete);

        /// <summary>Gets the list of entities.</summary>
        /// <param name="filter">The filter.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="selector">The selector.</param>
        /// <returns>The list of entities.</returns>
        IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null,
                                 Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                 params Expression<Func<TEntity, object>>[] selector);

        /// <summary>Gets the list of entities.</summary>
        /// <param name="repositoryQuery">The get query.</param>
        /// <returns>The list of entities.</returns>
        IEnumerable<TEntity> Get(IRepositoryQuery<TEntity> repositoryQuery);

        /// <summary>Gets the entity by specified identifier.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The entity.</returns>
        TEntity GetById(object id);

        /// <summary>Inserts the specified entity.</summary>
        /// <param name="entity">The entity.</param>
        void Insert(TEntity entity);

        /// <summary>Updates the entity.</summary>
        /// <param name="entityToUpdate">The entity.</param>
        void Update(TEntity entityToUpdate);
    }
}