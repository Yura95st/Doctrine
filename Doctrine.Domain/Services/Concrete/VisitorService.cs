namespace Doctrine.Domain.Services.Concrete
{
    using System;
    using System.Net;

    using Doctrine.Domain.Dal;
    using Doctrine.Domain.Exceptions;
    using Doctrine.Domain.Models;
    using Doctrine.Domain.Services.Abstract;
    using Doctrine.Domain.Services.Common;
    using Doctrine.Domain.Utils;

    public class VisitorService : ServiceBase, IVisitorService
    {
        public VisitorService(IUnitOfWork unitOfWork)
        : base(unitOfWork)
        {
        }

        #region IVisitorService Members

        public Visitor RegisterIpAddress(string ipAddress)
        {
            Guard.NotNull(ipAddress, "ipAddress");

            IPAddress address;
            if (!IPAddress.TryParse(ipAddress, out address))
            {
                throw new InvalidIpAddressFormatException(String.Format("Ip address '{0}' has invalid format.", ipAddress));
            }

            ipAddress = address.ToString();

            Visitor visitor = this._unitOfWork.VisitorRepository.GetByIpAddress(ipAddress);

            if (visitor == null)
            {
                // Ip address is not registered yet
                visitor = new Visitor { IpAddress = ipAddress };

                this._unitOfWork.VisitorRepository.Insert(visitor);
                this._unitOfWork.Save();
            }

            return visitor;
        }

        #endregion
    }
}