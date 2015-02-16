namespace Doctrine.Domain.Services.Concrete
{
    using Doctrine.Domain.Dal;
    using Doctrine.Domain.Models;
    using Doctrine.Domain.Services.Abstract;
    using Doctrine.Domain.Services.Common;
    using Doctrine.Domain.Utils;
    using Doctrine.Domain.Validation.Abstract;

    public class ArticleService : ServiceBase, IArticleService
    {
        private readonly IArticleValidation _articleValidation;

        public ArticleService(IUnitOfWork unitOfWork, IArticleValidation articleValidation)
        : base(unitOfWork)
        {
            Guard.NotNull(articleValidation, "articleValidation");

            this._articleValidation = articleValidation;
        }

        #region IArticleService Members

        public Article Create(int userId, string title, string text, int topicId)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(int articleId)
        {
            throw new System.NotImplementedException();
        }

        public Article GetById(int articleId)
        {
            Guard.IntMoreThanZero(articleId, "articleId");

            return this._unitOfWork.ArticleRepository.GetById(articleId);
        }

        #endregion
    }
}