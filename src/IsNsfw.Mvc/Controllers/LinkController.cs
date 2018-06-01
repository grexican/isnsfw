using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IsNsfw.ServiceModel;
using Microsoft.AspNetCore.Mvc;
using ServiceStack;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IsNsfw.Mvc.Controllers
{
    public class LinkController : ControllerBase
    {
        public async Task<IActionResult> Index(string id)
        {
            var vm = await Gateway.SendAsync(new GetLinkRequest() { Key = id });

            if(vm == null)
                return RedirectToAction("Error", "Home");

            return View(vm);
        }
    }
}
