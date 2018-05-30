using System.Collections.Generic;

namespace IsNsfw.ServiceModel.Types
{
    public class LinkResponse
    {
        public int Id { get; set; }

        public string Key { get; set; }

        public string SessionId { get; set; }

        public string Url { get; set; }

        public bool IsDeleted { get; set; }

        public int TotalViews { get; set; }
        public int TotalPreviews { get; set; }
        public int TotalClickThroughs { get; set; }
        public int TotalTurnBacks { get; set; }

        public int? UserId { get; set; }

        public HashSet<string> Tags { get; set; }
    }
}