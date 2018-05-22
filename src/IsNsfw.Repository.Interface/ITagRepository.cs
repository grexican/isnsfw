using System;
using System.Collections.Generic;
using System.Text;
using IsNsfw.Model;

namespace IsNsfw.Repository.Interface
{
    public interface ITagRepository : IIntRepository<Tag>
    {
        bool KeyExists(string key);
        Tag GetByKey(string key);
        List<Tag> GetOrderedTags();
    }
}
