using System;
using System.Collections.Generic;
using System.Text;
using IsNsfw.ServiceModel.Types;
using ServiceStack;

namespace IsNsfw.ServiceModel
{
    [Route("/tags", HttpMethods.Get)]
    public class GetTagsRequest : IReturn<TagsResponse>, IGet
    {
    }
}
