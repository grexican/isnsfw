using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IsNsfw.ServiceModel.Types;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IsNsfw.Mvc.Models
{
    public class IndexViewModel
    {
        public string Key { get; set; }
        public string Url { get; set; }
        public List<TagViewModel> Tags { get; set; }
    }
}
