using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using IsNsfw.Repository.Interface;
using ServiceStack;
using ServiceStack.Messaging;
using ServiceStack.Text;

namespace IsNsfw.ServiceInterface
{
    public class CreateLinkScreenshotRequest
    {
        public int Id { get; set; }
    }

    public interface ICreateLinkScreenshotHandler
    {
        object Handle(IMessage<CreateLinkScreenshotRequest> request);
    }

    public class ScreenshotService : ServiceBase, ICreateLinkScreenshotHandler
        //, IAny<CreateLinkScreenshotRequest>
    {
        private const string CreateUrl  = "https://api.browshot.com/api/v1/screenshot/create?key=##KEY##&format=jpeg&url=##URL##";
        private const string InfoUrl    = "https://api.browshot.com/api/v1/screenshot/info?id=##ID##&key=##KEY##";

        private readonly ILinkRepository _linkRepo;

        public ScreenshotService(ILinkRepository linkRepo)
        {
            _linkRepo = linkRepo;
        }

        public object Handle(IMessage<CreateLinkScreenshotRequest> message)
        {
            var request = message.GetBody();

            var link = _linkRepo.GetById(request.Id);

            if (link == null) throw HttpError.NotFound($"Link with ID '{request.Id}' not found.");

            var key = HostContext.AppSettings.Get<string>("Browshot.ApiKey", null);
            var url = CreateUrl.Replace("##KEY##", key).Replace("##URL##", link.Url.UrlEncode());

            var jsonString = url.GetStringFromUrl();

            var json = JsonObject.Parse(jsonString);
            var id = json["id"];

            url = InfoUrl.Replace("##ID##", id).Replace("##KEY##", key);

            var retriesLeft = 30;
            string status = null;
            string screenshotUrl = null;

            do
            {
                Thread.Sleep(1000); // sleep a second... it takes a while to generate these
                --retriesLeft;

                jsonString = url.GetStringFromUrl();
                json = JsonObject.Parse(jsonString);

                status = json["status"];
                
                if(status == "finished") screenshotUrl = json["screenshot_url"];
            } while (status != "finished" && retriesLeft > 0);

            if(screenshotUrl == null)
                throw HttpError.NotFound($"Not able to generate screenshot for Link with ID '{request.Id}'.");

            _linkRepo.SetScreenshotUrl(link.Id, screenshotUrl);

            link.ScreenshotUrl = screenshotUrl;

            return link;
        }

        //public object Any(CreateLinkScreenshotRequest request)
        //{
        //    var msg = new Message<CreateLinkScreenshotRequest>(request);
        //    return Handle(msg);
        //}
    }
}
