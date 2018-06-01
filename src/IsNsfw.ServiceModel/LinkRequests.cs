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

    /// <summary>
    /// Registers a "view" event
    /// </summary>
    [Route("/links/{Key}", HttpMethods.Get)]
    public class GetLinkRequest : IGet, IReturn<LinkResponse>
    {
        public string Key { get; set; }
    }

    [Route("/links/{Key}/analytics", HttpMethods.Get)]
    public class GetLinkAnalyticsRequest : IPost, IReturn<LinkResponse>
    {
        public string Key { get; set; }
    }

    [Route("/links/{Key}/event", HttpMethods.Post)]
    [Route("/links/{Key}/{LinkEventType}", HttpMethods.Post)]
    public class CreateLinkEventRequest : IPost, IReturn<LinkResponse>
    {
        public string Key { get; set; }
        public LinkEventType LinkEventType { get; set; }
    }
}
