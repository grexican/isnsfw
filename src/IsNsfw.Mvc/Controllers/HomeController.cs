using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IsNsfw.Mvc.Models;
using IsNsfw.ServiceModel;
using ServiceStack;
using ServiceStack.Mvc;

namespace IsNsfw.Mvc.Controllers
{
    public class HomeController : ControllerBase
    {
        public async Task<IActionResult> Index()
        {
            var vm = new IndexViewModel();
            await InitializeViewModel(vm);

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Index(IndexViewModel vm)
        {
            await InitializeViewModel(vm);

            return DoIfValid<IActionResult>(() =>
            {
                var req = vm.ConvertTo<CreateLinkRequest>();
                req.Tags = Enumerable.ToHashSet(vm.Tags.Where(m => m.IsSelected).Select(m => m.Key));

                var link = Gateway.Send(req);

                return Redirect($"~/{link.Key}");
            }, () => View(vm));
        }


        private async Task InitializeViewModel(IndexViewModel vm)
        {
            var tags = await Gateway.SendAsync(new GetTagsRequest() { });

            vm.Tags = tags.Select(m =>
            {
                var t = m.ConvertTo<TagViewModel>();
                t.IsSelected = vm.Tags?.FirstOrDefault(r => r.Key == m.Key)?.IsSelected ?? false;

                return t;
            }).ToList();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
