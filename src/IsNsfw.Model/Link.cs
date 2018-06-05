using System;
using System.Collections.Generic;
using System.Text;
using ServiceStack.Auth;
using ServiceStack.DataAnnotations;
using ServiceStack.Model;
using ServiceStack.OrmLite;

namespace IsNsfw.Model
{
    [Alias("Links")]
    public class Link : IHasIntId, ISoftDelete, ICreatedAt
    {
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }

        [Index(Unique = true)]
        [StringLength(20)]
        public string Key { get; set; }

        [Required]
        [StringLength(20)]
        public string SessionId { get; set; }

        [StringLength(1000)]
        public string Url { get; set; }

        public bool IsDeleted { get; set; }

        public int TotalViews { get; set; }
        public int TotalPreviews { get; set; }
        public int TotalClickThroughs { get; set; }
        public int TotalTurnBacks { get; set; }

        [Reference]
        public User User { get; set; }

        [References(typeof(User))]
        public int? UserId { get; set; }

        //[PgSqlIntArray]
        //public List<int> Tags { get; set; }

        [Reference]
        public List<LinkTag> LinkTags { get; set; }

        [StringLength(200)]
        public string ScreenshotUrl { get; set; }

        [Default(OrmLiteVariables.SystemUtc)]           // Populated with UTC Date by RDBMS
        public DateTime CreatedAt { get; set; }

        //[Ignore]
        //public List<Tag> Tags { get; set; }
    }
}
