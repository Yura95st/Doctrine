namespace Doctrine.Domain.Dal.Query.Abstract
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Doctrine.Domain.Dal.Query.SortCriteria.Abstract;

    public interface IRepositoryQuery<T>
    {
        /// <summary>Gets the list of filters to be applied to the query.</summary>
        /// <value>The list of filters.</value>
        IEnumerable<Expression<Func<T, bool>>> Filters
        {
            get;
        }

        /// <summary>Gets the list of properties that would be loaded with the query.</summary>
        /// <value>The list of properties.</value>
        IEnumerable<Expression<Func<T, object>>> IncludedProperties
        {
            get;
        }

        /// <summary>Gets or sets the number of items to be skipped.</summary>
        /// <value>The number of items to be skipped.</value>
        int Skip
        {
            get;
            set;
        }

        /// <summary>Gets the list of criteria that would be used for sorting.</summary>
        /// <value>The list of sort criteria.</value>
        IEnumerable<ISortCriteria<T>> SortCriteriaList
        {
            get;
        }

        /// <summary>Gets or sets the number of items to be returned by the query.</summary>
        /// <value>The number of items to be returned by the query.</value>
        int Take
        {
            get;
            set;
        }

        /// <summary>Adds the filter.</summary>
        /// <param name="filter">The filter.</param>
        void AddFilter(Expression<Func<T, bool>> filter);

        /// <summary>Adds the sort criteria.</summary>
        /// <param name="sortCriteria">The sort criteria.</param>
        void AddSortCriteria(ISortCriteria<T> sortCriteria);

        /// <summary>Includes the property to be loaded with the query.</summary>
        /// <param name="property">The property.</param>
        void IncludeProperty(Expression<Func<T, object>> property);
    }
}