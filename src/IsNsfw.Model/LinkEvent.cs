using System;
using ServiceStack.DataAnnotations;
using ServiceStack.Model;
using ServiceStack.OrmLite;

namespace IsNsfw.Model
{
    public class LinkEvent : IHasIntId, ICreatedAt
    {
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string SessionId { get; set; }

        [Reference]
        public Link Link { get; set; }

        [Index]
        [References(typeof(Link))]
        public int LinkId { get; set; }

        [Reference]
        public User User { get; set; }

        [References(typeof(User))]
        public int? UserId { get; set; }

        public LinkEventType LinkEventType { get; set; }

        [Default(OrmLiteVariables.SystemUtc)]           // Populated with UTC Date by RDBMS
        public DateTime CreatedAt { get; set; }
    }
}
