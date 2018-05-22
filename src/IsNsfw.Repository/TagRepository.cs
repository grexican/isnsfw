using System;
using System.Collections.Generic;
using System.Text;
using IsNsfw.Model;
using IsNsfw.Repository.Interface;
using ServiceStack.Data;
using ServiceStack.OrmLite;

namespace IsNsfw.Repository
{
    public class TagRepository : IntRepositoryBase<Tag>, ITagRepository
    {
        public TagRepository(IDbConnectionFactory factory) : base(factory)
        {
        }

        public bool KeyExists(string key)
        {
            return Execute(db => db.Exists<Tag>(m => m.Key == key));
        }

        public Tag GetByKey(string key)
        {
            return Execute(db => db.Single<Tag>(m => m.Key == key));
        }

        public List<Tag> GetOrderedTags()
        {
            return Execute(db => db.Select(db.From<Tag>().Where(m => !m.IsDeleted).OrderBy(m => m.SortOrder).ThenBy(m => m.Key)));
        }
    }
}
