namespace Doctrine.Domain.Services.Concrete
{
    using System;
    using System.Collections.Generic;

    using Doctrine.Domain.Models;
    using Doctrine.Domain.Services.Abstract;

    public class ArticleService : IArticleService
    {
        #region IArticleService Members

        public void Create(Article entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Article entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Article> GetAll()
        {
            throw new NotImplementedException();
        }

        public Article GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(Article entity)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}