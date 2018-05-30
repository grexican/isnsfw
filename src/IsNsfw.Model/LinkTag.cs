using ServiceStack.DataAnnotations;

namespace IsNsfw.Model
{
    [Alias("LinkTags")]
    [CompositeIndex(nameof(LinkId), nameof(TagId))]
    public class LinkTag
    {
        [PrimaryKey]
        [Ignore]
        public string Id => $"{this.LinkId}/{this.TagId}";

        [References(typeof(Link))]
        public int LinkId { get; set; }

        [References(typeof(Tag))]
        public int TagId { get; set; }
    }
}