using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ServiceStack.Caching;
using ServiceStack.Data;
using ServiceStack.OrmLite;

namespace IsNsfw.Command
{
    public abstract class CommandHandlerBase
    {
        private readonly IDbConnectionFactory _factory;

        protected CommandHandlerBase(IDbConnectionFactory factory)
        {
            _factory = factory;
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
            using(var db = _factory.OpenDbConnection())
            using(var trans = db.OpenTransaction())
            {
                a(db);

                trans.Commit();
            }
        }

        public void Execute(Action<IDbConnection> a)
        {
            using(var db = _factory.OpenDbConnection())
                a(db);
        }
    }
}
