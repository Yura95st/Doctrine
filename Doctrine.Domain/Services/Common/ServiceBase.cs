namespace Doctrine.Domain.Services.Common
{
    using Doctrine.Domain.Dal;
    using Doctrine.Domain.Utils;

    public abstract class ServiceBase
    {
        protected readonly IUnitOfWork _unitOfWork;

        protected ServiceBase(IUnitOfWork unitOfWork)
        {
            Guard.NotNull(unitOfWork, "unitOfWork");
            this._unitOfWork = unitOfWork;
        }
    }
}