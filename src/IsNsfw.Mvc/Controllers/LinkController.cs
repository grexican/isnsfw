using System;
using System.Linq;
using System.Threading.Tasks;
using IsNsfw.Model;
using IsNsfw.Mvc.Models;
using IsNsfw.ServiceModel;
using IsNsfw.ServiceModel.Types;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using ServiceStack;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IsNsfw.Mvc.Controllers
{
    public class LinkController : ControllerBase
    {
        public async Task<IActionResult> Index(string id)
        {
            var link = await Gateway.SendAsync(new CreateLinkEventRequest() { Key = id, LinkEventType = LinkEventType.View });

            if(link == null)
                return RedirectToAction("Error", "Home");

            var vm = link.ConvertTo<LinkViewModel>();
            await InitializeViewModel(vm, link);

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

        public async Task<IActionResult> Preview(string id)
        {
            var link = await Gateway.SendAsync(new CreateLinkEventRequest() { Key = id, LinkEventType = LinkEventType.Preview });

            if(link == null)
                return RedirectToAction("Error", "Home");

            var vm = link.ConvertTo<LinkViewModel>();
            await InitializeViewModel(vm, link);

            return new EmptyResult();
        }

        public async Task<IActionResult> ClickThrough(string id)
        {
            var link = await Gateway.SendAsync(new CreateLinkEventRequest() { Key = id, LinkEventType = LinkEventType.ClickThrough });

            if(link == null)
                return RedirectToAction("Error", "Home");

            var vm = link.ConvertTo<LinkViewModel>();
            await InitializeViewModel(vm, link);

            return Redirect(vm.Url);
        }

        public async Task<IActionResult> TurnBack(string id)
        {
            var link = await Gateway.SendAsync(new CreateLinkEventRequest() { Key = id, LinkEventType = LinkEventType.TurnBack });

            if(link == null)
                return RedirectToAction("Error", "Home");

            var vm = link.ConvertTo<LinkViewModel>();
            await InitializeViewModel(vm, link);

            return RedirectToAction("Index", "Home");
        }

        private async Task InitializeViewModel(LinkViewModel vm, LinkResponse link)
        {
            var domain = HostContext.AppSettings.GetString("Domain");

            if(domain.IsNullOrEmpty())
                domain = new Uri(Request.GetDisplayUrl()).GetComponents(UriComponents.SchemeAndServer,UriFormat.Unescaped);

            vm.ShortUrl = $"{domain.AppendPath(link.Key)}";

            if(vm.ScreenshotUrl.IsNullOrEmpty())
            {
                vm.IsScreenshotReady = false;
                vm.ScreenshotUrl = HostContext.AppSettings.GetString("DefaultScreenshotPlaceholder");
            }
            else
            {
                vm.IsScreenshotReady = true;
            }

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
