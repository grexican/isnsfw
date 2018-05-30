using System;
using System.Collections.Generic;
using System.Text;
using ServiceStack.Model;

namespace IsNsfw.Repository.Interface
{
    public interface IRepository<T, TIdType> where T : IHasId<TIdType>
    {
        T GetById(TIdType id);
        void DeleteById(TIdType id);
        TIdType Create(T item);
        void Update(T item);
    }

    public interface IIntRepository<T> : IRepository<T, int> where T : IHasId<int> { }
}
