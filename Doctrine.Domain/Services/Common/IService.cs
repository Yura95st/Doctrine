﻿namespace Doctrine.Domain.Services.Common
{
    using System.Collections.Generic;

    public interface IService<T>
    where T : class
    {
        void Create(T entity);

        void Delete(T entity);

        IEnumerable<T> GetAll();

        T GetById(int id);

        void Update(T entity);
    }
}