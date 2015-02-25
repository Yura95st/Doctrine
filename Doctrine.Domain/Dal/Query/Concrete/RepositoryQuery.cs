namespace Doctrine.Domain.Dal.Query.Concrete
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Doctrine.Domain.Dal.Query.Abstract;
    using Doctrine.Domain.Dal.Query.SortCriteria.Abstract;
    using Doctrine.Domain.Utils;

    public class RepositoryQuery<T> : IRepositoryQuery<T>
    {
        private readonly IList<Expression<Func<T, bool>>> _filters;

        private readonly IList<Expression<Func<T, object>>> _includeProperties;

        private readonly IList<ISortCriteria<T>> _sortCriteriaList;

        private int _skip;

        private int _take;

        public RepositoryQuery()
        {
            this._filters = new List<Expression<Func<T, bool>>>();

            this._includeProperties = new List<Expression<Func<T, object>>>();

            this._sortCriteriaList = new List<ISortCriteria<T>>();
        }

        #region IRepositoryQuery<T> Members

        public void AddFilter(Expression<Func<T, bool>> filter)
        {
            Guard.NotNull(filter, "filter");

            if (!this._filters.Contains(filter))
            {
                this._filters.Add(filter);
            }
        }

        public void AddSortCriteria(ISortCriteria<T> sortCriteria)
        {
            Guard.NotNull(sortCriteria, "sortCriteria");

            if (!this._sortCriteriaList.Contains(sortCriteria))
            {
                this._sortCriteriaList.Add(sortCriteria);
            }
        }

        public void IncludeProperty(Expression<Func<T, object>> property)
        {
            Guard.NotNull(property, "property");

            if (!this._includeProperties.Contains(property))
            {
                this._includeProperties.Add(property);
            }
        }

        public IEnumerable<Expression<Func<T, bool>>> Filters
        {
            get
            {
                return this._filters;
            }
        }

        public IEnumerable<Expression<Func<T, object>>> IncludedProperties
        {
            get
            {
                return this._includeProperties;
            }
        }

        public int Skip
        {
            get
            {
                return this._skip;
            }
            set
            {
                Guard.IntMoreOrEqualToZero(value, "value");

                this._skip = value;
            }
        }

        public IEnumerable<ISortCriteria<T>> SortCriteriaList
        {
            get
            {
                return this._sortCriteriaList;
            }
        }

        public int Take
        {
            get
            {
                return this._take;
            }
            set
            {
                Guard.IntMoreOrEqualToZero(value, "value");

                this._take = value;
            }
        }

        #endregion
    }
}