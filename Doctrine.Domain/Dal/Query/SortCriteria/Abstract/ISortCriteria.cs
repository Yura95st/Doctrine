namespace Doctrine.Domain.Dal.Query.SortCriteria.Abstract
{
    using System.Linq;

    public interface ISortCriteria<T>
    {
        /// <summary>Applies the ordering to the ordered query.</summary>
        /// <param name="query">The ordered query.</param>
        /// <returns>The query with applied ordering.</returns>
        IOrderedQueryable<T> ApplyOrdering(IOrderedQueryable<T> query);

        /// <summary>Applies the ordering to the query.</summary>
        /// <param name="query">The query.</param>
        /// <returns>The query with applied ordering.</returns>
        IOrderedQueryable<T> ApplyOrdering(IQueryable<T> query);
    }
}