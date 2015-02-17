namespace Doctrine.Domain.Dal.Repositories.Abstract
{
    using System;
    using System.Linq.Expressions;

    using Doctrine.Domain.Dal.Repositories.Common;
    using Doctrine.Domain.Models;

    public interface IVisitorRepository : IRepository<Visitor>
    {
        /// <summary>Gets the visitor by ip address.</summary>
        /// <param name="ipAddress">The ip address.</param>
        /// <returns>The visitor.</returns>
        Visitor GetByIpAddress(string ipAddress);

        /// <summary>Gets the visitor by identifier.</summary>
        /// <param name="visitorId">The visitor's identifier.</param>
        /// <param name="selector">The selector.</param>
        /// <returns>The visitor.</returns>
        Visitor GetById(int visitorId, params Expression<Func<Visitor, object>>[] selector);
    }
}