using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IsNsfw.Model;
using ServiceStack;

namespace IsNsfw.ServiceModel.Types
{
    public class TagsResponse : List<TagResponse>
    {
        public TagsResponse()
        {
            
        }

        public TagsResponse(IEnumerable<TagResponse> collection) : base(collection)
        {
        }

        public TagsResponse(IEnumerable<Tag> collection) : base(collection.Select(m => m.ConvertTo<TagResponse>()))
        {
        }
    }
}
