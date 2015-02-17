namespace Doctrine.Domain.Tests.TestHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Doctrine.Domain.Utils;

    public static class CollectionHelper
    {
        /// <summary>Filters the collection.</summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to filter.</param>
        /// <param name="filter">The filter expression.</param>
        /// <param name="orderBy">The orderBy function.</param>
        /// <returns>The filtered collection.</returns>
        public static IEnumerable<T> FilterCollection<T>(IEnumerable<T> collection, Expression<Func<T, bool>> filter = null,
                                                         Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null)
        {
            Guard.NotNull(collection, "collection");

            IQueryable<T> query = collection.AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                return orderBy(query)
                .ToList();
            }

            return query.ToList();
        }
    }
}