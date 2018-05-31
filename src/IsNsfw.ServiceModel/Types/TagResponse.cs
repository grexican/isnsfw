using System;
using System.Collections.Generic;
using System.Text;
using ServiceStack.DataAnnotations;

namespace IsNsfw.ServiceModel.Types
{
    public class TagResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public int SortOrder { get; set; }
    }
}
