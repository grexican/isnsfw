using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IsNsfw.Model;
using IsNsfw.Mvc.Models;
using IsNsfw.ServiceModel;
using IsNsfw.ServiceModel.Types;
using Microsoft.AspNetCore.Mvc;
using ServiceStack;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IsNsfw.Mvc.Controllers
{
    public class LinkController : ControllerBase
    {
        public async Task<IActionResult> Index(string id)
        {
            var vm = await Gateway.SendAsync(new CreateLinkEventRequest() { Key = id, LinkEventType = LinkEventType.View });

            if(vm == null)
                return RedirectToAction("Error", "Home");

            return View(vm);
        }

        public async Task<IActionResult> Share(string id)
        {
            var link = await Gateway.SendAsync(new GetLinkRequest() { Key = id });

            if(link == null)
                return RedirectToAction("Error", "Home");

            var vm = link.ConvertTo<LinkViewModel>();
            await InitializeViewModel(vm, link);

            return View(vm);
        }

        private async Task InitializeViewModel(LinkViewModel vm, LinkResponse link)
        {
            var tags = await Gateway.SendAsync(new GetTagsRequest() { });

            vm.Tags = tags.Select(m =>
            {
                var t = m.ConvertTo<TagViewModel>();
                t.IsSelected = vm.Tags?.FirstOrDefault(r => r.Key == m.Key)?.IsSelected ?? link?.Tags?.Contains(m.Key) ?? false;

                return t;
            })
            .Where(m => m.IsSelected) // for this VM, we only care about SELECTED tags
            .ToList();
        }
    }
}
