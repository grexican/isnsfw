using System;
using System.Collections.Generic;
using System.Text;
using IsNsfw.Model;

namespace IsNsfw.Repository.Interface
{
    public interface ILinkRepository : IIntRepository<Link>
    {
        bool KeyExists(string key);
        Link GetByKey(string key);
    }
}
