using ServiceStack.DataAnnotations;

namespace IsNsfw.Model
{
    [Alias("LinkTags")]
    public class LinkTag
    {
        [References(typeof(Link))]
        public int LinkId { get; set; }

        [References(typeof(Tag))]
        public int TagId { get; set; }
    }
}