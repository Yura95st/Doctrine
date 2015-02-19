namespace Doctrine.Domain.Services.Concrete
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Doctrine.Domain.Dal;
    using Doctrine.Domain.Exceptions.NotFound;
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

        public Article Create(int userId, string title, string text, int topicId, int[] tagIds)
        {
            Guard.IntMoreThanZero(userId, "userId");
            Guard.NotNullOrEmpty(title, "title");
            Guard.NotNullOrEmpty(text, "text");
            Guard.IntMoreThanZero(topicId, "topicId");
            Guard.NotNull(tagIds, "tagIds");

            if (tagIds.Any(t => t <= 0))
            {
                throw new ArgumentOutOfRangeException(String.Format("Argument '{0}' must contain only positive numbers.",
                "tagIds"));
            }

            User user = this._unitOfWork.UserRepository.GetById(userId);

            if (user == null)
            {
                throw new UserNotFoundException(String.Format("User with ID '{0}' was not found.", userId));
            }

            Topic topic = this._unitOfWork.TopicRepository.GetById(topicId);

            if (topic == null)
            {
                throw new TopicNotFoundException(String.Format("Topic with ID '{0}' was not found.", topicId));
            }

            string validatedTitle = this._articleValidation.ValidateTitle(title);
            string validatedText = this._articleValidation.ValidateArticleText(text);

            Article article = new Article
            {
                UserId = user.UserId, TopicId = topic.TopicId, Title = validatedTitle, Text = validatedText,
                PublicationDate = DateTime.Now
            };

            if (tagIds.Any())
            {
                IEnumerable<Tag> tags = this._unitOfWork.TagRepository.Get(t => tagIds.Contains(t.TagId));

                foreach (Tag tag in tags)
                {
                    article.Tags.Add(tag);
                }
            }

            this._unitOfWork.ArticleRepository.Insert(article);
            this._unitOfWork.Save();

            return article;
        }

        public void Delete(int articleId)
        {
            Guard.IntMoreThanZero(articleId, "articleId");

            Article article = this._unitOfWork.ArticleRepository.GetById(articleId);

            if (article == null)
            {
                throw new ArticleNotFoundException(String.Format("Article with ID '{0}' was not found.", articleId));
            }

            this._unitOfWork.ArticleRepository.Delete(article);

            this._unitOfWork.Save();
        }

        public Article GetById(int articleId)
        {
            Guard.IntMoreThanZero(articleId, "articleId");

            return this._unitOfWork.ArticleRepository.GetById(articleId);
        }

        #endregion
    }
}