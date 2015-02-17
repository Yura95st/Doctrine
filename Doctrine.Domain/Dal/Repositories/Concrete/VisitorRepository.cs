namespace Doctrine.Domain.Dal.Repositories.Concrete
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using Doctrine.Domain.Dal.Repositories.Abstract;
    using Doctrine.Domain.Dal.Repositories.Common;
    using Doctrine.Domain.Models;

    public class VisitorRepository : Repository<Visitor>, IVisitorRepository
    {
        public VisitorRepository(DoctrineContext context)
        : base(context)
        {
        }

        public Visitor GetByIpAddress(string ipAddress)
        {
            throw new System.NotImplementedException();
        }

        public Visitor GetById(int visitorId, params Expression<Func<Visitor, object>>[] selector)
        {
            return this.Get(v => v.VisitorId == visitorId, selector: selector).SingleOrDefault();
        }
    }
}