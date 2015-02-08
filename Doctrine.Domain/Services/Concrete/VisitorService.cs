namespace Doctrine.Domain.Services.Concrete
{
    using System;

    using Doctrine.Domain.Dal;
    using Doctrine.Domain.Exceptions.InvalidFormat;
    using Doctrine.Domain.Models;
    using Doctrine.Domain.Services.Abstract;
    using Doctrine.Domain.Services.Common;
    using Doctrine.Domain.Utils;
    using Doctrine.Domain.Validation.Abstract;

    public class VisitorService : ServiceBase, IVisitorService
    {
        private readonly IVisitorValidation _visitorValidation;

        public VisitorService(IUnitOfWork unitOfWork, IVisitorValidation visitorValidation)
        : base(unitOfWork)
        {
            Guard.NotNull(visitorValidation, "visitorValidation");

            this._visitorValidation = visitorValidation;
        }

        #region IVisitorService Members

        public Visitor RegisterIpAddress(string ipAddress)
        {
            Guard.NotNull(ipAddress, "ipAddress");

            if (!this._visitorValidation.IsValidIpAddress(ipAddress))
            {
                throw new InvalidIpAddressFormatException(String.Format("IP address '{0}' has invalid format.", ipAddress));
            }

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