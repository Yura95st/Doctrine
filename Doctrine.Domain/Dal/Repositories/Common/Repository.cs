namespace Doctrine.Domain.Dal.Repositories.Common
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Linq.Expressions;

    public class Repository<TEntity> : IRepository<TEntity>
    where TEntity : class
    {
        protected readonly DoctrineContext _context;

        protected readonly DbSet<TEntity> _dbSet;

        public Repository(DoctrineContext context)
        {
            this._context = context;
            this._dbSet = context.Set<TEntity>();
        }

        #region IRepository<TEntity> Members

        public virtual void Delete(object id)
        {
            TEntity entityToDelete = this._dbSet.Find(id);
            this.Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (this._context.Entry(entityToDelete)
            .State == EntityState.Detached)
            {
                this._dbSet.Attach(entityToDelete);
            }
            this._dbSet.Remove(entityToDelete);
        }

        public virtual IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null,
                                                Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                params Expression<Func<TEntity, object>>[] selector)
        {
            IQueryable<TEntity> query = this._dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (selector != null)
            {
                foreach (Expression<Func<TEntity, object>> selectorItem in selector)
                {
                    query = query.Include(selectorItem);
                }
            }

            if (orderBy != null)
            {
                return orderBy(query)
                .ToList();
            }
            return query.ToList();
        }

        public virtual TEntity GetById(object id)
        {
            return this._dbSet.Find(id);
        }

        public virtual void Insert(TEntity entity)
        {
            this._dbSet.Add(entity);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            this._dbSet.Attach(entityToUpdate);

            DbEntityEntry dbEntityEntry = this._context.Entry(entityToUpdate);

            //Ensure only modified fields are updated.
            foreach (string property in dbEntityEntry.OriginalValues.PropertyNames)
            {
                object original = dbEntityEntry.OriginalValues.GetValue<object>(property);
                object current = dbEntityEntry.CurrentValues.GetValue<object>(property);

                if (original != null && !original.Equals(current))
                {
                    dbEntityEntry.Property(property)
                    .IsModified = true;
                }
            }
        }

        #endregion
    }
}