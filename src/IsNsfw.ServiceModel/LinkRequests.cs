using System.Collections.Generic;
using IsNsfw.Model;
using IsNsfw.ServiceModel.Types;
using ServiceStack;

namespace IsNsfw.ServiceModel
{
    [Route("/links", HttpMethods.Post)]
    public class CreateLinkRequest : IPost, IReturn<LinkResponse>
    {
        public string Key { get; set; }
        public string Url { get; set; }
        public HashSet<string> Tags { get; set; }
    }

    [Route("/links/{Id}/event", HttpMethods.Post)]
    public class CreateLinkEventRequest : IPost, IReturnVoid
    {
        public int Id { get; set; }
        public LinkEventType LinkEventType { get; set; }
    }
}
