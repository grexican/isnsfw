using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IsNsfw.ServiceModel.Types;

namespace IsNsfw.Mvc.Models
{
    public class LinkViewModel
    {
        public string Key { get; set; }
        public string Url { get; set; }
        public List<TagViewModel> Tags { get; set; }
    }
}
