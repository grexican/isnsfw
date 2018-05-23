using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using IsNsfw.Model;
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

        public virtual T GetById(TIndex id)
        {
            return Execute(db => db.SingleById<T>(id));
        }

        public virtual void DeleteById(TIndex id)
        {
            if(typeof(ISoftDelete).IsAssignableFrom(typeof(T)))
            {
                Execute(db => db.Update<T>(new { IsDeleted = true }, where: m => m.Id.Equals(id)));
            }
            else
            {
                Execute(db => db.DeleteById<T>(id));
            }
        }

        public void ExecuteTransaction(Action<IDbConnection, IDbTransaction> a)
        {
            using(var db = _factory.OpenDbConnection())
            using(var trans = db.OpenTransaction())
            {
                a(db, trans);
            }
        }

        public void UnitOfWork(Action<IDbConnection> a)
        {
            using (var db = _factory.OpenDbConnection())
            using (var trans = db.OpenTransaction())
            {
                a(db);

                trans.Commit();
            }
        }

        public virtual void Execute(Action<IDbConnection> a)
        {
            using(var db = _factory.OpenDbConnection())
                a(db);
        }

        public virtual TResult Execute<TResult>(Func<IDbConnection, TResult> a)
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
