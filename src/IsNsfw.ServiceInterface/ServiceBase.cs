using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Transactions;
using ServiceStack.Auth;

namespace IsNsfw.ServiceInterface
{
    public class ServiceBase : ServiceStack.Service
    {
        private IAuthSession _session;
        public IAuthSession Session => _session ?? (_session = this.GetSession());

        public void ExecuteTransaction(Action<TransactionScope> a)
        {
            using (var trans = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                a(trans);
            }
        }

        public TResult ExecuteTransaction<TResult>(Func<TransactionScope, TResult> a)
        {
            using (var trans = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                return a(trans);
            }
        }

        public void UnitOfWork(Action a)
        {
            using (var trans = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                a();

                trans.Complete();
            }
        }

        public TResult UnitOfWork<TResult>(Func<TResult> a)
        {
            using (var trans = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                var ret = a();

                trans.Complete();

                return ret;
            }
        }
    }
}
