namespace Doctrine.Domain.Dal.Repositories.Abstract
{
    using Doctrine.Domain.Dal.Repositories.Common;
    using Doctrine.Domain.Models;

    public interface IVisitorRepository : IRepository<Visitor>
    {
        /// <summary>Gets the visitor by ip address.</summary>
        /// <param name="ipAddress">The ip address.</param>
        /// <returns>The visitor.</returns>
        Visitor GetByIpAddress(string ipAddress);
    }
}