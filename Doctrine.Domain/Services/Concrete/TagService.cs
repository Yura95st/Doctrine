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

    public class TagService : ServiceBase, ITagService
    {
        private readonly ITagValidation _tagValidation;

        public TagService(IUnitOfWork unitOfWork, ITagValidation tagValidation)
        : base(unitOfWork)
        {
            Guard.NotNull(tagValidation, "tagValidation");

            this._tagValidation = tagValidation;
        }

        #region ITagService Members

        public Tag Create(string tagName)
        {
            Guard.NotNullOrEmpty(tagName, "tagName");

            if (!this._tagValidation.IsValidName(tagName))
            {
                throw new InvalidTagNameFormatException(String.Format("Tag's name '{0}' has invalid format.", tagName));
            }

            Tag tag = new Tag { Name = tagName };

            this._unitOfWork.TagRepository.Insert(tag);

            this._unitOfWork.Save();

            return tag;
        }

        public Tag GetByName(string tagName)
        {
            Guard.NotNullOrEmpty(tagName, "tagName");

            return this._unitOfWork.TagRepository.GetByName(tagName);
        }

        #endregion
    }
}