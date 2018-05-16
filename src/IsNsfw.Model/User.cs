using System;
using System.Collections.Generic;
using System.Text;
using ServiceStack.DataAnnotations;
using ServiceStack.Model;

namespace IsNsfw.Model
{
    [Alias("Users")]
    public class User : IHasIntId, ISoftDelete
    {
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }

        [Index(Unique = true)]
        public int? UserAuthId { get; set; }

        public bool IsDeleted { get; set; }
    }
}
