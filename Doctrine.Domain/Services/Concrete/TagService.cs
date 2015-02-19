namespace Doctrine.Domain.Services.Concrete
{
    using System;

    using Doctrine.Domain.Dal;
    using Doctrine.Domain.Exceptions.AlreadyExists;
    using Doctrine.Domain.Exceptions.InvalidFormat;
    using Doctrine.Domain.Exceptions.NotFound;
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

            Tag tag = this._unitOfWork.TagRepository.GetByName(tagName);

            if (tag != null)
            {
                throw new TagNameAlreadyExistsException(String.Format("Tag with name '{0}' already exists.", tagName));
            }

            tag = new Tag { Name = tagName };

            this._unitOfWork.TagRepository.Insert(tag);

            this._unitOfWork.Save();

            return tag;
        }

        public void Edit(int tagId, string newTagName)
        {
            Guard.IntMoreThanZero(tagId, "tagId");
            Guard.NotNullOrEmpty(newTagName, "newTagName");

            if (!this._tagValidation.IsValidName(newTagName))
            {
                throw new InvalidTagNameFormatException(String.Format("Tag's new name '{0}' has invalid format.", newTagName));
            }

            Tag tag = this._unitOfWork.TagRepository.GetByName(newTagName);

            if (tag != null)
            {
                throw new TagNameAlreadyExistsException(String.Format("Tag with name '{0}' already exists.", newTagName));
            }

            tag = this._unitOfWork.TagRepository.GetById(tagId);

            if (tag == null)
            {
                throw new TagNotFoundException(String.Format("Tag with ID '{0}' was not found.", tagId));
            }

            tag.Name = newTagName;

            this._unitOfWork.TagRepository.Update(tag);

            this._unitOfWork.Save();
        }

        public Tag GetByName(string tagName)
        {
            Guard.NotNullOrEmpty(tagName, "tagName");

            return this._unitOfWork.TagRepository.GetByName(tagName);
        }

        #endregion
    }
}