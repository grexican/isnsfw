using System;
using System.Collections.Generic;
using System.Text;
using IsNsfw.Model;
using IsNsfw.Repository.Interface;
using ServiceStack.Data;
using ServiceStack.OrmLite;

namespace IsNsfw.Repository
{
    public class LinkRepository : IntRepositoryBase<Link>, ILinkRepository
    {
        public LinkRepository(IDbConnectionFactory factory) : base(factory)
        {
        }

        public bool KeyExists(string key)
        {
            return Execute(db => db.Exists<Link>(m => m.Key == key));
        }

        public Link GetByKey(string key)
        {
            return Execute(db => db.Single<Link>(m => m.Key == key));
        }
    }
}
