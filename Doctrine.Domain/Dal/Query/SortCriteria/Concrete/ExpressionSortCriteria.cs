namespace Doctrine.Domain.Dal.Query.SortCriteria.Concrete
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using Doctrine.Domain.Dal.Query.SortCriteria.Abstract;
    using Doctrine.Domain.Enums;
    using Doctrine.Domain.Utils;

    public class ExpressionSortCriteria<T, TKey> : ISortCriteria<T>
    {
        private readonly SortDirection _direction;

        private readonly Expression<Func<T, TKey>> _sortExpression;

        public ExpressionSortCriteria(Expression<Func<T, TKey>> sortExpression, SortDirection direction)
        {
            Guard.NotNull(sortExpression, "sortExpression");

            this._sortExpression = sortExpression;
            this._direction = direction;
        }

        #region ISortCriteria<T> Members

        public IOrderedQueryable<T> ApplyOrdering(IOrderedQueryable<T> query)
        {
            Guard.NotNull(query, "query");

            return this._direction == SortDirection.Descending
            ? query.ThenByDescending(this._sortExpression) : query.ThenBy(this._sortExpression);
        }

        public IOrderedQueryable<T> ApplyOrdering(IQueryable<T> query)
        {
            Guard.NotNull(query, "query");

            return this._direction == SortDirection.Descending
            ? query.OrderByDescending(this._sortExpression) : query.OrderBy(this._sortExpression);
        }

        #endregion
    }
}