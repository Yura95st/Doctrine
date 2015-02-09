namespace Doctrine.Domain.Services.Common
{
    using Doctrine.Domain.Dal;
    using Doctrine.Domain.Utils;

    public abstract class ServiceBase
    {
        protected readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBase"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        protected ServiceBase(IUnitOfWork unitOfWork)
        {
            Guard.NotNull(unitOfWork, "unitOfWork");
            this._unitOfWork = unitOfWork;
        }
    }
}