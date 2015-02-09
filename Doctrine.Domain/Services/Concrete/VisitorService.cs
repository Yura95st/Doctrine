namespace Doctrine.Domain.Services.Concrete
{
    using System;
    using System.Linq;

    using Doctrine.Domain.Dal;
    using Doctrine.Domain.Exceptions.InvalidFormat;
    using Doctrine.Domain.Exceptions.NotFound;
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

        public void ViewArticle(int visitorId, int articleId)
        {
            Guard.IntMoreThanZero(visitorId, "userId");
            Guard.IntMoreThanZero(articleId, "articleId");

            Visitor visitor =
            this._unitOfWork.VisitorRepository.Get(v => v.VisitorId == visitorId, selector: v => v.ArticleVisitors)
            .FirstOrDefault();

            if (visitor == null)
            {
                throw new VisitorNotFoundException(String.Format("Visitor with ID '{0}' was not found.", visitorId));
            }

            ArticleVisitor articleVisitor = visitor.ArticleVisitors.FirstOrDefault(x => x.ArticleId == articleId);

            if (articleVisitor == null)
            {
                Article article = this._unitOfWork.ArticleRepository.GetById(articleId);

                if (article == null)
                {
                    throw new ArticleNotFoundException(String.Format("Article with ID '{0}' was not found.", articleId));
                }

                articleVisitor = new ArticleVisitor { VisitorId = visitor.VisitorId, ArticleId = article.ArticleId };

                visitor.ArticleVisitors.Add(articleVisitor);
            }

            articleVisitor.LastViewDate = DateTime.Now;

            this._unitOfWork.VisitorRepository.Update(visitor);
            this._unitOfWork.Save();
        }

        #endregion
    }
}