using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using IsNsfw.Repository.Interface;
using ServiceStack.Data;
using ServiceStack.Model;
using ServiceStack.OrmLite;

namespace IsNsfw.Repository
{
    public abstract class RepositoryBase<T, TIndex> : IRepository<T, TIndex> where T : IHasId<TIndex>
    {
        private readonly IDbConnectionFactory _factory;

        protected RepositoryBase(IDbConnectionFactory factory)
        {
            _factory = factory;
        }

        public T GetById(TIndex id)
        {
            return Execute(db => db.SingleById<T>(id));
        }

        public void DeleteById(TIndex id)
        {
            Execute(db => db.DeleteById<T>(id));
        }

        public void Execute(Action<IDbConnection> a)
        {
            using(var db = _factory.OpenDbConnection())
                a(db);
        }

        public TResult Execute<TResult>(Func<IDbConnection, TResult> a)
        {
            using(var db = _factory.OpenDbConnection())
                return a(db);
        }
    }

    public abstract class IntRepositoryBase<T> : RepositoryBase<T, int> where T : IHasId<int> 
    {
        protected IntRepositoryBase(IDbConnectionFactory factory) : base(factory)
        {
        }
    }
}
