namespace Doctrine.Domain.Services.Concrete
{
    using System;
    using System.Collections.Generic;
    using System.Net;

    using Doctrine.Domain.Dal;
    using Doctrine.Domain.Exceptions;
    using Doctrine.Domain.Models;
    using Doctrine.Domain.Services.Abstract;
    using Doctrine.Domain.Utils;

    public class VisitorService : IVisitorService
    {
        private readonly IUnitOfWork _unitOfWork;

        public VisitorService(IUnitOfWork unitOfWork)
        {
            Guard.NotNull(unitOfWork, "unitOfWork");

            this._unitOfWork = unitOfWork;
        }

        #region IVisitorService Members

        public void Create(Visitor entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Visitor entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Visitor> GetAll()
        {
            throw new NotImplementedException();
        }

        public Visitor GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(Visitor entity)
        {
            throw new NotImplementedException();
        }

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