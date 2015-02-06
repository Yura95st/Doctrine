namespace Doctrine.Domain.Dal.Repositories.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public interface IRepository<TEntity>
    where TEntity : class
    {
        void Delete(object id);

        void Delete(TEntity entityToDelete);

        IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null,
                                 Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                 string includeProperties = "");

        TEntity GetById(object id);

        void Insert(TEntity entity);

        void Update(TEntity entityToUpdate);
    }
}