using ServiceStack.DataAnnotations;
using ServiceStack.Model;

namespace IsNsfw.Model
{
    [Alias("Tags")]
    public class Tag : IHasIntId, ISoftDelete
    {
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }

        public string Name { get; set; }

        [Index(Unique = true)]
        [StringLength(5)]
        public string Key { get; set; }

        [StringLength(50)]
        public string ShortDescription { get; set; }

        [StringLength(1000)]
        public string LongDescription { get; set; }

        public bool IsDeleted { get; set; }
    }
}